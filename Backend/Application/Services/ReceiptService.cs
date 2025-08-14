using Application.DTOs;
using Application.Events;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using System;

namespace Application.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public ReceiptService(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<IEnumerable<ReceiptDocumentDto>> GetAllReceiptDocumentsAsync(ReceiptFilterDto filters)
        {
            var filtersObj = _mapper.Map<ReceiptFilter>(filters);
            var receiptDocuments = await _unitOfWork.ReceiptDocuments.GetAllAsync(filtersObj);
            return _mapper.Map<IEnumerable<ReceiptDocumentDto>>(receiptDocuments);
        }

        public async Task<ReceiptDocumentDto> GetReceiptDocumentByIdAsync(Guid id)
        {
            var receiptDocument = await _unitOfWork.ReceiptDocuments.GetByIdAsync(id);
            return _mapper.Map<ReceiptDocumentDto>(receiptDocument);
        }

        public async Task UpdateReceiptDocumentAsync(ReceiptDocumentUpdateDto receiptDto)
        {
            var receiptDocument = _mapper.Map<ReceiptDocument>(receiptDto);
            var existingDocument = await _unitOfWork.ReceiptDocuments.GetByIdAsync(receiptDto.Id);
            if (existingDocument == null) return;
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var originalResources = existingDocument.ReceiptResources
                    .Select(r => new ReceiptResource
                    {
                        Id = r.Id,
                        ResourceId = r.ResourceId,
                        MeasureUnitId = r.MeasureUnitId,
                        Count = r.Count
                    })
                    .ToList();
                var updatedDocument = _mapper.Map<ReceiptDocument>(receiptDto);

                _unitOfWork.ReceiptDocuments.Update(updatedDocument);

                var updatedResources = updatedDocument.ReceiptResources
                    .Select(r => new ReceiptResource
                    {
                        Id = r.Id,
                        ResourceId = r.ResourceId,
                        MeasureUnitId = r.MeasureUnitId,
                        Count = r.Count
                    })
                    .ToList();

                await _mediator.Publish(new ReceiptDocumentUpdatedEvent(
                    existingDocument.Id,
                    originalResources,
                    updatedResources
                ));


                await _unitOfWork.SaveEntitiesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Ошибка: {ex.Message.ToString()}");
            }
        }

        public async Task<Guid> CreateReceiptDocumentAsync(ReceiptDocumentCreateDto receiptDto)
        {
            var receiptDocument = _mapper.Map<ReceiptDocument>(receiptDto);
            if (await _unitOfWork.ReceiptDocuments.ExistsByNumAsync(receiptDocument.Num))
                throw new Exception("Документ поступления с таким номером уже существует");
            using var transaction =  await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.ReceiptDocuments.AddAsync(receiptDocument);
                await _mediator.Publish(new ReceiptDocumentCreatedEvent(receiptDocument.Id, receiptDocument.ReceiptResources));
                await _unitOfWork.SaveEntitiesAsync();
                await transaction.CommitAsync();
                return receiptDocument.Id;
            }
            catch (Exception ex) 
            {
                await transaction.RollbackAsync();
                throw new Exception($"Ошибка: {ex.Message.ToString()}");
            }
        }

        public async Task DeleteReceiptDocumentAsync(Guid id)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var document = await _unitOfWork.ReceiptDocuments.GetByIdUntrackedAsync(id);
                if (document == null) return;

                var resources = document.ReceiptResources.ToList();

                _unitOfWork.ReceiptDocuments.Delete(document);
                await _unitOfWork.SaveEntitiesAsync();

                await _mediator.Publish(new UpdateBalanceEvent(
                resources.Select(r =>
                    new ResourceOperation(
                        r.ResourceId,
                        r.MeasureUnitId,
                        r.Count,
                        OperationType.Decrease
                    )).ToList()
                ));

                await _mediator.Publish(new RemoveZeroBalancesEvent(
                    resources.Select(r =>
                        new ResourceOperation(
                            r.ResourceId,
                            r.MeasureUnitId,
                            r.Count,
                            OperationType.Decrease
                        )).ToList()
                ));

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

    }
}
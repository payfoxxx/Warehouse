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
    public class ShipmentService : IShipmentService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public ShipmentService(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<IEnumerable<ShipmentDocumentDto>> GetAllShipmentDocumentsAsync(ShipmentFilterDto filters)
        {
            var filtersObj = _mapper.Map<ShipmentFilter>(filters);
            var shipmentDocuments = await _unitOfWork.ShipmentDocuments.GetAllAsync(filtersObj);
            return _mapper.Map<IEnumerable<ShipmentDocumentDto>>(shipmentDocuments);
        }

        public async Task<ShipmentDocumentDto> GetShipmentDocumentByIdAsync(Guid id)
        {
            var shipmentDocument = await _unitOfWork.ShipmentDocuments.GetByIdAsync(id);
            return _mapper.Map<ShipmentDocumentDto>(shipmentDocument);
        }

        public async Task UpdateShipmentDocumentAsync(ShipmentDocumentUpdateDto shipmentDto)
        {
            var document = _mapper.Map<ShipmentDocument>(shipmentDto);
            if (document == null) return;
            if (string.IsNullOrWhiteSpace(document.Num))
                throw new Exception("Номер не может быть пустым");
            _unitOfWork.ShipmentDocuments.Update(document);
            await _unitOfWork.CommitAsync();
        }

        public async Task<Guid> CreateShipmentDocumentAsync(ShipmentDocumentCreateDto shipmentDto)
        {
            var shipmentDocument = _mapper.Map<ShipmentDocument>(shipmentDto);
            if (await _unitOfWork.ShipmentDocuments.ExistsByNumAsync(shipmentDocument.Num))
                throw new Exception("Документ отгрузки с таким номером уже существует");
            try
            {
                shipmentDocument.State = DocumentState.NotSigned;
                await _unitOfWork.ShipmentDocuments.AddAsync(shipmentDocument);
                await _unitOfWork.CommitAsync();
                return shipmentDocument.Id;
            }
            catch (Exception ex) 
            {
                throw new Exception($"Ошибка: {ex.Message.ToString()}");
            }
        }

        public async Task DeleteShipmentDocumentAsync(Guid id)
        {
            var document = await _unitOfWork.ShipmentDocuments.GetByIdAsync(id);
            if (document == null) return;

            _unitOfWork.ShipmentDocuments.Delete(document);
            await _unitOfWork.CommitAsync();
        }

        public async Task SignAsync(Guid documentId, ShipmentDocumentUpdateDto shipmentDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var document = await _unitOfWork.ShipmentDocuments.GetByIdAsync(documentId);
                if (document == null) throw new Exception("Документ не найден");

                if (shipmentDto != null)
                {
                    if (shipmentDto.Id != documentId) throw new ArgumentException("ID документа не совпадает");

                    document.Num = shipmentDto.Num;
                    document.Date = shipmentDto.Date;
                    document.ClientId = shipmentDto.ClientId;

                    var newResources = shipmentDto.ShipmentResources.Select(dto => new ShipmentResource
                    {
                        Id = dto.Id,
                        ResourceId = dto.ResourceId,
                        MeasureUnitId = dto.MeasureUnitId,
                        Count = dto.Count
                    }).ToList();

                    _unitOfWork.ShipmentDocuments.SyncDocumentResources(document, newResources);
                }

                await CheckResourceAvailability(document);

                var resourcesInfo = document.ShipmentResources
                    .Select(r => new ResourceChange(r.ResourceId, r.MeasureUnitId, r.Count))
                    .ToList();

                document.State = DocumentState.Signed;
                await _unitOfWork.SaveEntitiesAsync();

                await _mediator.Publish(new ShipmentDocumentSignedEvent(documentId, resourcesInfo));

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Ошибка при подписании: {ex.Message}");
            }

        }

        public async Task RevokeAsync(Guid documentId)
        {
            var document = await _unitOfWork.ShipmentDocuments.GetByIdAsync(documentId);
            if (document == null)
                throw new Exception("Документ не найден");
            
            if (document.State != DocumentState.Signed)
                throw new Exception("Только подписанные документы могут быть отозваны");
            
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                document.State = DocumentState.Revoked;

                _unitOfWork.ShipmentDocuments.Update(document);
                await _unitOfWork.SaveEntitiesAsync();

                await _mediator.Publish(new ShipmentDocumentRevokedEvent(
                    documentId,
                    document.ShipmentResources.Select(r => new ShipmentResource
                    {
                        ResourceId = r.ResourceId,
                        MeasureUnitId = r.MeasureUnitId,
                        Count = r.Count
                    }).ToList()
                ));

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Ошибка при отзыве документа: {ex.Message}");
            }
        }

        private async Task CheckResourceAvailability(ShipmentDocument document)
        {
            var resourceIds = document.ShipmentResources.Select(r => r.ResourceId).Distinct().ToList();
            var measureUnitIds = document.ShipmentResources.Select(r => r.MeasureUnitId).Distinct().ToList();

            var resources = await _unitOfWork.Resources.GetByIdsAsync(resourceIds);
            var measureUnits = await _unitOfWork.MeasureUnits.GetByIdsAsync(measureUnitIds);

            var resourceDict = resources.ToDictionary(r => r.Id);
            var unitDict = measureUnits.ToDictionary(u => u.Id);

            foreach (var resource in document.ShipmentResources)
            {
                if (!resourceDict.TryGetValue(resource.ResourceId, out var res))
                {
                    throw new Exception($"Ресурс не найден: {resource.ResourceId}");
                }

                if (!unitDict.TryGetValue(resource.MeasureUnitId, out var unit))
                {
                    throw new Exception($"Единица измерения не найдена: {resource.MeasureUnitId}");
                }

                var balance = await _unitOfWork.Balances.GetByResourceAndMeasureUnitAsync(
                    resource.ResourceId,
                    resource.MeasureUnitId
                );

                if (balance == null || balance.Count < resource.Count)
                {
                    throw new Exception(
                        $"Недостаточно ресурса '{res.Name}' ({unit.Name}): " +
                        $"доступно {balance?.Count ?? 0}, требуется {resource.Count}");
                }
            }
        }
    }
}
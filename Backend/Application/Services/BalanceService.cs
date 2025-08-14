using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System;

namespace Application.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BalanceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BalanceDto>> GetAllBalancesAsync(BalanceFilterDto filters)
        {
            var filtersObj = _mapper.Map<BalanceFilter>(filters);
            var receiptDocuments = await _unitOfWork.Balances.GetAllAsync(filtersObj);
            return _mapper.Map<IEnumerable<BalanceDto>>(receiptDocuments);
        }

        public async Task<BalanceDto> GetBalanceByIdAsync(Guid id)
        {
            var receiptDocument = await _unitOfWork.Balances.GetByIdAsync(id);
            return _mapper.Map<BalanceDto>(receiptDocument);
        }

        public async Task UpdateBalanceAsync(BalanceUpdateDto receiptDto)
        {
            var receiptDocument = _mapper.Map<Balance>(receiptDto);
            if (receiptDocument == null) return;
            _unitOfWork.Balances.Update(receiptDocument);
            await _unitOfWork.CommitAsync();
        }

        public async Task<Guid> CreateBalanceAsync(BalanceCreateDto receiptDto)
        {
            var receiptDocument = _mapper.Map<Balance>(receiptDto);
            await _unitOfWork.Balances.AddAsync(receiptDocument);
            await _unitOfWork.CommitAsync();
            return receiptDocument.Id;
        }

        public async Task<BalanceDto?> GetByResourceAndMeasureUnitAsync(Guid resourceId, Guid measureUnitId)
        {
            var balance = await _unitOfWork.Balances.GetByResourceAndMeasureUnitAsync(resourceId, measureUnitId);
            return _mapper.Map<BalanceDto?>(balance);
        }

        public async Task DeleteBalanceAsync(Guid id)
        {
            var balance = await _unitOfWork.Balances.GetByIdAsync(id);
            if (balance == null) return;
            _unitOfWork.Balances.Delete(balance);
            await _unitOfWork.CommitAsync();
        }
    }
}
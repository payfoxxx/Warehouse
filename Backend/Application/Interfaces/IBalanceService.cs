using System;
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IBalanceService
    {
        Task<IEnumerable<BalanceDto>> GetAllBalancesAsync(BalanceFilterDto filters);
        Task<BalanceDto?> GetBalanceByIdAsync(Guid id);
        Task<Guid> CreateBalanceAsync(BalanceCreateDto BalanceDto);
        Task UpdateBalanceAsync(BalanceUpdateDto BalanceDto);
        Task<BalanceDto?> GetByResourceAndMeasureUnitAsync(Guid resourceId, Guid measureUnitId);
        Task DeleteBalanceAsync(Guid id);
    }
}
using System;
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMeasureUnitService
    {
        Task<IEnumerable<MeasureUnitDto>> GetAllMeasureUnitsAsync();
        Task<IEnumerable<MeasureUnitDto>> GetAllMeasureUnitsByStateAsync(int state);
        Task<MeasureUnitDto?> GetMeasureUnitByIdAsync(Guid id);
        Task<Guid> CreateMeasureUnitAsync(MeasureUnitCreateDto MeasureUnitDto);
        Task UpdateMeasureUnitAsync(MeasureUnitUpdateDto MeasureUnitDto);
        Task DeleteMeasureUnitAsync(Guid id);
    }
}
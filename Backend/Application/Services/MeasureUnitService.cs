using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System;

namespace Application.Services
{
    public class MeasureUnitService : IMeasureUnitService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MeasureUnitService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MeasureUnitDto>> GetAllMeasureUnitsAsync()
        {
            var measureUnits = await _unitOfWork.MeasureUnits.GetAllAsync();
            return _mapper.Map<IEnumerable<MeasureUnitDto>>(measureUnits);
        }

        public async Task<IEnumerable<MeasureUnitDto>> GetAllMeasureUnitsByStateAsync(int state)
        {
            var measureUnits = await _unitOfWork.MeasureUnits.GetAllByStateAsync((State)state);
            return _mapper.Map<IEnumerable<MeasureUnitDto>>(measureUnits);
        }

        public async Task<MeasureUnitDto?> GetMeasureUnitByIdAsync(Guid id)
        {
            var measureUnit = await _unitOfWork.MeasureUnits.GetByIdAsync(id);
            return _mapper.Map<MeasureUnitDto>(measureUnit);
        }

        public async Task<Guid> CreateMeasureUnitAsync(MeasureUnitCreateDto measureUnitDto)
        {
            var measureUnit = _mapper.Map<MeasureUnit>(measureUnitDto);
            if (!await _unitOfWork.MeasureUnits.ExistsByNameAsync(measureUnit.Name))
            {
                measureUnit.State = State.Active;
                await _unitOfWork.MeasureUnits.AddAsync(measureUnit);
                await _unitOfWork.CommitAsync();
                return measureUnit.Id;
            }
            else
                throw new Exception("Имя уже существует");
        }

        public async Task UpdateMeasureUnitAsync(MeasureUnitUpdateDto measureUnitDto)
        {
            var measureUnit = _mapper.Map<MeasureUnit>(measureUnitDto);
            if (measureUnit == null) return;
            if (string.IsNullOrWhiteSpace(measureUnit.Name))
                throw new Exception("Имя не должно быть пустым");
            if (await _unitOfWork.MeasureUnits.ExistsByNameAndIdAsync(measureUnit.Name, measureUnit.Id))
                throw new Exception("Такое имя уже существует");
            _unitOfWork.MeasureUnits.Update(measureUnit);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteMeasureUnitAsync(Guid id)
        {
            var measureUnit = await _unitOfWork.MeasureUnits.GetByIdAsync(id);
            if (measureUnit == null) return;
            if (await _unitOfWork.MeasureUnits.IsMeasureUnitUsedAsync(id))
                throw new Exception("Единица измерения используется, ее нельзя удалить");
            _unitOfWork.MeasureUnits.Delete(measureUnit);
            await _unitOfWork.CommitAsync();
        }
    }
}
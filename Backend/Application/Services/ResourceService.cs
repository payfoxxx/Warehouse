using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System;

namespace Application.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ResourceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResourceDto>> GetAllResourcesAsync()
        {
            var resources = await _unitOfWork.Resources.GetAllAsync();
            return _mapper.Map<IEnumerable<ResourceDto>>(resources);
        }

        public async Task<IEnumerable<ResourceDto>> GetAllResourcesByStateAsync(int state)
        {
            var resources = await _unitOfWork.Resources.GetAllByStateAsync((State)state);
            return _mapper.Map<IEnumerable<ResourceDto>>(resources);
        }

        public async Task<ResourceDto?> GetResourceByIdAsync(Guid id)
        {
            var resource = await _unitOfWork.Resources.GetByIdAsync(id);
            return _mapper.Map<ResourceDto>(resource);
        }

        public async Task<Guid> CreateResourceAsync(ResourceCreateDto resourceDto)
        {
            var resource = _mapper.Map<Resource>(resourceDto);
            if (!await _unitOfWork.Resources.ExistsByNameAsync(resource.Name))
            {
                resource.State = State.Active;
                await _unitOfWork.Resources.AddAsync(resource);
                await _unitOfWork.CommitAsync();
                return resource.Id;
            }
            else
                throw new Exception("Ресурс с таким именем уже существует");
        }

        public async Task UpdateResourceAsync(ResourceUpdateDto resourceDto)
        {
            var resource = _mapper.Map<Resource>(resourceDto);
            if (resource == null) return;
            if (string.IsNullOrWhiteSpace(resource.Name))
                throw new Exception("Имя не должно быть пустым");
            if (await _unitOfWork.Resources.ExistsByNameAndIdAsync(resource.Name, resource.Id))
                throw new Exception("Такое имя уже существует");

            _unitOfWork.Resources.Update(resource);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteResourceAsync(Guid id)
        {
            var resource = await _unitOfWork.Resources.GetByIdAsync(id);
            if (resource == null) return;
            if (await _unitOfWork.Resources.IsResourceUsedAsync(id))
                throw new Exception("Ресурс используется, его нельзя удалить");
            _unitOfWork.Resources.Delete(resource);
            await _unitOfWork.CommitAsync();
        }
    }
}
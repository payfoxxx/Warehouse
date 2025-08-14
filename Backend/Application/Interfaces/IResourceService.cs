using System;
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IResourceService
    {
        Task<IEnumerable<ResourceDto>> GetAllResourcesAsync();
        Task<IEnumerable<ResourceDto>> GetAllResourcesByStateAsync(int state);
        Task<ResourceDto?> GetResourceByIdAsync(Guid id);
        Task<Guid> CreateResourceAsync(ResourceCreateDto ResourceDto);
        Task UpdateResourceAsync(ResourceUpdateDto ResourceDto);
        Task DeleteResourceAsync(Guid id);
    }
}
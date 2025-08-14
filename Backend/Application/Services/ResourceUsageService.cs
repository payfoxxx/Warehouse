using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ResourceUsageService : IResourceUsageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResourceUsageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsResourceUsedAsync(Guid resourceId, Guid measureUnitId)
        {
            return await _unitOfWork.ResourceUsage.IsResourceUsedAsync(resourceId, measureUnitId);
        }
    }
}
namespace Domain.Interfaces
{
    public interface IResourceUsageRepository
    {
        Task<bool> IsResourceUsedAsync(Guid resourceId, Guid measureUnitId);
    }
}
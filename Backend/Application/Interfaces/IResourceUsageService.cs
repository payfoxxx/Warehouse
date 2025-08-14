namespace Application.Interfaces
{
    public interface IResourceUsageService
    {
        Task<bool> IsResourceUsedAsync(Guid resourceId, Guid measureUnitId);
    }
}
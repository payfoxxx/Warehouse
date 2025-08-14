using System;
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IShipmentService
    {
        Task<IEnumerable<ShipmentDocumentDto>> GetAllShipmentDocumentsAsync(ShipmentFilterDto filters);
        Task<ShipmentDocumentDto> GetShipmentDocumentByIdAsync(Guid id);
        Task<Guid> CreateShipmentDocumentAsync(ShipmentDocumentCreateDto shipmentDto);
        Task UpdateShipmentDocumentAsync(ShipmentDocumentUpdateDto shipmentDto);
        Task DeleteShipmentDocumentAsync(Guid id);
        Task SignAsync(Guid documentId, ShipmentDocumentUpdateDto shipmentDto);
        Task RevokeAsync(Guid documentId);
    }
}
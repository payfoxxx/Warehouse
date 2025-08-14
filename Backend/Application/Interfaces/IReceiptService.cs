using System;
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IReceiptService
    {
        Task<IEnumerable<ReceiptDocumentDto>> GetAllReceiptDocumentsAsync(ReceiptFilterDto filters);
        Task<ReceiptDocumentDto> GetReceiptDocumentByIdAsync(Guid id);
        Task<Guid> CreateReceiptDocumentAsync(ReceiptDocumentCreateDto receiptDto);
        Task UpdateReceiptDocumentAsync(ReceiptDocumentUpdateDto receiptDto);
        Task DeleteReceiptDocumentAsync(Guid id);
    }
}
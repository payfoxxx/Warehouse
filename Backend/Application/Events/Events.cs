using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Events
{
    public record ReceiptDocumentCreatedEvent(Guid DocumentId, List<ReceiptResource> Resources) : INotification;
    public record ReceiptDocumentUpdatedEvent(Guid DocumentId, List<ReceiptResource> OriginalResources, List<ReceiptResource> UpdatedResources) : INotification;
    public record ReceiptDocumentDeletedEvent(Guid DocumentId, List<ReceiptResource> Resources) : INotification;

    public record ShipmentDocumentSignedEvent(Guid DocumentId, List<ResourceChange> Resources) : INotification;
    public record ShipmentDocumentRevokedEvent(Guid DocumentId, List<ShipmentResource> Resources) : INotification;

    public record CheckBalanceBeforeOperationEvent(List<ResourceOperation> Operations) : INotification;
    public record UpdateBalanceEvent(List<ResourceOperation> Operations) : INotification;
    public record RemoveZeroBalancesEvent(List<ResourceOperation> Operations) : INotification;

    public record ResourceOperation(
        Guid ResourceId,
        Guid MeasureUnitId,
        int Count,
        OperationType Type
    );

    public record ResourceChange(
        Guid ResourceId,
        Guid MeasureUnitId,
        int Count
    );

    public enum OperationType
    {
        Increase,
        Decrease
    }
}

using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.DTOs;

namespace Application.Events.Handlers
{
    public class BalanceEventHandler :
        INotificationHandler<ReceiptDocumentCreatedEvent>,
        INotificationHandler<ReceiptDocumentUpdatedEvent>,
        INotificationHandler<ReceiptDocumentDeletedEvent>,
        INotificationHandler<ShipmentDocumentSignedEvent>,
        INotificationHandler<ShipmentDocumentRevokedEvent>,
        INotificationHandler<CheckBalanceBeforeOperationEvent>,
        INotificationHandler<UpdateBalanceEvent>,
        INotificationHandler<RemoveZeroBalancesEvent>
    {
        private readonly IBalanceService _balanceService;
        private readonly IResourceUsageService _resourceUsageService;

        public BalanceEventHandler(IBalanceService balanceService, IResourceUsageService resourceUsageService)
        {
            _balanceService = balanceService;
            _resourceUsageService = resourceUsageService;
        }

        public async Task Handle(ReceiptDocumentCreatedEvent notification, CancellationToken ct)
        {
            var operations = notification.Resources.Select(r => 
            new ResourceOperation(
                r.ResourceId,
                r.MeasureUnitId,
                r.Count,
                OperationType.Increase
            )).ToList();
            await ApplyBalanceOperations(operations, ct);
        }

        public async Task Handle(ReceiptDocumentUpdatedEvent notification, CancellationToken ct)
        {
            var changes = new List<ResourceChange>();

            foreach (var oldResource in notification.OriginalResources)
            {
                var newResource = notification.UpdatedResources
                    .FirstOrDefault(r => r.Id == oldResource.Id);

                if (newResource == null)
                {
                    changes.Add(new ResourceChange(
                        oldResource.ResourceId,
                        oldResource.MeasureUnitId,
                        -oldResource.Count
                    ));
                }
                else if (newResource.ResourceId != oldResource.ResourceId ||
                         newResource.MeasureUnitId != oldResource.MeasureUnitId)
                {
                    changes.Add(new ResourceChange(
                        oldResource.ResourceId,
                        oldResource.MeasureUnitId,
                        -oldResource.Count
                    ));
        
                    changes.Add(new ResourceChange(
                        newResource.ResourceId,
                        newResource.MeasureUnitId,
                        newResource.Count
                    ));
                }
                else if (oldResource.Count != newResource.Count)
                {
                    changes.Add(new ResourceChange(
                        oldResource.ResourceId,
                        oldResource.MeasureUnitId,
                        newResource.Count - oldResource.Count
                    ));
                }
            }

            foreach (var newResource in notification.UpdatedResources)
            {
                if (!notification.OriginalResources.Any(r => r.Id == newResource.Id))
                {
                    changes.Add(new ResourceChange(
                        newResource.ResourceId,
                        newResource.MeasureUnitId,
                        newResource.Count
                    ));
                }
            }

            await ApplyBalanceChanges(changes, ct);
            await RemoveZeroBalancesForChanges(changes, ct);
        }

        public async Task Handle(ReceiptDocumentDeletedEvent notification, CancellationToken ct)
        {
            var operations = notification.Resources.Select(r => 
            new ResourceOperation(
                r.ResourceId,
                r.MeasureUnitId,
                r.Count,
                OperationType.Decrease
            )).ToList();
            await ApplyBalanceOperations(operations, ct);
        }

        public async Task Handle(ShipmentDocumentSignedEvent notification, CancellationToken ct)
        {
            var operations = notification.Resources.Select(r => 
            new ResourceOperation(
                r.ResourceId,
                r.MeasureUnitId,
                r.Count,
                OperationType.Decrease
            )).ToList();
            await ApplyBalanceOperations(operations, ct);
        }

        public async Task Handle(ShipmentDocumentRevokedEvent notification, CancellationToken ct)
        {
            var operations = notification.Resources.Select(r => 
            new ResourceOperation(
                r.ResourceId,
                r.MeasureUnitId,
                r.Count,
                OperationType.Increase
            )).ToList();
            await ApplyBalanceOperations(operations, ct); 
            
        }

        public async Task Handle(CheckBalanceBeforeOperationEvent notification, CancellationToken ct)
        {
            foreach (var operation in notification.Operations)
            {
                if (operation.Type != OperationType.Decrease) continue;

                var balance = await _balanceService.GetByResourceAndMeasureUnitAsync(
                    operation.ResourceId,
                    operation.MeasureUnitId
                );

                if (balance == null || balance.Count < operation.Count)
                    throw new Exception("Будет неправильный баланс");
            }
        }

        public async Task Handle(RemoveZeroBalancesEvent notification, CancellationToken ct)
        {
            foreach (var operation in notification.Operations)
            {
                var balance = await _balanceService.GetByResourceAndMeasureUnitAsync(
                    operation.ResourceId,
                    operation.MeasureUnitId
                );

                if (balance != null && balance.Count == 0)
                {
                    bool isResourceUsed = await _resourceUsageService.IsResourceUsedAsync(
                        operation.ResourceId,
                        operation.MeasureUnitId
                    );

                    if (!isResourceUsed)
                    {
                        await _balanceService.DeleteBalanceAsync(balance.Id);
                    }
                }
            }
        }

        public async Task Handle(UpdateBalanceEvent notification, CancellationToken ct)
        {
            await ApplyBalanceOperations(notification.Operations, ct);
        }

        private async Task ApplyBalanceOperations(List<ResourceOperation> operations, CancellationToken ct)
        {
            var groupedOperations = operations
                .GroupBy(o => new { o.ResourceId, o.MeasureUnitId })
                .Select(g => new {
                    Key = g.Key,
                    Count = g.Sum(o => o.Type == OperationType.Increase ? o.Count : -o.Count)
                });

            foreach (var operation in groupedOperations)
            {
                var balance = await _balanceService.GetByResourceAndMeasureUnitAsync(
                    operation.Key.ResourceId,
                    operation.Key.MeasureUnitId
                );

                if (balance == null)
                {
                    if (operation.Count < 0)
                        throw new Exception("Количество меньше нуля");
                    
                    BalanceCreateDto balanceCreate = new BalanceCreateDto
                    {
                        ResourceId = operation.Key.ResourceId,
                        MeasureUnitId = operation.Key.MeasureUnitId,
                        Count = operation.Count
                    };

                    await _balanceService.CreateBalanceAsync(balanceCreate);
                }
                else
                {
                    var balanceCount = balance.Count + operation.Count;
                    if (balanceCount < 0)
                        throw new Exception("Возникает отрицательный баланс");
                    
                    BalanceUpdateDto balanceUpdate = new BalanceUpdateDto
                    {
                        Id = balance.Id,
                        ResourceId = balance.ResourceId,
                        MeasureUnitId = balance.MeasureUnitId,
                        Count = balanceCount
                    };

                    await _balanceService.UpdateBalanceAsync(balanceUpdate);
                }
            }
        }

        private async Task ApplyBalanceChanges(List<ResourceChange> changes, CancellationToken ct)
        {
            foreach (var change in changes
            .GroupBy(c => new { c.ResourceId, c.MeasureUnitId })
            .Select(g => new {
                Key = g.Key,
                TotalCount = g.Sum(c => c.Count)
            }))
            {
                var balance = await _balanceService.GetByResourceAndMeasureUnitAsync(
                    change.Key.ResourceId,
                    change.Key.MeasureUnitId
                );

                if (balance == null)
                {
                    if (change.TotalCount < 0)
                        throw new InvalidOperationException("Возникает отрицательный баланс");

                    await _balanceService.CreateBalanceAsync(new BalanceCreateDto
                    {
                        ResourceId = change.Key.ResourceId,
                        MeasureUnitId = change.Key.MeasureUnitId,
                        Count = change.TotalCount
                    });
                }
                else
                {
                    int newCount = balance.Count + change.TotalCount;
                    if (newCount < 0)
                        throw new InvalidOperationException("Возникает отрицательный баланс");

                    await _balanceService.UpdateBalanceAsync(new BalanceUpdateDto
                    {
                        Id = balance.Id,
                        ResourceId = balance.ResourceId,
                        MeasureUnitId = balance.MeasureUnitId,
                        Count = newCount
                    });
                }
            }
        }

        private async Task RemoveZeroBalancesForChanges(List<ResourceChange> changes, CancellationToken ct)
        {
            var uniquePairs = changes
                .Select(c => new { c.ResourceId, c.MeasureUnitId })
                .Distinct()
                .ToList();

            foreach (var pair in uniquePairs)
            {
                var balance = await _balanceService.GetByResourceAndMeasureUnitAsync(
                    pair.ResourceId,
                    pair.MeasureUnitId
                );

                if (balance != null && balance.Count == 0)
                {
                    bool isUsed = await _resourceUsageService.IsResourceUsedAsync(
                        pair.ResourceId,
                        pair.MeasureUnitId
                    );

                    if (!isUsed)
                    {
                        await _balanceService.DeleteBalanceAsync(balance.Id);
                    }
                }
            }
        }
    }
}

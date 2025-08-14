using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Application.Events.Handlers;
using Application.Events;
using Domain.Interfaces;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IMeasureUnitService, MeasureUnitService>();
            services.AddScoped<IReceiptService, ReceiptService>();
            services.AddScoped<IBalanceService, BalanceService>();
            services.AddScoped<IShipmentService, ShipmentService>();
            services.AddScoped<IResourceUsageService, ResourceUsageService>();

            services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssemblies(
                    typeof(ReceiptDocumentCreatedEvent).Assembly,
                    typeof(ReceiptDocumentUpdatedEvent).Assembly,
                    typeof(ReceiptDocumentDeletedEvent).Assembly,
                    typeof(ShipmentDocumentSignedEvent).Assembly,
                    typeof(ShipmentDocumentRevokedEvent).Assembly,
                    typeof(CheckBalanceBeforeOperationEvent).Assembly,
                    typeof(RemoveZeroBalancesEvent).Assembly,
                    typeof(UpdateBalanceEvent).Assembly,
                    typeof(BalanceEventHandler).Assembly
                ));

            return services;
        }
    }
}

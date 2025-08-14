using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Mapping Client
            CreateMap<ClientCreateDto, Client>();
            CreateMap<ClientUpdateDto, Client>();
            CreateMap<Client, ClientDto>()
                .ForMember(dto => dto.StateName, opt => opt.MapFrom(src => StateConverter.ToRussian(src.State)))
                .ForMember(dto => dto.State, opt => opt.MapFrom(src => (int)src.State));
            
            // Mapping MeasureUnit
            CreateMap<MeasureUnitCreateDto, MeasureUnit>();
            CreateMap<MeasureUnitUpdateDto, MeasureUnit>();
            CreateMap<MeasureUnit, MeasureUnitDto>()
                .ForMember(dto => dto.StateName, opt => opt.MapFrom(src => StateConverter.ToRussian(src.State)))
                .ForMember(dto => dto.State, opt => opt.MapFrom(src => (int)src.State));
            
            // Mapping Resource
            CreateMap<ResourceCreateDto, Resource>();
            CreateMap<ResourceUpdateDto, Resource>();
            CreateMap<Resource, ResourceDto>()
                .ForMember(dto => dto.StateName, opt => opt.MapFrom(src => StateConverter.ToRussian(src.State)))
                .ForMember(dto => dto.State, opt => opt.MapFrom(src => (int)src.State));
            
            // Mapping Balance
            CreateMap<Balance, BalanceDto>()
                .ForMember(dto => dto.ResourceName, opt => opt.MapFrom(src => src.Resource.Name))
                .ForMember(dto => dto.MeasureUnitName, opt => opt.MapFrom(src => src.MeasureUnit.Name));
            CreateMap<BalanceCreateDto, Balance>();
            CreateMap<BalanceUpdateDto, Balance>();
            CreateMap<BalanceFilterDto, BalanceFilter>();

            // Mapping Receipt
            CreateMap<ReceiptDocumentCreateDto, ReceiptDocument>();
            CreateMap<ReceiptResourceCreateDto, ReceiptResource>();

            CreateMap<ReceiptDocumentUpdateDto, ReceiptDocument>();
            CreateMap<ReceiptResourceUpdateDto, ReceiptResource>();

            CreateMap<ReceiptDocument, ReceiptDocumentDto>();
            CreateMap<ReceiptResource, ReceiptResourceDto>()
                .ForMember(dto => dto.ResourceName, opt => opt.MapFrom(src => src.Resource.Name))
                .ForMember(dto => dto.MeasureUnitName, opt => opt.MapFrom(src => src.MeasureUnit.Name));
            
            // Mapping Shipping
            CreateMap<ShipmentDocumentCreateDto, ShipmentDocument>();
            CreateMap<ShipmentResourceCreateDto, ShipmentResource>();

            CreateMap<ShipmentDocumentUpdateDto, ShipmentDocument>()
                .ForMember(obj => obj.State, dto => dto.MapFrom(src => src.StatusId));
            CreateMap<ShipmentResourceUpdateDto, ShipmentResource>();

            CreateMap<ShipmentDocument, ShipmentDocumentDto>()
                .ForMember(dto => dto.ClientName, opt => opt.MapFrom(src => src.Client.Name))
                .ForMember(dto => dto.StatusName, opt => opt.MapFrom(src => StateConverter.ToRussian(src.State)))
                .ForMember(dto => dto.StatusId, opt => opt.MapFrom(src => (int)src.State));
            CreateMap<ShipmentResource, ShipmentResourceDto>()
                .ForMember(dto => dto.ResourceName, opt => opt.MapFrom(src => src.Resource.Name))
                .ForMember(dto => dto.MeasureUnitName, opt => opt.MapFrom(src => src.MeasureUnit.Name));
            
            // Filter
            CreateMap<ReceiptFilterDto, ReceiptFilter>();
            CreateMap<ShipmentFilterDto, ShipmentFilter>();
        }
    }

    public static class Test 
    {
        public static DateTime? Convert(string? t)
        {
            if (string.IsNullOrWhiteSpace(t))
                return null;
            else
                return DateTime.Parse(t);
        }
    }

    public static class StateConverter
    {
        public static string ToRussian(State state)
        {
            switch (state)
            {
                case State.Active:
                    return "Активный";
                case State.Archived:
                    return "В архиве";
                default:
                    return "Не определен";
            }
        }

        public static string ToRussian(DocumentState state)
        {
            switch (state)
            {
                case DocumentState.NotSigned:
                    return "Не подписан";
                case DocumentState.Signed:
                    return "Подписан";
                case DocumentState.Revoked:
                    return "Отозван";
                default:
                    return "Не определен";
            }
        }

        public static State ToState(string word)
        {
            switch (word)
            {
                case "Активный":
                    return State.Active;
                case "В архиве":
                    return State.Archived;
                default:
                    return State.Active;
            }
        }

        public static DocumentState ToDocumentState(string word)
        {
            switch (word)
            {
                case "Не подписан":
                    return DocumentState.NotSigned;
                case "Подписан":
                    return DocumentState.Signed;
                case "Отозван":
                    return DocumentState.Revoked;
                default:
                    return DocumentState.NotSigned;
            }
        }

        public static DocumentState ToDocumentState(int status)
        {
            switch (status)
            {
                case 1:
                    return DocumentState.Signed;
                case 2:
                    return DocumentState.NotSigned;
                case 3:
                    return DocumentState.Revoked;
                default:
                    return DocumentState.NotSigned;
            }
        }
    }
}
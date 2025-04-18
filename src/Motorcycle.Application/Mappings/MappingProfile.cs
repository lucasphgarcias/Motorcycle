using AutoMapper;
using Motorcycle.Application.DTOs.DeliveryPerson;
using Motorcycle.Application.DTOs.Motorcycle;
using Motorcycle.Application.DTOs.Rental;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Enums;
using System;

namespace Motorcycle.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Configuração de mapeamento global para DateTime -> DateOnly
        CreateMap<DateTime, DateOnly>()
            .ConvertUsing(src => DateOnly.FromDateTime(src));
            
        // Configuração de mapeamento global para DateOnly -> DateTime
        CreateMap<DateOnly, DateTime>()
            .ConvertUsing(src => DateTime.SpecifyKind(new DateTime(src.Year, src.Month, src.Day), DateTimeKind.Utc));

        // Motorcycle mappings
        CreateMap<MotorcycleEntity, MotorcycleDto>()
            .ForMember(dest => dest.LicensePlate, opt => opt.MapFrom(src => src.LicensePlate.Value));
            
        CreateMap<CreateMotorcycleDto, MotorcycleEntity>()
            .ConstructUsing(src => MotorcycleEntity.Create(src.Model, src.Year, src.LicensePlate));

        // DeliveryPerson mappings
        CreateMap<DeliveryPersonEntity, DeliveryPersonDto>()
            .ForMember(dest => dest.Cnpj, opt => opt.MapFrom(src => src.Cnpj.Value))
            .ForMember(dest => dest.LicenseNumber, opt => opt.MapFrom(src => src.DriverLicense.Number))
            .ForMember(dest => dest.LicenseType, opt => opt.MapFrom(src => src.DriverLicense.Type))
            .ForMember(dest => dest.LicenseImagePath, opt => opt.MapFrom(src => src.DriverLicense.ImagePath));
            
        CreateMap<CreateDeliveryPersonDto, DeliveryPersonEntity>()
            .ConstructUsing(src => DeliveryPersonEntity.Create(
                src.Name, 
                src.Cnpj, 
                DateOnly.FromDateTime(src.BirthDate), 
                src.LicenseNumber, 
                src.LicenseType));

        // Rental mappings
        CreateMap<RentalEntity, RentalDto>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Period.StartDate))
            .ForMember(dest => dest.ExpectedEndDate, opt => opt.MapFrom(src => src.Period.ExpectedEndDate))
            .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(src => src.Period.ActualEndDate))
            .ForMember(dest => dest.PlanType, opt => opt.MapFrom(src => src.Period.PlanType))
            .ForMember(dest => dest.DailyRate, opt => opt.MapFrom(src => src.DailyRate.Amount))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount != null ? src.TotalAmount.Amount : (decimal?)null))
            .ForMember(dest => dest.MotorcycleModel, opt => opt.MapFrom(src => src.Motorcycle.Model))
            .ForMember(dest => dest.MotorcycleLicensePlate, opt => opt.MapFrom(src => src.Motorcycle.LicensePlate.Value))
            .ForMember(dest => dest.DeliveryPersonName, opt => opt.MapFrom(src => src.DeliveryPerson.Name));
    }
}
using Microsoft.AspNetCore.Http;
using FluentValidation;
using Motorcycle.Application.DTOs.DeliveryPerson;
using Motorcycle.Domain.Interfaces.Services;

namespace Motorcycle.Application.Validators;

public class UpdateDriverLicenseImageDtoValidator : AbstractValidator<UpdateDriverLicenseImageDto>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB

    public UpdateDriverLicenseImageDtoValidator(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;

        RuleFor(x => x.LicenseImage)
            .NotNull().WithMessage("A imagem da CNH é obrigatória.")
            .Must(HaveValidSize).WithMessage($"O tamanho máximo do arquivo é de {_maxFileSize / 1024 / 1024}MB.")
            .Must(HaveValidFileExtension).WithMessage("O formato do arquivo deve ser PNG ou BMP.");
    }

    private bool HaveValidSize(IFormFile file)
    {
        return file != null && file.Length > 0 && file.Length <= _maxFileSize;
    }

    private bool HaveValidFileExtension(IFormFile file)
    {
        if (file == null)
            return false;

        return _fileStorageService.IsValidImageType(file.ContentType);
    }
}
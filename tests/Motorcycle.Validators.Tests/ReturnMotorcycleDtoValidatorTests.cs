using FluentValidation.TestHelper;
using Motorcycle.Application.DTOs.Rental;
using Motorcycle.Application.Validators;
using Xunit;

namespace Motorcycle.Validators.Tests;

public class ReturnMotorcycleDtoValidatorTests
{
    private readonly ReturnMotorcycleDtoValidator _validator;

    public ReturnMotorcycleDtoValidatorTests()
    {
        _validator = new ReturnMotorcycleDtoValidator();
    }

    [Fact]
    public void ShouldHaveErrorWhenReturnDateIsEmpty()
    {
        // Arrange
        var dto = new ReturnMotorcycleDto
        {
            ReturnDate = default
        };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ReturnDate)
            .WithErrorMessage("A data de devolução é obrigatória.");
    }

    [Fact]
    public void ShouldHaveErrorWhenReturnDateIsInFuture()
    {
        // Arrange
        var dto = new ReturnMotorcycleDto
        {
            ReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
        };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ReturnDate)
            .WithErrorMessage("A data de devolução não pode ser no futuro.");
    }

    [Fact]
    public void ShouldNotHaveErrorWithValidReturnDate()
    {
        // Arrange
        var dto = new ReturnMotorcycleDto
        {
            ReturnDate = DateOnly.FromDateTime(DateTime.Today)
        };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.ReturnDate);
    }

    [Fact]
    public void ShouldNotHaveErrorWithPastReturnDate()
    {
        // Arrange
        var dto = new ReturnMotorcycleDto
        {
            ReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1))
        };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.ReturnDate);
    }
} 
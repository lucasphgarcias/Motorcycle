using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Motorcycle.Application.Behaviors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Motorcycle.Application.Tests.Behaviors;

public class ValidationBehaviorTests
{
    private readonly Mock<ILogger<ValidationBehavior<TestRequest, TestResponse>>> _mockLogger;

    public ValidationBehaviorTests()
    {
        _mockLogger = new Mock<ILogger<ValidationBehavior<TestRequest, TestResponse>>>();
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldProceedWithRequest()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>>
        {
            CreatePassingValidator()
        };

        var mockNextBehavior = new Mock<RequestHandlerDelegate<TestResponse>>();
        mockNextBehavior.Setup(x => x()).ReturnsAsync(new TestResponse());

        var behavior = new ValidationBehavior<TestRequest, TestResponse>(_mockLogger.Object, validators);

        // Act
        var result = await behavior.Handle(new TestRequest(), CancellationToken.None, mockNextBehavior.Object);

        // Assert
        result.Should().NotBeNull();
        mockNextBehavior.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>>
        {
            CreateFailingValidator()
        };

        var mockNextBehavior = new Mock<RequestHandlerDelegate<TestResponse>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(_mockLogger.Object, validators);

        // Act
        var act = () => behavior.Handle(new TestRequest(), CancellationToken.None, mockNextBehavior.Object);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        mockNextBehavior.Verify(x => x(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNoValidators_ShouldProceedWithRequest()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>>();
        
        var mockNextBehavior = new Mock<RequestHandlerDelegate<TestResponse>>();
        mockNextBehavior.Setup(x => x()).ReturnsAsync(new TestResponse());

        var behavior = new ValidationBehavior<TestRequest, TestResponse>(_mockLogger.Object, validators);

        // Act
        var result = await behavior.Handle(new TestRequest(), CancellationToken.None, mockNextBehavior.Object);

        // Assert
        result.Should().NotBeNull();
        mockNextBehavior.Verify(x => x(), Times.Once);
    }

    private static IValidator<TestRequest> CreatePassingValidator()
    {
        var mockValidator = new Mock<IValidator<TestRequest>>();
        mockValidator.Setup(x => x.ValidateAsync(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        return mockValidator.Object;
    }

    private static IValidator<TestRequest> CreateFailingValidator()
    {
        var mockValidator = new Mock<IValidator<TestRequest>>();
        var validationFailures = new List<ValidationFailure>
        {
            new("Property", "Error message")
        };

        mockValidator.Setup(x => x.ValidateAsync(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        return mockValidator.Object;
    }
}

public class TestRequest : IRequest<TestResponse>
{
}

public class TestResponse
{
} 
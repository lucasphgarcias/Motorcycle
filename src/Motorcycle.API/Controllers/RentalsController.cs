using Microsoft.AspNetCore.Mvc;
using Motorcycle.API.Models.Responses;
using Motorcycle.Application.DTOs.Rental;
using Motorcycle.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Motorcycle.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;
    private readonly ILogger<RentalsController> _logger;

    public RentalsController(
        IRentalService rentalService,
        ILogger<RentalsController> logger)
    {
        _rentalService = rentalService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todos os aluguéis cadastrados
    /// </summary>
    /// <returns>Lista de aluguéis</returns>
    [HttpGet]
    [SwaggerResponse(200, "Lista de aluguéis retornada com sucesso", typeof(ApiResponse<IEnumerable<RentalDto>>))]
    public async Task<IActionResult> GetAll()
    {
        var rentals = await _rentalService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<RentalDto>>.Ok(rentals, "Aluguéis retornados com sucesso"));
    }

    /// <summary>
    /// Obtém um aluguel pelo ID
    /// </summary>
    /// <param name="id">ID do aluguel</param>
    /// <returns>Dados do aluguel</returns>
    [HttpGet("{id}")]
    [SwaggerResponse(200, "Aluguel encontrado com sucesso", typeof(ApiResponse<RentalDto>))]
    [SwaggerResponse(404, "Aluguel não encontrado", typeof(ApiResponse))]
    public async Task<IActionResult> GetById(Guid id)
    {
        var rental = await _rentalService.GetByIdAsync(id);
        
        if (rental == null)
            return NotFound(ApiResponse.Fail("Aluguel não encontrado"));

        return Ok(ApiResponse<RentalDto>.Ok(rental, "Aluguel encontrado com sucesso"));
    }

    /// <summary>
    /// Cadastra um novo aluguel
    /// </summary>
    /// <param name="createDto">Dados do aluguel</param>
    /// <returns>Aluguel cadastrado</returns>
    [HttpPost]
    [SwaggerResponse(201, "Aluguel criado com sucesso", typeof(ApiResponse<RentalDto>))]
    [SwaggerResponse(400, "Dados inválidos", typeof(ApiResponse))]
    public async Task<IActionResult> Create([FromBody] CreateRentalDto createDto)
    {
        var createdRental = await _rentalService.CreateAsync(createDto);
        
        return CreatedAtAction(
            nameof(GetById), 
            new { id = createdRental.Id }, 
            ApiResponse<RentalDto>.Ok(createdRental, "Aluguel criado com sucesso"));
    }

    /// <summary>
    /// Registra a devolução de uma moto
    /// </summary>
    /// <param name="id">ID do aluguel</param>
    /// <param name="returnDto">Data de devolução</param>
    /// <returns>Detalhes do valor total</returns>
    [HttpPut("{id}/return")]
    [SwaggerResponse(200, "Devolução registrada com sucesso", typeof(ApiResponse<RentalTotalAmountDto>))]
    [SwaggerResponse(400, "Dados inválidos", typeof(ApiResponse))]
    [SwaggerResponse(404, "Aluguel não encontrado", typeof(ApiResponse))]
    public async Task<IActionResult> ReturnMotorcycle(Guid id, [FromBody] ReturnMotorcycleDto returnDto)
    {
        var rental = await _rentalService.GetByIdAsync(id);
        
        if (rental == null)
            return NotFound(ApiResponse.Fail("Aluguel não encontrado"));

        var totalAmount = await _rentalService.ReturnMotorcycleAsync(id, returnDto);
        
        return Ok(ApiResponse<RentalTotalAmountDto>.Ok(totalAmount, "Devolução registrada com sucesso"));
    }

    /// <summary>
    /// Calcula o valor total do aluguel para uma data de devolução específica
    /// </summary>
    /// <param name="id">ID do aluguel</param>
    /// <param name="returnDate">Data de devolução</param>
    /// <returns>Detalhes do valor total</returns>
    [HttpGet("{id}/calculate")]
    [SwaggerResponse(200, "Valor calculado com sucesso", typeof(ApiResponse<RentalTotalAmountDto>))]
    [SwaggerResponse(400, "Dados inválidos", typeof(ApiResponse))]
    [SwaggerResponse(404, "Aluguel não encontrado", typeof(ApiResponse))]
    public async Task<IActionResult> CalculateTotalAmount(Guid id, [FromQuery] DateOnly returnDate)
    {
        var rental = await _rentalService.GetByIdAsync(id);
        
        if (rental == null)
            return NotFound(ApiResponse.Fail("Aluguel não encontrado"));

        var totalAmount = await _rentalService.CalculateTotalAmountAsync(id, returnDate);
        
        return Ok(ApiResponse<RentalTotalAmountDto>.Ok(totalAmount, "Valor calculado com sucesso"));
    }

    /// <summary>
    /// Obtém os aluguéis de uma moto específica
    /// </summary>
    /// <param name="motorcycleId">ID da moto</param>
    /// <returns>Lista de aluguéis</returns>
    [HttpGet("by-motorcycle/{motorcycleId}")]
    [SwaggerResponse(200, "Aluguéis encontrados com sucesso", typeof(ApiResponse<IEnumerable<RentalDto>>))]
    public async Task<IActionResult> GetByMotorcycleId(Guid motorcycleId)
    {
        var rentals = await _rentalService.GetByMotorcycleIdAsync(motorcycleId);
        return Ok(ApiResponse<IEnumerable<RentalDto>>.Ok(rentals, "Aluguéis encontrados com sucesso"));
    }

    /// <summary>
    /// Obtém os aluguéis de um entregador específico
    /// </summary>
    /// <param name="deliveryPersonId">ID do entregador</param>
    /// <returns>Lista de aluguéis</returns>
    [HttpGet("by-delivery-person/{deliveryPersonId}")]
    [SwaggerResponse(200, "Aluguéis encontrados com sucesso", typeof(ApiResponse<IEnumerable<RentalDto>>))]
    public async Task<IActionResult> GetByDeliveryPersonId(Guid deliveryPersonId)
    {
        var rentals = await _rentalService.GetByDeliveryPersonIdAsync(deliveryPersonId);
        return Ok(ApiResponse<IEnumerable<RentalDto>>.Ok(rentals, "Aluguéis encontrados com sucesso"));
    }

    /// <summary>
    /// Obtém o aluguel ativo de um entregador
    /// </summary>
    /// <param name="deliveryPersonId">ID do entregador</param>
    /// <returns>Aluguel ativo</returns>
    [HttpGet("active/by-delivery-person/{deliveryPersonId}")]
    [SwaggerResponse(200, "Aluguel ativo encontrado com sucesso", typeof(ApiResponse<RentalDto>))]
    [SwaggerResponse(404, "Aluguel ativo não encontrado", typeof(ApiResponse))]
    public async Task<IActionResult> GetActiveByDeliveryPersonId(Guid deliveryPersonId)
    {
        var rental = await _rentalService.GetActiveRentalByDeliveryPersonIdAsync(deliveryPersonId);
        
        if (rental == null)
            return NotFound(ApiResponse.Fail("Aluguel ativo não encontrado para este entregador"));

        return Ok(ApiResponse<RentalDto>.Ok(rental, "Aluguel ativo encontrado com sucesso"));
    }
}
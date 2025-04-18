using Microsoft.AspNetCore.Mvc;
using Motorcycle.API.Models.Responses;
using Motorcycle.Application.DTOs.Motorcycle;
using Motorcycle.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
namespace Motorcycle.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]

public class MotorcyclesController : ControllerBase
{
    private readonly IMotorcycleService _motorcycleService;
    private readonly ILogger<MotorcyclesController> _logger;

    public MotorcyclesController(
        IMotorcycleService motorcycleService,
        ILogger<MotorcyclesController> logger)
    {
        _motorcycleService = motorcycleService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todas as motos cadastradas
    /// </summary>
    /// <param name="licensePlate">Placa para filtrar (opcional)</param>
    /// <returns>Lista de motos</returns>
    [HttpGet]
    [SwaggerResponse(200, "Lista de motos retornada com sucesso", typeof(ApiResponse<IEnumerable<MotorcycleDto>>))]
    public async Task<IActionResult> GetAll([FromQuery] string? licensePlate = null)
    {
        IEnumerable<MotorcycleDto> motorcycles;
    
        if (!string.IsNullOrWhiteSpace(licensePlate))
        {
            motorcycles = await _motorcycleService.SearchByLicensePlateAsync(licensePlate);
        }
        else
        {
            motorcycles = await _motorcycleService.GetAllAsync();
        }
        
        // Converter para o formato de resposta exigido
        var response = motorcycles.Select(m => new MotorcycleResponseModel
        {
            Identificador = m.Id.ToString(),
            Ano = m.Year,
            Modelo = m.Model,
            Placa = m.LicensePlate
        });
        
        return Ok(response);
    }

    /// <summary>
    /// Obtém uma moto pelo ID
    /// </summary>
    /// <param name="id">ID da moto</param>
    /// <returns>Dados da moto</returns>
    [HttpGet("{id}")]
    [SwaggerResponse(200, "Moto encontrada com sucesso", typeof(ApiResponse<MotorcycleDto>))]
    [SwaggerResponse(404, "Moto não encontrada", typeof(ApiResponse))]
    public async Task<IActionResult> GetById(Guid id)
    {
        var motorcycle = await _motorcycleService.GetByIdAsync(id);
        
        if (motorcycle == null)
            return NotFound(ApiResponse.Fail("Moto não encontrada"));

        return Ok(ApiResponse<MotorcycleDto>.Ok(motorcycle, "Moto encontrada com sucesso"));
    }

    /// <summary>
    /// Cadastra uma nova moto
    /// </summary>
    /// <param name="createDto">Dados da moto</param>
    /// <returns>Moto cadastrada</returns>
    [HttpPost]
     // Restrict to admin role
    [SwaggerResponse(201, "Moto criada com sucesso", typeof(ApiResponse<MotorcycleDto>))]
    [SwaggerResponse(400, "Dados inválidos", typeof(ApiResponse))]
    public async Task<IActionResult> Create([FromBody] CreateMotorcycleDto createDto)
    {
        var createdMotorcycle = await _motorcycleService.CreateAsync(createDto);
        
        return CreatedAtAction(
            nameof(GetById), 
            new { id = createdMotorcycle.Id }, 
            ApiResponse<MotorcycleDto>.Ok(createdMotorcycle, "Moto criada com sucesso"));
    }

    /// <summary>
    /// Atualiza a placa de uma moto
    /// </summary>
    /// <param name="id">ID da moto</param>
    /// <param name="updateDto">Nova placa</param>
    /// <returns>Moto atualizada</returns>
    [HttpPut("{id}/license-plate")]
    [SwaggerResponse(200, "Placa atualizada com sucesso", typeof(ApiResponse<MotorcycleDto>))]
    [SwaggerResponse(400, "Dados inválidos", typeof(ApiResponse))]
    [SwaggerResponse(404, "Moto não encontrada", typeof(ApiResponse))]
    public async Task<IActionResult> UpdateLicensePlate(Guid id, [FromBody] UpdateMotorcycleLicensePlateDto updateDto)
    {
        var motorcycle = await _motorcycleService.GetByIdAsync(id);
        
        if (motorcycle == null)
            return NotFound(ApiResponse.Fail("Moto não encontrada"));

        var updatedMotorcycle = await _motorcycleService.UpdateLicensePlateAsync(id, updateDto);
        
        return Ok(ApiResponse<MotorcycleDto>.Ok(updatedMotorcycle, "Placa atualizada com sucesso"));
    }

    /// <summary>
    /// Remove uma moto
    /// </summary>
    /// <param name="id">ID da moto</param>
    /// <returns>Confirmação de remoção</returns>
    [HttpDelete("{id}")]
    [SwaggerResponse(200, "Moto removida com sucesso", typeof(ApiResponse))]
    [SwaggerResponse(400, "Não é possível remover a moto", typeof(ApiResponse))]
    [SwaggerResponse(404, "Moto não encontrada", typeof(ApiResponse))]
    public async Task<IActionResult> Delete(Guid id)
    {
        var motorcycle = await _motorcycleService.GetByIdAsync(id);
        
        if (motorcycle == null)
            return NotFound(ApiResponse.Fail("Moto não encontrada"));

        var result = await _motorcycleService.DeleteAsync(id);
        
        if (!result)
            return BadRequest(ApiResponse.Fail("Não foi possível remover a moto"));

        return Ok(ApiResponse.Ok("Moto removida com sucesso"));
    }
}
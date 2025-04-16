using Microsoft.AspNetCore.Mvc;
using Motorcycle.API.Models.Responses;
using Motorcycle.Application.DTOs.DeliveryPerson;
using Motorcycle.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Motorcycle.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DeliveryPersonsController : ControllerBase
{
    private readonly IDeliveryPersonService _deliveryPersonService;
    private readonly ILogger<DeliveryPersonsController> _logger;

    public DeliveryPersonsController(
        IDeliveryPersonService deliveryPersonService,
        ILogger<DeliveryPersonsController> logger)
    {
        _deliveryPersonService = deliveryPersonService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todos os entregadores cadastrados
    /// </summary>
    /// <returns>Lista de entregadores</returns>
    [HttpGet]
    [SwaggerResponse(200, "Lista de entregadores retornada com sucesso", typeof(ApiResponse<IEnumerable<DeliveryPersonDto>>))]
    public async Task<IActionResult> GetAll()
    {
        var deliveryPersons = await _deliveryPersonService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<DeliveryPersonDto>>.Ok(deliveryPersons, "Entregadores retornados com sucesso"));
    }

    /// <summary>
    /// Obtém um entregador pelo ID
    /// </summary>
    /// <param name="id">ID do entregador</param>
    /// <returns>Dados do entregador</returns>
    [HttpGet("{id}")]
    [SwaggerResponse(200, "Entregador encontrado com sucesso", typeof(ApiResponse<DeliveryPersonDto>))]
    [SwaggerResponse(404, "Entregador não encontrado", typeof(ApiResponse))]
    public async Task<IActionResult> GetById(Guid id)
    {
        var deliveryPerson = await _deliveryPersonService.GetByIdAsync(id);
        
        if (deliveryPerson == null)
            return NotFound(ApiResponse.Fail("Entregador não encontrado"));

        return Ok(ApiResponse<DeliveryPersonDto>.Ok(deliveryPerson, "Entregador encontrado com sucesso"));
    }

    /// <summary>
    /// Cadastra um novo entregador
    /// </summary>
    /// <param name="createDto">Dados do entregador</param>
    /// <returns>Entregador cadastrado</returns>
    [HttpPost]
    [SwaggerResponse(201, "Entregador criado com sucesso", typeof(ApiResponse<DeliveryPersonDto>))]
    [SwaggerResponse(400, "Dados inválidos", typeof(ApiResponse))]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryPersonDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse.Fail("Dados inválidos: " + string.Join(", ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage))));
        }
        
        if (createDto == null)
        {
            return BadRequest(ApiResponse.Fail("Os dados do entregador são obrigatórios."));
        }
        
        var createdDeliveryPerson = await _deliveryPersonService.CreateAsync(createDto);
        
        return CreatedAtAction(
            nameof(GetById), 
            new { id = createdDeliveryPerson.Id }, 
            ApiResponse<DeliveryPersonDto>.Ok(createdDeliveryPerson, "Entregador criado com sucesso"));
    }

    /// <summary>
    /// Atualiza a imagem da CNH de um entregador
    /// </summary>
    /// <param name="id">ID do entregador</param>
    /// <param name="file">Arquivo de imagem (PNG ou BMP)</param>
    /// <returns>Entregador atualizado</returns>
    [HttpPut("{id}/license-image")]
    [Consumes("multipart/form-data")]
    [SwaggerResponse(200, "Imagem da CNH atualizada com sucesso", typeof(ApiResponse<DeliveryPersonDto>))]
    [SwaggerResponse(400, "Dados inválidos", typeof(ApiResponse))]
    [SwaggerResponse(404, "Entregador não encontrado", typeof(ApiResponse))]
    public async Task<IActionResult> UpdateLicenseImage(Guid id, IFormFile file)
    {
        var deliveryPerson = await _deliveryPersonService.GetByIdAsync(id);
        
        if (deliveryPerson == null)
            return NotFound(ApiResponse.Fail("Entregador não encontrado"));

        var updatedDeliveryPerson = await _deliveryPersonService.UpdateDriverLicenseImageAsync(id, file);
        
        return Ok(ApiResponse<DeliveryPersonDto>.Ok(updatedDeliveryPerson, "Imagem da CNH atualizada com sucesso"));
    }

    /// <summary>
    /// Busca um entregador pelo CNPJ
    /// </summary>
    /// <param name="cnpj">CNPJ do entregador</param>
    /// <returns>Dados do entregador</returns>
    [HttpGet("by-cnpj/{cnpj}")]
    [SwaggerResponse(200, "Entregador encontrado com sucesso", typeof(ApiResponse<DeliveryPersonDto>))]
    [SwaggerResponse(404, "Entregador não encontrado", typeof(ApiResponse))]
    public async Task<IActionResult> GetByCnpj(string cnpj)
    {
        var deliveryPerson = await _deliveryPersonService.GetByCnpjAsync(cnpj);
        
        if (deliveryPerson == null)
            return NotFound(ApiResponse.Fail("Entregador não encontrado"));

        return Ok(ApiResponse<DeliveryPersonDto>.Ok(deliveryPerson, "Entregador encontrado com sucesso"));
    }

    /// <summary>
    /// Busca um entregador pelo número da CNH
    /// </summary>
    /// <param name="licenseNumber">Número da CNH</param>
    /// <returns>Dados do entregador</returns>
    [HttpGet("by-license/{licenseNumber}")]
    [SwaggerResponse(200, "Entregador encontrado com sucesso", typeof(ApiResponse<DeliveryPersonDto>))]
    [SwaggerResponse(404, "Entregador não encontrado", typeof(ApiResponse))]
    public async Task<IActionResult> GetByLicenseNumber(string licenseNumber)
    {
        var deliveryPerson = await _deliveryPersonService.GetByDriverLicenseNumberAsync(licenseNumber);
        
        if (deliveryPerson == null)
            return NotFound(ApiResponse.Fail("Entregador não encontrado"));

        return Ok(ApiResponse<DeliveryPersonDto>.Ok(deliveryPerson, "Entregador encontrado com sucesso"));
    }
}
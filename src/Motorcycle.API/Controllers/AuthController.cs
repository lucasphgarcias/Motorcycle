using Microsoft.AspNetCore.Mvc;
using Motorcycle.API.Models.Responses;
using Motorcycle.Application.DTOs.Auth;
using Motorcycle.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Motorcycle.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    /// <summary>
    /// Realiza login de usuário
    /// </summary>
    /// <param name="loginDto">Credenciais de login</param>
    /// <returns>Token de autenticação</returns>
    [HttpPost("login")]
    [SwaggerResponse(200, "Login realizado com sucesso", typeof(ApiResponse<AuthResponseDto>))]
    [SwaggerResponse(400, "Credenciais inválidas", typeof(ApiResponse))]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);
        return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Login realizado com sucesso"));
    }
    
    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    /// <param name="registerDto">Dados do usuário</param>
    /// <returns>Token de autenticação</returns>
    [HttpPost("register")]
    [SwaggerResponse(201, "Usuário registrado com sucesso", typeof(ApiResponse<AuthResponseDto>))]
    [SwaggerResponse(400, "Dados inválidos", typeof(ApiResponse))]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var response = await _authService.RegisterAsync(registerDto);
        
        return CreatedAtAction(
            nameof(Login),
            ApiResponse<AuthResponseDto>.Ok(response, "Usuário registrado com sucesso"));
    }
}
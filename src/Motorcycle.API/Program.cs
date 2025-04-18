// using System.Reflection;
// using System.Text.Json.Serialization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.OpenApi.Models;
// using Motorcycle.API.Middlewares;
// using Motorcycle.Application;
// using Motorcycle.Infrastructure;
// using Motorcycle.Infrastructure.Data.Migrations;
// using Motorcycle.API.Configurations;
// using Serilog;

// var builder = WebApplication.CreateBuilder(args);

// // Configurar Serilog
// Log.Logger = new LoggerConfiguration()
//     .ReadFrom.Configuration(builder.Configuration)
//     .Enrich.FromLogContext()
//     .WriteTo.Console()
//     .WriteTo.File("logs/motorcycle-api-.log", rollingInterval: RollingInterval.Day)
//     .CreateLogger();

// builder.Host.UseSerilog();

// // Adicionar serviços ao container
// builder.Services.AddControllers()
//     .AddJsonOptions(options =>
//     {
//         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
//         options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
//     });

// // Configurar API Behavior
// builder.Services.Configure<ApiBehaviorOptions>(options =>
// {
//     options.SuppressModelStateInvalidFilter = true;
// });

// // Configurar Swagger
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Title = "Motorcycle Rental API",
//         Version = "v1.0.0",
//         Description = "API para gerenciamento de aluguel de motos e entregadores"
//     });

//     c.EnableAnnotations();

//     // Incluir comentários XML
//     var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//     var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//     if (File.Exists(xmlPath))
//     {
//         c.IncludeXmlComments(xmlPath);
//     }

//     // Add this document filter to control the order
//     c.DocumentFilter<SwaggerControllerOrderFilter>();

//     // Add JWT Authentication to Swagger
//     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
//         Name = "Authorization",
//         In = ParameterLocation.Header,
//         Type = SecuritySchemeType.ApiKey,
//         Scheme = "Bearer"
//     });

//     c.AddSecurityRequirement(new OpenApiSecurityRequirement()
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "Bearer"
//                 },
//                 Scheme = "oauth2",
//                 Name = "Bearer",
//                 In = ParameterLocation.Header,
//             },
//             new List<string>()
//         }
//     });
// });

// // Adicionar camadas
// builder.Services.AddApplication();
// builder.Services.AddInfrastructure(builder.Configuration);

// var app = builder.Build();

// // Aplicar migrações de banco de dados
// app.MigrateDatabase();

// // Middleware para tratamento de exceções
// app.UseMiddleware<ExceptionHandlingMiddleware>();

// // Configurar pipeline de requisições HTTP
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c => 
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "Motorcycle Rental API v1");
//     });
// }

// // Para esta:
// app.UseSwagger();
// app.UseSwaggerUI(c => 
// {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Motorcycle Rental API v1");
// });

// // Add JWT authentication
// builder.Services.AddJwtAuthentication(builder.Configuration);


// app.UseHttpsRedirection();
// app.UseAuthentication();
// app.UseAuthorization();
// app.MapControllers();

// app.Run();

using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Motorcycle.API.Middlewares;
using Motorcycle.Application;
using Motorcycle.Infrastructure;
using Motorcycle.Infrastructure.Data.Migrations;
using Motorcycle.API.Configurations;
using Motorcycle.API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/motorcycle-api-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// 🔹 Configurar Controllers e JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// 🔹 Configurar comportamento da API
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// 🔹 Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Motorcycle Rental API",
        Version = "v1.0.0",
        Description = "API para gerenciamento de aluguel de motos e entregadores"
    });

    c.EnableAnnotations();

    // Comentários XML para documentação
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Ordem dos controladores no Swagger
    c.DocumentFilter<SwaggerControllerOrderFilter>();

    // 🔐 JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// 🔹 Adicionar as camadas Application e Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 🔹 Configurar Autenticação JWT (precisa ser antes do Build)
builder.Services.AddJwtAuthenticationConfig(builder.Configuration);

// 🔹 Criar a aplicação (depois de registrar tudo)
var app = builder.Build();

// 🔹 Aplicar migrações do banco de dados automaticamente
app.MigrateDatabase();

// 🔹 Middleware customizado para exceções
app.UseMiddleware<ExceptionHandlingMiddleware>();

// builder.Services.AddJwtAuthentication(configuration);

// 🔹 Habilitar Swagger (em qualquer ambiente)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Motorcycle Rental API v1");
});

// 🔹 Middleware padrão ASP.NET
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 🔹 Executar
app.Run();

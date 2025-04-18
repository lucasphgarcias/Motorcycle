using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Motorcycle.Domain.Enums;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;

namespace Motorcycle.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static void AddCustomExamples(this SwaggerGenOptions options)
        {
            // Configurar exemplos personalizados para tipos específicos
            options.SchemaFilter<EnumSchemaFilter>();
        }
    }

    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                // Para o enum UserRole, mostrar valores específicos
                if (context.Type == typeof(UserRole))
                {
                    schema.Example = new OpenApiString("0(User) ou 1(Admin)");
                    schema.Description = "Função do usuário: 0(User) ou 1(Admin)";
                }
            }
        }
    }
} 
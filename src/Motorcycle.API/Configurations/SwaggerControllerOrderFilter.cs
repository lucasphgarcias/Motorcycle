using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Motorcycle.API.Configurations
{
    public class SwaggerControllerOrderFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Define the desired controller order
            var orderMap = new Dictionary<string, int>
            {
                {"Motorcycles", 1},
                {"DeliveryPersons", 2},
                {"Rentals", 3}
            };

            var paths = swaggerDoc.Paths.ToList();
            
            // Sort paths based on controller name order
            var sortedPaths = paths
                .OrderBy(p => 
                {
                    // Extract controller name from path
                    var controllerName = p.Key.Split('/')[2];
                    
                    // Return the defined order or a large number if not found
                    return orderMap.TryGetValue(controllerName, out var order) ? order : 99;
                })
                .ToDictionary(x => x.Key, x => x.Value);

            // Replace with sorted dictionary
            swaggerDoc.Paths.Clear();
            foreach (var path in sortedPaths)
            {
                swaggerDoc.Paths.Add(path.Key, path.Value);
            }
        }
    }
}
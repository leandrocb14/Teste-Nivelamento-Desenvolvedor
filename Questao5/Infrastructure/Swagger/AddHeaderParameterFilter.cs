using Microsoft.OpenApi.Models;
using Questao5.Infrastructure.Constants;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Questao5.Infrastructure.Swagger
{
    public class AddHeaderParameterFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = HeaderConstants.IDEMPOTENCY_KEY,
                In = ParameterLocation.Header,
                Required = false,
                Description = "Avoid duplicated requests",
                
            });
        }
    }
}

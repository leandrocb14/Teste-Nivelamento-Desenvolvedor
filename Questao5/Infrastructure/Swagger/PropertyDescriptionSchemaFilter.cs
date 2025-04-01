using System.ComponentModel;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Questao5.Infrastructure.Swagger
{
    public class PropertyDescriptionSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsClass)
            {
                foreach (var property in context.Type.GetProperties())
                {
                    var description = property.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                               .Cast<DescriptionAttribute>()
                                               .FirstOrDefault()?.Description;

                    if (!string.IsNullOrEmpty(description))
                    {
                        var propertyNameCamelCase = string.Concat(property.Name[0].ToString().ToLower(), property.Name.Substring(1));
                        if (schema.Properties.ContainsKey(propertyNameCamelCase))
                        {
                            schema.Properties[propertyNameCamelCase].Description = description;
                        }
                    }
                }
            }
        }
    }
}

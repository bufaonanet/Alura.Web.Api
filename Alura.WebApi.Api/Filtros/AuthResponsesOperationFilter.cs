using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebApi.Api.Filtros
{
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
        }
    }
}

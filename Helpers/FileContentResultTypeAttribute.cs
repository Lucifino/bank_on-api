using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace bank_on_api.Helpers
{
    public class FileContentResultTypeAttribute
    {
        [AttributeUsage(AttributeTargets.Method)]
        public class FileResultContentTypeAttribute : Attribute
        {
            public FileResultContentTypeAttribute(string contentType)
            {
                ContentType = contentType;
            }

            public string ContentType { get; }
        }

        public class FileResultContentTypeOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var requestAttribute = context.MethodInfo.GetCustomAttributes(typeof(FileResultContentTypeAttribute), false)
                    .Cast<FileResultContentTypeAttribute>()
                    .FirstOrDefault();

                if (requestAttribute == null) return;

                operation.Responses.Clear();
                operation.Responses.Add("200", new OpenApiResponse
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            requestAttribute.ContentType, new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                }
                            }
                        }
                    }
                });
            }
        }
    }
}
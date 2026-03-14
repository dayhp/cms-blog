using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CMSBlog.API.Filters
{
    public class SwaggerNullableParameterFilter : IParameterFilter
    {
        //public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        //{
        //    if (parameter.Schema.Nullable &&
        //        !context.ApiParameterDescription.Type.IsValueType)
        //    {
        //        parameter.Schema.Nullable = true;
        //    }
        //}
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (context.ParameterInfo != null)
            {
                var type = context.ParameterInfo.ParameterType;

                if (Nullable.GetUnderlyingType(type) != null)
                {
                    parameter.Schema.Nullable = true;
                    parameter.Required = false;
                }
            }
        }
    }
}

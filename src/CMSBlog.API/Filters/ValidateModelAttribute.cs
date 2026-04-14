using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CMSBlog.API.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {

                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}

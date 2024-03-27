using BuisinessLayer.CustomException;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.Filter.ExceptionFilter
{
    public class LableExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            if(context.Exception is LableNotFoundException)
            {

                context.ModelState.AddModelError("GetLable", context.Exception.Message);
                ValidationProblemDetails problemdetail = new ValidationProblemDetails(context.ModelState);
                problemdetail.Status = StatusCodes.Status404NotFound;
                context.Result = new NotFoundObjectResult(problemdetail);
            }
        }
    }
}

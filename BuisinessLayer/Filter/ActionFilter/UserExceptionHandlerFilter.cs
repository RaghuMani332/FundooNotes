using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;

namespace BuisinessLayer.Filter.ActionFilter
{
    public class UserExceptionHandlerFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
           if(context.Exception is SqlException)
            {
                context.ModelState.AddModelError("EMAIL ID","EMAIL ALREADY PRESENT");
                ValidationProblemDetails problemdetail = new ValidationProblemDetails(context.ModelState);
                problemdetail.Status = StatusCodes.Status409Conflict;
                context.Result = new ConflictObjectResult(problemdetail);
            }
            if(context.Exception is Exception)
            {
                context.ModelState.AddModelError("this is a message for which field exception occured"," here reason");
            }
        }
    }
}

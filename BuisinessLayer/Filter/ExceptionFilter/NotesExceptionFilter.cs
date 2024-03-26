using BuisinessLayer.CustomException;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.Filter.ExceptionFilter
{
    public class NotesExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
           if(context.Exception is UnableToCreateNoteException)
            {
                context.ModelState.AddModelError("Notes", context.Exception.Message);
                ValidationProblemDetails problemdetail = new ValidationProblemDetails(context.ModelState);
                problemdetail.Status = StatusCodes.Status422UnprocessableEntity;
                context.Result = new UnprocessableEntityObjectResult(problemdetail);
            }
           else if(context.Exception is UnableToFetchAllNotesException)
            {
                context.ModelState.AddModelError("GetAllNotes", context.Exception.Message);
                ValidationProblemDetails problemdetail = new ValidationProblemDetails(context.ModelState);
                problemdetail.Status = StatusCodes.Status404NotFound;
                context.Result = new NotFoundObjectResult(problemdetail);
            }
           else if(context.Exception is UnableToDeleteLabelException)
            {
                context.ModelState.AddModelError("DeleteLable", context.Exception.Message);
                ValidationProblemDetails problemdetail = new ValidationProblemDetails(context.ModelState);
                problemdetail.Status = StatusCodes.Status409Conflict;
                context.Result = new ConflictObjectResult(problemdetail);
            }
           else if (context.Exception is NoteNotPresentByIdException)
            {
                context.ModelState.AddModelError("NoteId", context.Exception.Message);
                ValidationProblemDetails problemdetail = new ValidationProblemDetails(context.ModelState);
                problemdetail.Status = StatusCodes.Status404NotFound;
                context.Result = new NotFoundObjectResult(problemdetail);
            }
           else if (context.Exception is UnableToDeletNoteException)
            {
                context.ModelState.AddModelError("DeleteNote", context.Exception.Message);
                ValidationProblemDetails problemdetail = new ValidationProblemDetails(context.ModelState);
                problemdetail.Status = StatusCodes.Status409Conflict;
                context.Result = new ConflictObjectResult(problemdetail);
            }
           else if(context.Exception is UserNotFoundException)
            {
                context.ModelState.AddModelError("UserNotFound", context.Exception.Message);
                ValidationProblemDetails problemdetail = new ValidationProblemDetails(context.ModelState);
                problemdetail.Status = StatusCodes.Status404NotFound;
                context.Result = new NotFoundObjectResult(problemdetail);
            }
            else if(context.Exception is UserMissMatchException)
            {
                context.ModelState.AddModelError("UserMissMatchException", context.Exception.Message);
                ValidationProblemDetails problemdetail = new ValidationProblemDetails(context.ModelState);
                problemdetail.Status = StatusCodes.Status404NotFound;
                context.Result = new NotFoundObjectResult(problemdetail);
            }
           else 
            {
                context.ModelState.AddModelError("unknown exception in notes contact developer", context.Exception.Message + "  stacktrace==> " + context.Exception.StackTrace);
                ValidationProblemDetails problemDetails = new ValidationProblemDetails(context.ModelState);
                problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                context.Result = new UnprocessableEntityObjectResult(problemDetails);
            }
        }
    }
}

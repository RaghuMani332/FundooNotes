using BuisinessLayer.Filter.ExceptionFilter;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FundooNotes.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesService NotesService;
        public NotesController(INotesService NotesService) { 
            this.NotesService = NotesService;
        }

        [HttpPost("/CreateNote")]
        [NotesExceptionFilter]
        public IActionResult createNotes(NotesRequest request)
        {
            
            return Ok(NotesService.createNotes(request));
        }

        [HttpGet("GetAllNotes")]
        [NotesExceptionFilter]
        public IActionResult GetAllNotes(String Email)
        {
           return Ok( NotesService.GetAllNotes(Email));
        }
        [HttpPut("UpdateNotes")]
        [NotesExceptionFilter]
        public IActionResult UpdateNotes(NotesRequest update,int noteId)
        {
           
            return Ok(NotesService.UpdateNotes(update, noteId));
        }
        [HttpDelete("DeleteNotes")]
        [NotesExceptionFilter]
        public IActionResult deleteNotes(int noteId,String Email)
        {
            NotesService.DeleteNote(noteId, Email);
            return Ok();
        }
        [HttpDelete("DeleteLabel")]
        [NotesExceptionFilter]
        public IActionResult deleteLabel(String LableName)
        {
            NotesService.deleteLabel(LableName);
            return Ok();

        }

    }
}

using BuisinessLayer.Filter.ExceptionFilter;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FundooNotes.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
   //  [Authorize]
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
            //------------SAMPLE INPUT FOR THIS------- 
            /*
             {
  "title": "done by collabrato 2",
  "description": "string",
  "bgColor": "string",
  "imagePath": "string",
  "remainder": "2024-03-27T13:05:23.052Z",
  "isArchive": true,
  "isPinned": true,
  "isTrash": true,
  "collabEmailId": [
        "raghum11154@gmail.com",
    "srikanthraghu11154@gmail.com",
    "subamraghu11154@gmail.com"
  ],
  "userEmailId": "raghumani11154@gmail.com"
}
             */
            // WHENEVER UPDATE REQUEST IS COMMING IT SHOULD BE CONTAIN ALL COLLABID,USERID --> colabemail we can get through the GETBYID() method
            return Ok(NotesService.UpdateNotes(update, noteId));
        }
        [HttpDelete("DeleteNotes")]
        [NotesExceptionFilter]
        public IActionResult deleteNotes(int noteId,String Email)
        {
            NotesService.DeleteNote(noteId, Email);
            return Ok("Deleted SuccessFully");
        }
        [HttpGet("GetById /{noteId}")]
        [NotesExceptionFilter]
        public IActionResult GetByNoteId(int noteId)
        {
            return Ok(NotesService.GetByNoteId(noteId));
        }

    }
}

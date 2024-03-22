using BuisinessLayer.service.Iservice;
using CommonLayer.Models.RequestDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FundooNotes.Controllers
{

    /*unable to create note exception -->notesrepo 40
    unable to fetch all notes ->notesrepo 90
    "UserMissMatchException()160
    in line 193 note service impl issue is there to solve
    in 206,201 throw new exceptuon


    logger,session,exception filter,lable

*/
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesService NotesService;
        public NotesController(INotesService NotesService) { 
            this.NotesService = NotesService;
        }

        [HttpPost("/createNote")]
        public IActionResult createNotes(NotesRequest request)
        {
            
            return Ok(NotesService.createNotes(request));
        }

        [HttpGet]
        public IActionResult GetAllNotes(String Email)
        {
           return Ok( NotesService.GetAllNotes(Email));
        }
        [HttpPut]
        public IActionResult UpdateNotes(NotesRequest update,int noteId)
        {
            NotesService.UpdateNotes(update,noteId);
            return Ok();
        }
        [HttpDelete]
        public IActionResult deleteNotes(int noteId,String Email)
        {
            NotesService.DeleteNote(noteId, Email);
            return Ok();
        }

    }
}

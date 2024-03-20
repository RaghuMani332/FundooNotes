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
        public IActionResult UpdateNotes(NotesRequest update)
        {
            NotesService.UpdateNotes(update);
            return Ok();
        }

    }
}

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
            NotesService.createNotes(request);
            return Ok();
        }
    }
}

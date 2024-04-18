using Azure.Core;
using BuisinessLayer.Filter.ExceptionFilter;
using BuisinessLayer.service.Iservice;
using CommonLayer.Models;
using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FundooNotes.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [NotesExceptionFilter]
    public class NotesController : ControllerBase
    {
        private readonly INotesService NotesService;
        public NotesController(INotesService NotesService) { 
            this.NotesService = NotesService;
        }

        [HttpPost]
        public ResponceStructure<Dictionary<String, List<NotesResponce>>> createNotes(NotesRequest request)
        {
            request.UserEmailId = User.FindFirstValue(ClaimTypes.Email);
            return new ResponceStructure<Dictionary<String, List<NotesResponce>>>(NotesService.createNotes(request),"Success");
        }

        [HttpGet]
        
        public ResponceStructure<Dictionary<String, List<NotesResponce>>> GetAllNotes()
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);
            return new ResponceStructure<Dictionary<String,List<NotesResponce>>>(NotesService.GetAllNotes(Email), "Success");
        }
        [HttpPut]
        public ResponceStructure<Dictionary<String, List<NotesResponce>>> UpdateNotes(NotesRequest update,int noteId)
        {
            update.UserEmailId = User.FindFirstValue(ClaimTypes.Email);

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
            return new ResponceStructure<Dictionary<String, List<NotesResponce>>>(NotesService.UpdateNotes(update, noteId), "Success");
           // return Ok(NotesService.UpdateNotes(update, noteId));
        }
        [HttpDelete]
        [NotesExceptionFilter]
        public ResponceStructure<String> deleteNotes(int noteId)
        {
            var Email=User.FindFirstValue(ClaimTypes.Email);
            NotesService.DeleteNote(noteId, Email);
           return new ResponceStructure<String>("Deleted SuccessFully", "Success");
          //  return Ok("Deleted SuccessFully");
        }
        [HttpGet("{noteId}")]
        [NotesExceptionFilter]
        public ResponceStructure<NotesResponce> GetByNoteId(int noteId)
        {
            return new ResponceStructure<NotesResponce>(NotesService.GetByNoteId(noteId), "Success");
            //return Ok(NotesService.GetByNoteId(noteId));
        }
       

    }
}

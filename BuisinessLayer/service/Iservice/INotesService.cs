using CommonLayer.Models.RequestDto;
using CommonLayer.Models.ResponceDto;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface INotesService
    {
        public Dictionary<String, List<NotesResponce>> createNotes(NotesRequest request);
        void DeleteNote(int noteId,String Email);
        Dictionary<String, List<NotesResponce>> GetAllNotes(string email);
        void UpdateNotes(NotesRequest update,int NoteId);
    }
}

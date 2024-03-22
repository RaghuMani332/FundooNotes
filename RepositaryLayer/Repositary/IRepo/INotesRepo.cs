using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface INotesRepo
    {
        public Dictionary<String, List<NotesEntity>> createNote(NotesEntity entity);
        Dictionary<String, List<NotesEntity>> GetAllNotes(int userId);
        void UpdateNotes(NotesEntity notesEntity);
      //  public void AddCollaborator(int noteId, int collaboratorId);
        void DeleteNotes(int noteId);
        NotesEntity GetById(int noteId);
        List<int> GetUserByNotesIdInCollab(int noteId);
    }
}

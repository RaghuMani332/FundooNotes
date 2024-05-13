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
        void DeleteNotes(int noteId);
        NotesEntity GetById(int noteId);
        List<int> GetUserByNotesIdInCollab(int noteId);
        int updateArchive(int noteId, int userId);
        int updateColor(int noteId, int userId, string color);
        int UpdateTrash(int noteId, int userId);
        int permanentDelete(int noteId);
    }
}

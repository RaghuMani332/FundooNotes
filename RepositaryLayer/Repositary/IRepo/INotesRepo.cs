using RepositaryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface INotesRepo
    {
        public int createNote(NotesEntity entity);
        
    }
}

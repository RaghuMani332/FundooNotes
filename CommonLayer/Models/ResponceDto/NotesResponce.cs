using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Models.ResponceDto
{
    public class NotesResponce
    {
        public int NoteId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BgColor { get; set; }
        public string ImagePath { get; set; }
        public DateTime? Remainder { get; set; }
        public bool IsArchive { get; set; }
        public bool IsPinned { get; set; }
        public bool IsTrash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public List<string> CollabEmailId { get; set; } // Change the type to List<string>
        public string UserEmailId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Models.RequestDto
{
    public class NotesRequest
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string? BgColor { get; set; }
        public string? ImagePath { get; set; }
        public DateTime? Remainder { get; set; }
        public bool IsArchive { get; set; } = false;
        public bool IsPinned { get; set; }
        //public bool IsTrash { get; set; }//(When create notes this should be false)
     //   public DateTime CreatedAt { get; set; }
       // public DateTime ModifiedAt { get; set; }
        public String? CollabEmailId { get; set; }
        public String UserEmailId { get; set; }
    }
}

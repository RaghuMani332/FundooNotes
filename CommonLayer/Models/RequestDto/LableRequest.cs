using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Models.RequestDto
{
    public class LableRequest
    {
        public int LableId { get; set; }
        public String LabelName { get; set; }
        public String UserEmail { get; set; }
        public int NoteId { get; set; }
    }
}

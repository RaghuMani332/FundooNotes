using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace CommonLayer.Models.ResponceDto
{
    public class LableResponce
    {
        public int LabelId { get; set; }
        public String LabelName { get; set; }
        public List<int> NoteId { get; set; }
        public String UserEmail { get; set; }
    }
}

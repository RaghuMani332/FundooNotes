using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Entity
{
    public class LableEntity
    {
        public int LabelId { get; set; }
        public String LabelName { get; set; }
        public List<int> NoteId { get; set; }
        public int UserId { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommonLayer.Models.RequestDto
{
    public class LableRequest
    {

        [JsonIgnore]
        public int LableId { get; set; }
        public String LabelName { get; set; }
        [JsonIgnore]
        public String? UserEmail { get; set; }
        public List<int> NoteId { get; set; }
    }
}

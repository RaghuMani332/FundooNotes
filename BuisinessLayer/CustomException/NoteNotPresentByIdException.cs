using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.CustomException
{
    public class NoteNotPresentByIdException :Exception
    {
        public NoteNotPresentByIdException() { }
        public NoteNotPresentByIdException(string message): base(message) { }
    }
}

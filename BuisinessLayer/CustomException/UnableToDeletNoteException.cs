using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.CustomException
{
    public class UnableToDeletNoteException : Exception
    {
        public UnableToDeletNoteException() { }
        public UnableToDeletNoteException(string message) : base(message) { }
    }
}

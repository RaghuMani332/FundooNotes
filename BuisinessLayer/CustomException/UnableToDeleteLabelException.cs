using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.CustomException
{
    public class UnableToDeleteLabelException : Exception
    {
        public UnableToDeleteLabelException() { }
        public UnableToDeleteLabelException(string message) : base(message) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.CustomException
{
    public class LableNotFoundException : Exception
    {
        public LableNotFoundException() { }
        public LableNotFoundException(string message) : base(message) { }
    }
}

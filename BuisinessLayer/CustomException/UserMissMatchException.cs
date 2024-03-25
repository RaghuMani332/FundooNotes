using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.CustomException
{
    public class UserMissMatchException : Exception
    {
        public UserMissMatchException() { }
        public UserMissMatchException(string message) : base(message) { }
    }
}

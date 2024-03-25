﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.CustomException
{
    public class UnableToCreateNoteException : Exception
    {
        public UnableToCreateNoteException() { }
        public UnableToCreateNoteException(string message) : base(message) { }
    }
}
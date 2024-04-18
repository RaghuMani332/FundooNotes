using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Models
{
    public class ResponceStructure<T>
    {
        public ResponceStructure(T data,String Message) 
        {
            this.Data = data;
            this.Message = Message;
        }
        public bool Success { get; set; }=true;
        public String Message { get; set; }
        public T Data { get; set; }
    }
}

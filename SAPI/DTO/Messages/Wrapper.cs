using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Messages
{
    public class Wrapper
    {
        public string Status { get; set; }

        public int Code { get; set; }

        public Dictionary<string, string> Messages { get; set; }

        public Object Data { get; set; }

        public Wrapper()
        {
            Status = "OK";
            Code = 200;
            Messages = new Dictionary<string, string>();
            Data = null;
        }
    }
}
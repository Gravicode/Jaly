using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.DKI.Entities
{
    public class OutputData
    {
        public bool IsSucceed { set; get; }
        public object Data { set; get; }
        public string ErrorMessage { set; get; }
    }
}

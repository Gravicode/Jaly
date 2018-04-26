using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DKI.Bot.App.Helpers
{
    [Serializable]
    public class OutputCls
    {
        public ResultImage Result { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSucceed { get; set; }
    }

    [Serializable]
    public class ResultImage
    {
        public string SubmissionId { get; set; }
        public string Url { get; set; }
    }

}
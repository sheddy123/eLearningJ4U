using OtpSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eLearningLMS.Models
{
    public class GoogleAuthenticatorViewModel
    {
        public string Codes { get; set; }
      
        public string SecretKey { get; set; }
        public string BarcodeUrl { get; set; }


    }
}
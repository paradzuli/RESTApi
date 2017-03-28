using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestWebApi.Models
{
    public class ServiceStatus
    {

        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string ReasonPhrase { get; set; }
    }
}
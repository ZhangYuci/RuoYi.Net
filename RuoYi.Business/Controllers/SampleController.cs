using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuoYi.Business.Controllers
{
    [ApiDescriptionSettings("Business")]
    [Route("business/sample")]
    public class SampleController : ControllerBase
    {
        [HttpGet("helloworld")]
        public string HelloWorld()
        {
            return "Hello World";
        }

    }
}

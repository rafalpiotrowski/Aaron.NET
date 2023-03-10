using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchingEngine.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WriteSomeLogsController : ControllerBase
    {
        private readonly ILogger<WriteSomeLogsController> _logger;

        public WriteSomeLogsController(ILogger<WriteSomeLogsController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet]
        public void GenerateLogs(int nrOfLoggsToGenerate)
        {
            for(int i=0; i<nrOfLoggsToGenerate; i++)
            {
                _logger.LogInformation($"{i}: log generated");
            }
            _logger.LogInformation($"{nrOfLoggsToGenerate} log messages generated");
        }
    }
}

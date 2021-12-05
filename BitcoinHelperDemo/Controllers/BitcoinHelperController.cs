using Microsoft.AspNetCore.Mvc;

namespace BitcoinHelperDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BitcoinHelperController : ControllerBase
    {

        private readonly ILogger<BitcoinHelperController> _logger;

        public BitcoinHelperController(ILogger<BitcoinHelperController> logger)
        {
            _logger = logger;
        }

    }
}
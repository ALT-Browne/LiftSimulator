using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class LiftSimController : ControllerBase
{
        private readonly ILogger<LiftSimController> _logger;
        private readonly ILiftSimService _LiftSimService;

        public LiftSimController(ILogger<LiftSimController> logger,
                            ILiftSimService LiftSimService)
        {
                _logger = logger;
                _LiftSimService = LiftSimService;
        }

        [HttpPost]
        [Route("sendLiftDataFromCsv")]
        public IActionResult SendLiftDataFromCsv([FromForm] IFormFile file)
        {
                Result<Lift> liftDataResponse = _LiftSimService.SendLiftDataFromCsv(file);

                if (liftDataResponse.Payload == null)
                        return BadRequest(new { errorMessage = liftDataResponse.ErrorMessage });

                return Ok(liftDataResponse);
        }

        [HttpPost]
        [Route("sendLiftDataFromUser")]
        public IActionResult SendLiftDataFromUser(LiftData liftDataRequest)
        {
                Result<Lift> liftDataResponse = _LiftSimService.SendLiftDataFromUser(liftDataRequest);

                if (liftDataResponse.Payload == null)
                        return BadRequest(new { errorMessage = liftDataResponse.ErrorMessage });

                return Ok(liftDataResponse);
        }
}

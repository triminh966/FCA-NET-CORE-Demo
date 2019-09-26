using FCA.API.Models;
using FCA.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCA.API.Controllers
{
    //[Authorize]
    [Route("api/challenge-tracker")]
    [ApiController]
    public class ChallengeTrackerController : ControllerBase
    {
        private readonly IChallengeTrackerService _challengeTrackerService;

        public ChallengeTrackerController(IChallengeTrackerService challengeTrackerService)
        {
            _challengeTrackerService = challengeTrackerService;
        }

        [HttpGet]
        [Route("studio-avg/{challengeId}")]
        public IActionResult GetStudioAvg(int challengeId, [FromQuery] StudioChallengeResultAvgParameters parameters)
        {
            return Ok();
        }
    }
}
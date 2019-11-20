using FCA.API.Models;
using FCA.API.Services;
using FCA.Data.Entities;
using FCA.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FCA.API.Controllers
{
    //[Authorize]
    [Route("api/challenge-tracker")]
    [ApiController]
    public class ChallengeTrackerController : ControllerBase
    {
        private readonly IChallengeTrackerService _challengeTrackerService;
        private readonly IStudentRepository _studentRepository;

        public ChallengeTrackerController(IChallengeTrackerService challengeTrackerService, IStudentRepository StudentRepository)
        {
            _challengeTrackerService = challengeTrackerService;
            _studentRepository = StudentRepository;
        }

        [HttpGet]
        [Route("studio-avg/{challengeId}")]
        public IActionResult GetStudioAvg(int challengeId, [FromQuery] StudioChallengeResultAvgParameters parameters)
        {
            return Ok();
        }
        [HttpGet]
        [Route("student")]
        public ActionResult<List<Student>> GetStudent()
        {
            try
            {
                var dto = _studentRepository.GetAll().ToList();
                return Ok(dto);
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
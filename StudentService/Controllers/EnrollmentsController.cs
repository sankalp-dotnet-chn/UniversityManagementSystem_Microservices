using Microsoft.AspNetCore.Mvc;
using StudentService.Models;
using System.Net;
using System.Net.Http.Json;

namespace StudentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private static readonly List<Enrollment> Enrollments = new();
        private static int _nextId = 1;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EnrollmentsController> _logger;

        public EnrollmentsController(IHttpClientFactory httpClientFactory, ILogger<EnrollmentsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Enrollment>> Get() => Ok(Enrollments);

        public class EnrollRequest
        {
            public int StudentId { get; set; }
            public string StudentName { get; set; }
            public int CourseId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EnrollRequest req)
        {
            if (req == null) return BadRequest("Invalid payload");

            var client = _httpClientFactory.CreateClient("CourseService");
            // call CourseService to validate and fetch course details
            var resp = await client.GetAsync($"/api/courses/{req.CourseId}");
            if (resp.StatusCode == HttpStatusCode.NotFound)
                return BadRequest($"Course with id {req.CourseId} not found.");

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("CourseService returned {Status}", resp.StatusCode);
                return StatusCode(502, "Failed to validate course");
            }

            var course = await resp.Content.ReadFromJsonAsync<CourseDto>();
            if (course == null)
                return StatusCode(502, "Invalid response from course service");

            var enrollment = new Enrollment
            {
                Id = _nextId++,
                StudentId = req.StudentId,
                StudentName = req.StudentName,
                CourseId = course.Id,
                CourseTitle = course.Title
            };

            Enrollments.Add(enrollment);

            return CreatedAtAction(nameof(Get), new { id = enrollment.Id }, enrollment);
        }
    }
}

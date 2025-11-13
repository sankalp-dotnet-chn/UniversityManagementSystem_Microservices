using Microsoft.AspNetCore.Mvc;
using CourseService.Models;

namespace CourseService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private static readonly List<Course> courses = new()
        {
            new() { Id = 1, Title = "Microservices Architecture", Instructor = "Dr. Allen", Credits = 4 },
            new() { Id = 2, Title = "Cloud Computing", Instructor = "Prof. Smith", Credits = 3 },
            new() { Id = 3, Title = "Data Structures", Instructor = "Dr. Lee", Credits = 4 }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Course>> Get() => Ok(courses);

        [HttpGet("{id:int}")]
        public ActionResult<Course> Get(int id)
        {
            var c = courses.FirstOrDefault(x => x.Id == id);
            if (c == null) return NotFound();
            return Ok(c);
        }
    }
}

using AnyLearnServer.Database;
using AnyLearnServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnyLearnServer.Controllers
{
    [Route("/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ILogger<CoursesController> _logger;
        private readonly AnyLearnDatabaseContext _context;

        public CoursesController(ILogger<CoursesController> logger, AnyLearnDatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        [Route("/courses")]
        public async Task<Course> PostAsync([FromBody] Course course)
        {
            await _context.Courses!.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }

    }
}

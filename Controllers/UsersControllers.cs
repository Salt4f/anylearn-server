using System.Collections.Generic;
using System.Threading.Tasks;
using AnyLearnServer.Database;
using AnyLearnServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnyLearnServer.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersControllers : ControllerBase
    {

        private readonly ILogger<UsersControllers> _logger;
        private readonly AnyLearnDatabaseContext _context;

        public UsersControllers(ILogger<UsersControllers> logger, AnyLearnDatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet, Route("/users/{token}")]
        public async Task<User?> GetAsync(string token)
        {
            _logger.LogInformation("GET /users/" + token);
            var user = await _context.Users!.FirstOrDefaultAsync(u => u.Token == token);
            return user;
        }

    }
}

using System.Text;
using AnyLearnServer.Database;
using AnyLearnServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnyLearnServer.Controllers
{
    [ApiController]
    [Route("/mock")]
    public class MockController : ControllerBase
    {

        private readonly ILogger<MockController> _logger;
        private readonly AnyLearnDatabaseContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public MockController(ILogger<MockController> logger, AnyLearnDatabaseContext context, HttpClient httpClient, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _httpClient = httpClient;
            _config = config;
        }

        [HttpPost, Route("/mock/start")]
        public async Task<IActionResult> PostAsync()
        {
            _logger.LogInformation("POST /mock/start");
            
            _logger.LogInformation("Recreating database...");
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            _logger.LogInformation("Registering users...");

            User[] users =
            {
                // Duplicar esta línea para añadir más
                new User() {Name="Pepito",Surname="Grillo",Email="pepito.grillo@anylearn.es",Linkedin=false,Password="pepito",Photo="null"},
            };

            foreach (var user in users)
            {
                user.Token = Convert.ToBase64String(Encoding.ASCII.GetBytes(user.Email!));
                user.Password = Utils.Crypto.GetHashString(user.Password!);
            }

            await _context.Users!.AddRangeAsync(users);

            _logger.LogInformation("Saving changes...");
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}

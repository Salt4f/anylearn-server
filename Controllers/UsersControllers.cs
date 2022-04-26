using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AnyLearnServer.Database;
using AnyLearnServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnyLearnServer.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersControllers : ControllerBase
    {

        private readonly ILogger<UsersControllers> _logger;
        private readonly AnyLearnDatabaseContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public UsersControllers(ILogger<UsersControllers> logger, AnyLearnDatabaseContext context, HttpClient httpClient, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _httpClient = httpClient;
            _config = config;
        }

        [HttpGet, Route("/users/{token}")]
        public async Task<User?> GetAsync(string token)
        {
            _logger.LogInformation("GET /users/" + token);
            var user = await _context.Users!.FirstOrDefaultAsync(u => u.Token == token);
            return user;
        }

        [HttpPost, Route("/users/{code}")]
        public async Task<User?> PostAsync(string code)
        {
            _logger.LogInformation("POST /users/" + code);
            //Get access token

            var client = _config.GetValue<string>("LinkedIn.ClientId");
            var secret = _config.GetValue<string>("LinkedIn.Secret");

            string url = "https://www.linkedin.com/oauth/v2/accessToken?grant_type=authorization_code&code=" +
                code +
                "&redirect_uri=http://localhost:3000/linkedin" +
                "&client_id=" +
                client +
                "&client_secret=" +
                secret;
            var res = await _httpClient.GetAsync(url);
            res.EnsureSuccessStatusCode();
            var body = await res.Content.ReadAsStringAsync();
            var json = JObject.Parse(body);
            string? token = json.GetValue("access_token")!.Value<string>();

            url = "https://api.linkedin.com/v2/me?projection=(localizedFirstName,localizedLastName,profilePicture(displayImage~:playableStreams))";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            res = await _httpClient.SendAsync(request);
            res.EnsureSuccessStatusCode();

            body = await res.Content.ReadAsStringAsync();
            json = JObject.Parse(body);

            var user = new User()
            {
                Email = "",
                Linkedin = true,
                Name = json.GetValue("localizedFirstName")!.Value<string>(),
                Surname = json.GetValue("localizedLastName")!.Value<string>(),
                Password = null,
                Photo = json.GetValue("profilePicture")!.Value<JObject>()
                            !.GetValue("displayImage~")!.Value<JObject>()
                            !.GetValue("elements")!.Value<JArray>()
                            !.First!.Value<JObject>()
                            !.GetValue("identifiers")!.Value<JArray>()
                            !.First!.Value<JObject>()
                            !.GetValue("identifier")!.Value<string>()
            };

            url = "https://api.linkedin.com/v2/emailAddress?q=members&projection=(elements*(handle~))";
            request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            res = await _httpClient.SendAsync(request);
            res.EnsureSuccessStatusCode();

            body = await res.Content.ReadAsStringAsync();
            json = JObject.Parse(body);

            string? email = json.GetValue("elements")!.Value<JArray>()!.First!.Value<JObject>()!.GetValue("handle~")!.Value<JObject>()!.GetValue("emailAddress")!.Value<string>();

            user.Email = email!;

            _context.Users!.Update(user);

            await _context.SaveChangesAsync();

            return user;
        }

        /*
         * name
         * lastname
         * password
         * email
         * 
        */

    }
}

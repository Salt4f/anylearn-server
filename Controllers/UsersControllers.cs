using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

            var client = _config.GetSection("Linkedin").GetValue<string>("CliendId");
            var secret = _config.GetSection("Linkedin").GetValue<string>("Secret");


            var b = new UriBuilder("https://www.linkedin.com/oauth/v2/accessToken");
            b.Port = -1;
            var q = HttpUtility.ParseQueryString(string.Empty);
            q["grant_type"] = "authorization_code";
            q["code"] = code;
            q["redirect_uri"] = "http://localhost:3000/linkedin";
            q["cliend_id"] = client;
            q["client_secret"] = secret;

            b.Query = q.ToString();

            _logger.LogInformation(b.ToString());
            var res = await _httpClient.GetAsync(b.ToString());
            res.EnsureSuccessStatusCode();
            var body = await res.Content.ReadAsStringAsync();
            var json = JObject.Parse(body);
            string? token = json.GetValue("access_token")!.Value<string>();

            string url = "https://api.linkedin.com/v2/me?projection=(localizedFirstName,localizedLastName,profilePicture(displayImage~:playableStreams))";
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

        [HttpPost, Route("/users/")]
        public async Task<User?> PostAsync([FromBody] RegisterInfo info)
        {
            var user = new User()
            {
                Email = info.Email!,
                Linkedin = false,
                Name = info.Name!,
                Surname = info.Surname!,
                Password = info.Password,
                Photo = null,
                Token = Convert.ToBase64String(Encoding.ASCII.GetBytes(info.Email!))
            };

            _context.Users!.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public class RegisterInfo
        {
            public string? Name { get; set; }
            public string? Surname { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

    }
}

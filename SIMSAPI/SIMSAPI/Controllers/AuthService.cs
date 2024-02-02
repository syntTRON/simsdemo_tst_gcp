using Microsoft.AspNetCore.Mvc;

namespace SIMSAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthService : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet]
        [Route("check")]
        public IActionResult Get(string username, string token)
        {
            if (new RedisDB().CheckToken(username, token) == true)
            {
                return Ok();
            }

            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public string Post(string username, string password)
        {
            //TODO user-management
            if ((username == "admin" && password == "admin") || (username == "user" && password == "user"))
            {
                string token = generateToken(username, password);
                new RedisDB().StoreToken(username, token);
                return token;
            }
            else
            {
                return "";
            }
        }

        private string generateToken(string username, string password)
        {
            //HACK generate cool Token ;-) -> base64 is not encryption!
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + password + DateTime.Now.ToString()));
        }
    }
}
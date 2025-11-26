using CegautokAP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CegautokAP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        
        [HttpGet("GetSalt")]
        public IActionResult GetSalt(string username)
        {
            try
            {
                using (var context = new FlottaContext())
                {
                    if (context.Users.Any(u => u.LoginName == username))
                    {
                        return Ok(context.Users.First(u => u.LoginName == username).Salt);
                    }
                    else
                    {
                        return BadRequest("Nincs ilyen felhasználónév!");
                    }
                }
            }catch (Exception ex)
            {
                return BadRequest("hiba: "+ ex.Message);
            }
            
        }
    }
}

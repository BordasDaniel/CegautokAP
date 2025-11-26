using CegautokAP.DTO;
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

        [HttpGet("Login")]
        public IActionResult Login(LoginDTO loginDTO)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    string doubleHash = Program.CreateSHA256(loginDTO.Hash);
                    User user = context.Users.FirstOrDefault(u => u.LoginName == loginDTO.LoginName && u.Hash == doubleHash && u.Active);
                    if (user == null)
                    {
                        return NotFound("Nincs megfelelő felhasználó! A belépés sikertelen!");
                    }
                    return Ok("Sikeres belépés!");

                } catch(Exception ex)
                {
                    return BadRequest($"Hiba a bejelentkezés során: {ex.Message}");
                }
            }
        }
    }
}

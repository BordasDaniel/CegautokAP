using CegautokAP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CegautokAP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("Users")]
        public IActionResult GetAllUsers()
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    List<User> users = [.. context.Users];

                    return Ok(users);

                }
                catch (Exception ex)
                {
                    return BadRequest(new User()
                    {
                        Id = -1,
                        Name = $"Hiba történt: {ex.Message}",
                        Address = null
                    });
                }

            }
        }

        [HttpGet("UserById")]
        public IActionResult GetUserById(int Id)
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == Id);
                    if (user is User)
                    {
                        return Ok(user);
                    }
                    else
                    {
                        return BadRequest(new User()
                        {
                            Id = -1,
                            Name = $"Hiba történt: Nincs ilyen azonosítójú felhasználó",
                            Address = null
                        });
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(new User()
                    {
                        Id = -1,
                        Name = $"Hiba történt: {ex.Message}",
                        Address = null
                    });
                }
            }
        }

    }
}

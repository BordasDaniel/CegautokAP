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

        [HttpGet("UserById/{Id}")]
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


        [HttpPost("NewUser")]
        public IActionResult AddNewUser(User user)
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    user.Image ??= "..\\imgs\\defaultUser.jpg";
                    
                    context.Add(user);
                    context.SaveChanges();
                    return Ok("Sikeres rögzítés");

                } catch (Exception ex)
                {
                    return BadRequest($"Hiba történt a felvétel során: {ex.Message}");
                }
            }
        }

        [HttpPut("ModifyUser")]
        public IActionResult ModifyUser(User user)
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    if (context.Users.Contains(user))
                    {
                        context.Update(user);
                        context.SaveChanges();
                        return Ok("Sikeres módosítás!");
                    }
                    else
                    {
                        return BadRequest("Nincs ilyen felhasználó!");
                    }
                } catch (Exception ex)
                {
                    return BadRequest($"Hiba a módosítás során: {ex.Message}");
                }
            }
        }

        [HttpDelete("DelUser/{Id}")]
        public IActionResult DeleteUser(int Id)
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    if (context.Users.Select(u => u.Id).Contains(Id))
                    {
                        User del = context.Users.FirstOrDefault(u => u.Id == Id); 
                        context.Remove(del);
                        context.SaveChanges();
                        return Ok("Sikeres törlés!");
                    }
                    else
                    {
                        return BadRequest("Nincs ilyen felhasználó!");
                    }

                } catch (Exception ex)
                {
                    return BadRequest($"Hiba a törlés közben: {ex.Message}");
                }
            }
        }
    }
}

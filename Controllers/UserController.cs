using CegautokAP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CegautokAP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly FlottaContext _context;

        public UserController(FlottaContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "10, 2")]
        [HttpGet("Users")]
        public IActionResult GetAllUsers()
        {
                try
                {
                    List<User> users = [.. _context.Users];

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

        
        [HttpGet("UserById/{Id}")]
        public IActionResult GetUserById(int Id)
        {
                try
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == Id);
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


        [HttpPost("NewUser")]
        public IActionResult AddNewUser(User user)
        {
                try
                {
                    user.Image ??= @"defaultUser.jpg";
                    
                    _context.Add(user);
                    _context.SaveChanges();
                    return Ok("Sikeres rögzítés");

                } catch (Exception ex)
                {
                    return BadRequest($"Hiba történt a felvétel során: {ex.Message}");
                }
        }

        [HttpPut("ModifyUser")]
        public IActionResult ModifyUser(User user)
        {
                try
                {
                    if (_context.Users.Contains(user))
                    {
                        _context.Update(user);
                        _context.SaveChanges();
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

        [HttpDelete("DelUser/{Id}")]
        public IActionResult DeleteUser(int Id)
        {
                try
                {
                    if (_context.Users.Select(u => u.Id).Contains(Id))
                    {
                        User del = _context.Users.FirstOrDefault(u => u.Id == Id); 
                        _context.Remove(del);
                        _context.SaveChanges();
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

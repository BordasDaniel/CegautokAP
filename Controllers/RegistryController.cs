using CegautokAP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CegautokAP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistryController : ControllerBase
    {
        private readonly FlottaContext _context;
        public RegistryController(FlottaContext context) {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Registry(User user)
        {
            try
            {
                if (_context.Users.FirstOrDefault(u => u.LoginName == user.LoginName) != null)
                {
                    return BadRequest("Felhasználónév már foglalt.");
                }
                if (_context.Users.FirstOrDefault(u => u.Email == user.Email) != null)
                {
                    return BadRequest("Ezzel az emaillel már regisztáltak");
                }

                user.Active = false;
                user.Permission = 1;
                user.Hash = Program.CreateSHA256(user.Hash);

               await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                Program.SendEmail(user.Email, "Regisztráció megerősítése", $"{user.Name} Erősítsd meg: \n\nhttps://localhost:7087/api/Registry?felhasznaloNev={user.LoginName}&email={user.Email} ");
                // https://localhost:7087/api/Registry?felhasznaloNev=elsoerik&email=bordas.daniel0124%40gmail.com
                return Ok("Sikeres regisztráció, erősítse meg a megadott emailre kapott linken keresztül.");
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmRegistry(string felhasznaloNev, string email)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.LoginName == felhasznaloNev && u.Email == email);
                if (user != null)
                {
                    user.Active = true;
                    user.Permission = 2;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return Ok("Sikeres regisztráció");
                }
                else
                {
                    return BadRequest("Hibás adatok.");
                }
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

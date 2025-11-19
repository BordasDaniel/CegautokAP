using CegautokAP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CegautokAP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KikuldetesController : ControllerBase
    {
        [HttpGet("Kikuldtes")]
        public IActionResult GetAllKikuldtes()
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    List<Kikuldte> kikuldtes = [.. context.Kikuldtes];

                    return Ok(kikuldtes);

                }
                catch (Exception ex)
                {
                    return BadRequest(new Kikuldte()
                    {
                        Id = -1,
                        Celja = $"Hiba történt: {ex.Message}",
                        Cim = "Hiba",
                        Kezdes = DateTime.Now,
                        Befejezes = DateTime.Now
                    });
                }

            }
        }

        [HttpGet("KikuldteById/{Id}")]
        public IActionResult GetKikuldteById(int Id)
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    var kikuldte = context.Kikuldtes.FirstOrDefault(u => u.Id == Id);
                    if (kikuldte is Kikuldte)
                    {
                        return Ok(kikuldte);
                    }
                    else
                    {
                        return BadRequest("Nincs ilyen id!");
                        
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(new Kikuldte()
                    {
                        Id = -1,
                        Celja = $"Hiba történt: {ex.Message}",
                        Cim = "Hiba",
                        Kezdes = DateTime.Now,
                        Befejezes = DateTime.Now
                    });
                }
            }
        }


        [HttpPost("NewKikuldte")]
        public IActionResult AddNewKikuldte(Kikuldte kikuldte)
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    
                    context.Add(kikuldte);
                    context.SaveChanges();
                    return Ok("Sikeres rögzítés");

                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba történt a felvétel során: {ex.Message}");
                }
            }
        }

        [HttpPut("ModifyKikuldte")]
        public IActionResult ModifyKikuldte(Kikuldte kikuldte)
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    if (context.Kikuldtes.Contains(kikuldte))
                    {
                        context.Update(kikuldte);
                        context.SaveChanges();
                        return Ok("Sikeres módosítás!");
                    }
                    else
                    {
                        return BadRequest("Nincs ilyen kikuldte!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a módosítás során: {ex.Message}");
                }
            }
        }

        [HttpDelete("DelKikuldte/{Id}")]
        public IActionResult DeleteKikuldte(int Id)
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    if (context.Kikuldtes.Select(u => u.Id).Contains(Id))
                    {
                        Kikuldte del = context.Kikuldtes.FirstOrDefault(u => u.Id == Id);
                        context.Remove(del);
                        context.SaveChanges();
                        return Ok("Sikeres törlés!");
                    }
                    else
                    {
                        return BadRequest("Nincs ilyen kikuldte!");
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a törlés közben: {ex.Message}");
                }
            }
        }
    }
}

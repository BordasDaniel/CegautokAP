using CegautokAP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CegautokAP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GepjarmuController : ControllerBase
    {
        [HttpGet("Gepjarmus")]
        public IActionResult GetAllGepjarmus()
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    List<Gepjarmu> gepjarmus = context.Gepjarmus.ToList();
                    return Ok(gepjarmus);

                }
                catch (Exception ex)
                {
                    return BadRequest(new Gepjarmu()
                    {
                        Id = -1,
                        Rendszam = $"Hiba történt: {ex.Message}".Substring(0, 16),
                        Tipus = "hiba",
                        Ulesek = -1
                    });
                }

            }
        }

        [HttpGet("GepjarmuById/{Id}")]
        public IActionResult GetGepjarmuById(int Id)
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    var gepjarmu = context.Gepjarmus.FirstOrDefault(u => u.Id == Id);
                    if (gepjarmu is Gepjarmu)
                    {
                        return Ok(gepjarmu);
                    }
                    else
                    {
                        return BadRequest("Nincs ilyen gépjármű");
                         
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(new Gepjarmu()
                    {
                        Id = -1,
                        Rendszam = $"Hiba történt: {ex.Message}".Substring(0, 16),
                        Tipus = "hiba",
                        Ulesek = -1
                    });
                }
            }
        }


        [HttpPost("NewGepjarmu")]
        public IActionResult AddNewGepjarmu(Gepjarmu gepjarmu)
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    
                    context.Add(gepjarmu);
                    context.SaveChanges();
                    return Ok("Sikeres rögzítés");

                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba történt a felvétel során: {ex.Message}");
                }
            }
        }

        [HttpPut("ModifyGepjarmu")]
        public IActionResult ModifyGepjarmu(Gepjarmu gepjarmu)
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    if (context.Gepjarmus.Contains(gepjarmu))
                    {
                        context.Update(gepjarmu);
                        context.SaveChanges();
                        return Ok("Sikeres módosítás!");
                    }
                    else
                    {
                        return BadRequest("Nincs ilyen gépjűrmű!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a módosítás során: {ex.Message}");
                }
            }
        }

        [HttpDelete("DelGepjarmu/{Id}")]
        public IActionResult DeleteGepjarmu(int Id)
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    if (context.Gepjarmus.Select(u => u.Id).Contains(Id))
                    {
                        Gepjarmu del = context.Gepjarmus.FirstOrDefault(u => u.Id == Id);
                        context.Remove(del);
                        context.SaveChanges();
                        return Ok("Sikeres törlés!");
                    }
                    else
                    {
                        return BadRequest("Nincs ilyen gépjármű!");
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

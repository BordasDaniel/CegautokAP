using CegautokAP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CegautokAP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GepjarmuController : ControllerBase
    {
        private readonly FlottaContext _context;

        public GepjarmuController(FlottaContext context)
        {
            _context = context;
        }


        [Authorize]
        [HttpGet("Gepjarmus")]
        public IActionResult GetAllGepjarmus()
        {
                try
                {
                    List<Gepjarmu> gepjarmus = _context.Gepjarmus.ToList();
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

        [HttpGet("GepjarmuById/{Id}")]
        public IActionResult GetGepjarmuById(int Id)
        {
                try
                {
                    var gepjarmu = _context.Gepjarmus.FirstOrDefault(u => u.Id == Id);
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


        [HttpPost("NewGepjarmu")]
        public IActionResult AddNewGepjarmu(Gepjarmu gepjarmu)
        {
                try
                {
                    
                    _context.Add(gepjarmu);
                    _context.SaveChanges();
                    return Ok("Sikeres rögzítés");

                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba történt a felvétel során: {ex.Message}");
                }
        }

        [HttpPut("ModifyGepjarmu")]
        public IActionResult ModifyGepjarmu(Gepjarmu gepjarmu)
        {
                try
                {
                    if (_context.Gepjarmus.Contains(gepjarmu))
                    {
                        _context.Update(gepjarmu);
                        _context.SaveChanges();
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

        [HttpDelete("DelGepjarmu/{Id}")]
        public IActionResult DeleteGepjarmu(int Id)
        {
                try
                {
                    if (_context.Gepjarmus.Select(u => u.Id).Contains(Id))
                    {
                        Gepjarmu del = _context.Gepjarmus.FirstOrDefault(u => u.Id == Id);
                        _context.Remove(del);
                        _context.SaveChanges();
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

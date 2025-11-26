using CegautokAP.DTO;
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
        [HttpGet("Jarmuvek")]
        public IActionResult GetJarmuvekDTO()
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    List<KikuldJarmuDTO> valasz = [.. context.Kikuldottjarmus
                        .Include(x => x.Kikuldetes)
                        .Include(x => x.Gepjarmu)
                        .Select(x => new KikuldJarmuDTO()
                    {
                        Cim = x.Kikuldetes.Cim,
                        Datum = x.Kikuldetes.Kezdes,
                        Rendszam = x.Gepjarmu.Rendszam
                    })];

                    return Ok(valasz);

                } catch(Exception ex)
                {
                    return BadRequest(new KikuldJarmuDTO()
                    {
                        Cim = ex.Message,
                        Datum = DateTime.Now,
                        Rendszam = "hiba"
                    });
                }
            }
        }

        [HttpGet("Jarmuvek/{Id}")]
        public IActionResult GetJarmuvekDTO(int Id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    Kikuldottjarmu jarmu = context.Kikuldottjarmus
                     .Include(x => x.Kikuldetes)
                     .Include(x => x.Gepjarmu)
                     .FirstOrDefault(x => x.Id == Id);


                    if (jarmu is Kikuldottjarmu)
                    {
                        KikuldJarmuDTO dto = new()
                        {
                            Cim = jarmu.Kikuldetes.Cim,
                            Datum = jarmu.Kikuldetes.Befejezes,
                            Rendszam = jarmu.Gepjarmu.Rendszam
                        };
                        return Ok(dto);
                    }
                    else return BadRequest("Nincs ilyen azonosító!");
                }

                catch (Exception ex)
                {
                    return BadRequest(new KikuldJarmuDTO()
                    {
                        Cim = ex.Message,
                        Datum = DateTime.Now,
                        Rendszam = "hiba"
                    });
                }
            }
        }

        [HttpGet("Jarmu/{Id}/Hasznalat")]
        public IActionResult GetHasznalat(int Id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    Kikuldottjarmu jarmu = context.Kikuldottjarmus
                     .Include(x => x.Kikuldetes)
                     .Include(x => x.Gepjarmu)
                     .FirstOrDefault(x => x.Id == Id);


                    if (jarmu is Kikuldottjarmu)
                    {
                        HasznalatDTO dto = new()
                        {
                            Id = Id,
                            Rendszam = jarmu.Gepjarmu.Rendszam,
                            Kezdes = jarmu.Kikuldetes.Kezdes,
                            Befejezes = jarmu.Kikuldetes.Befejezes
                        };
                        return Ok(dto);
                    }
                    else return BadRequest("Nincs ilyen azonosító!");

                }
                catch (Exception ex)
                {
                    return BadRequest(new HasznalatDTO()
                    {
                        Id = -1,
                        Rendszam = ex.Message,
                        Kezdes = DateTime.Now,
                        Befejezes = DateTime.Now
                    });
                }
            }
        }

        [HttpGet("Jarmu/Sofor")]
        public IActionResult GetSoforDTO()
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    List<SoforDTO> dto = [.. context.Kikuldottjarmus
                         .Include(x => x.Gepjarmu)
                         .Include(x => x.SoforNavigation)
                         .GroupBy(x => new
                         {
                             rsz = x.Gepjarmu.Rendszam,
                             sof = x.SoforNavigation.Name
                         }).Select(elem => new SoforDTO()
                         {
                             Rendszam = elem.Key.rsz,
                             SoforNev = elem.Key.sof,
                             Darab = elem.Count()
                         })];

                    return Ok(dto);



                } catch (Exception ex)
                {
                    return BadRequest(new SoforDTO()
                    {
                        SoforNev = "Hiba",
                        Darab = -1,
                        Rendszam = ex.Message
                    });
                }
            }
        }
        /*
         KikuldetesControllerben kellene megállpaítani, hogy adott Idű kiküldetésnél ki volt a sofőr
         */

        [HttpGet("KikuldetesSoforje/{Id}")]
        public IActionResult KikuldetesSoforje(int Id)
        {
            try
            {
                using(var context = new FlottaContext())
                {
                    KikuldetesSoforDTO dto = context.Kikuldottjarmus
                        .Include(x => x.Sofor)
                        .Where(x => x.Kikuldetes.Id == Id)
                        .Select(x => new KikuldetesSoforDTO()
                        {
                            KikuldetesId = Id,
                            SoforNev = x.SoforNavigation.Name
                        }).FirstOrDefault();


                    if (dto is KikuldetesSoforDTO) return Ok(dto);
                    else return BadRequest("Nincs ilyen azonosítóval!");
                }

            } catch (Exception ex)
            {
                return BadRequest("bottomtext "+ ex.Message);
            }
        }


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

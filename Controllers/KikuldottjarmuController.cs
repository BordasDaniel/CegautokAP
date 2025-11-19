using CegautokAP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CegautokAP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KikuldottjarmuController : ControllerBase
    {
        [HttpGet("KikuldottJarmuvek")]
        public IActionResult GetAllKikuldottJarmuvek()
        {
            using (var context = new CegautokAP.Models.FlottaContext())
            {
                try
                {
                    List<Kikuldottjarmu> kikuldottjarmus = [.. context.Kikuldottjarmus
                .Include(k => k.Gepjarmu)
                .Include(k => k.Kikuldetes)
                .Include(k => k.SoforNavigation)];
                    return Ok(kikuldottjarmus);

                } catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}

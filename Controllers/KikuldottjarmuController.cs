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
        private readonly FlottaContext _context;

        public KikuldottjarmuController(FlottaContext context)
        {
            _context = context;
        }

        [HttpGet("KikuldottJarmuvek")]
        public IActionResult GetAllKikuldottJarmuvek()
        {
                try
                {
                    List<Kikuldottjarmu> kikuldottjarmus = [.. _context.Kikuldottjarmus
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

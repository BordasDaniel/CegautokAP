using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CegautokAP.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        [HttpPost]
        [Route("ToFtpServer")]
        public async Task<IActionResult> FileUploadToFtp()
        {
            try
            {
                var HttpRequest = Request.Form;
                var postedFile = HttpRequest.Files[0];
                string fileName = postedFile.FileName;
                Stream fileStream = postedFile.OpenReadStream();
                string valasz = await Program.UploadToFtpServer(fileStream, fileName);
                return Ok(valasz);

            } catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}") ;
            }
        }

    }
}

using CegautokAP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using System.IO;

namespace CegautokAP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupRestoreController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly FlottaContext _context;

        public BackupRestoreController(IWebHostEnvironment env, FlottaContext context)
        {
            _context = context;
            _env = env;
        }

        [Route("Backup")]
        [HttpPost]
        //[Authorize("admin")]
        public async Task<IActionResult> SQLBackup(string fileName)
        {
            string sqlDataSource = _context.Database.GetConnectionString();
            MySqlCommand sqlCommand = new();
            MySqlBackup backup = new();
            using (MySqlConnection  connection = new MySqlConnection(sqlDataSource))
            {
                try
                {
                    sqlCommand.Connection = connection;
                    connection.Open();
                    var filePath = "SQLBackupRestore/" + fileName;
                    backup.ExportToFile(filePath);
                    connection.Close();
                    if (System.IO.File.Exists(filePath))
                    {
                        var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
                        return File(bytes, "text/plain", Path.GetFileName(filePath));
                    }
                    else
                    {
                        string hibauzenet = "Nincs ilyen fájl!";
                        byte[] hiba = new byte[hibauzenet.Length];
                        for (int i = 0; i < hiba.Length; i++)
                        {
                            hiba[i] = Convert.ToByte(hibauzenet[i]); 
                        }
                        return File(hiba, "text/plain", "Error.txt");
                    }


                } catch(Exception ex)
                {
                    return new JsonResult(ex);
                }
            }
        }

        [Route("Restore")]
        [HttpPost]
        [Authorize]
        public JsonResult SQLRestore()
        {
            try
            {
                string sqlDatabase = _context.Database.GetConnectionString();
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string fileName = postedFile.FileName;
                var filePath = _env.ContentRootPath + "/SQLBackupRestore" + fileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                MySqlCommand command = new();
                MySqlBackup restore = new(command);
                using (MySqlConnection connection = new MySqlConnection(sqlDatabase))
                {
                    try
                    {
                        command.Connection = connection;
                        connection.Open();
                        restore.ImportFromFile(fileName);
                        System.IO.File.Delete(fileName);
                        return new JsonResult("A visszaállítás sikeres");
                    }
                    catch (Exception ex)
                    {
                        return new JsonResult(ex);
                    }
                }

            } catch(Exception)
            {
                return new JsonResult("Hiba a fájl feltöltése során.");
            }
        }
    }
}

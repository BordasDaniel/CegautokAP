using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using CegautokAP.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using FluentFTP;

namespace CegautokAP
{
    public class Program
    {
        private static FtpSettings ftpSettings = new();

        private static MailSettings mailSettings = new();
        public static async Task<string> UploadToFtpServer(Stream filestream, string fileName)
        {
            try
            {
                NetworkCredential credential = new(ftpSettings.FtpUser, ftpSettings.FtpUser);
                await using(IAsyncFtpClient client = new AsyncFtpClient(ftpSettings.Host, credential))
                {
                    client.Config.DataConnectionType = FtpDataConnectionType.AutoPassive;
                    await client.Connect();
                    await client.UploadStream(filestream, ftpSettings.SubFolder + fileName);
                    await client.Disconnect();
                    return fileName;
                }

            } catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static async Task SendEmail(string mailAddressTo, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(mailSettings.SmtpServer);
            mail.From = new MailAddress(mailSettings.SenderEmail);
            mail.To.Add(mailAddressTo);
            mail.Subject = subject;
            mail.Body = body;

            /*System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment("");
            mail.Attachments.Add(attachment);*/

            SmtpServer.Port = mailSettings.Port;
            SmtpServer.Credentials = new System.Net.NetworkCredential(mailSettings.SenderEmail, mailSettings.SenderPassword);

            SmtpServer.EnableSsl = true;

            await SmtpServer.SendMailAsync(mail);

        }



        public static string GenerateSalt()
        {
            Random random = new Random();
            string karakterek = "qwertzuiopı˙asdfghjklÈ·˚ÌyxcvbnmQWERTZUIOP’⁄ASDFGHJKL…¡€ÕYXCVBNM÷‹”ˆ¸Û1234567890";
            string salt = "";
            for (int i = 0; i < 64; i++)
            {
                salt += karakterek[random.Next(karakterek.Length)];
            }
            return salt;
        }

        public static string CreateSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sbuilder = new();

                for (int i = 0; i < data.Length; i++)
                {
                    sbuilder.Append(data[i].ToString("x2"));
                }

                return sbuilder.ToString();
            }

        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FlottaContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("FlottaConnection")));

            builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);



            // Mail settings
            builder.Configuration.GetSection("MailServices").Bind(mailSettings);
            builder.Services.AddSingleton(mailSettings);

            // FTP settings
            builder.Configuration.GetSection("FtpSettings").Bind(ftpSettings);
            builder.Services.AddSingleton(ftpSettings);

            // JWT Settings
            var jwtSettings = new JwtSettings();
            builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
            builder.Services.AddSingleton(jwtSettings);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey))
                };
            });


            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddCors(c => { c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                    
                });
            });
            
           


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.MapControllers();

            app.Run();
        }
    }
}

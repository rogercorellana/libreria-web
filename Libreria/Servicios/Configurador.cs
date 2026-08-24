using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Configuration;


namespace Servicios
{
    public class Configurador
    {
        public static string GenerarClave()
        {
            string clave = Guid.NewGuid().ToString("N").Substring(0, 6);
            return clave;
        }

        public static bool EnviarCorreo(string correo,string asunto,string mensaje)
        {
            bool resultado = false;
            try
            {
                string correoSistema = ConfigurationManager.AppSettings["CorreoSistema"];
                string claveCorreo = ConfigurationManager.AppSettings["ClaveCorreoSistema"];
                MailMessage mail = new MailMessage();
                mail.To.Add(correo);
                mail.From = new MailAddress(correoSistema);
                mail.Subject = asunto;
                mail.Body = mensaje;
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Credentials = new NetworkCredential(correoSistema,claveCorreo);
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }

                resultado = true;
            }
            catch
            {
                resultado = false;
            }
            return resultado;
        }
    }
}



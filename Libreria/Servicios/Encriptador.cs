using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Servicios
{
    public class Encriptador
    {
        //Encriptacion SHA256

        public static string ConvertirSHA256(string texto)
        {
            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding e = Encoding.UTF8;
                byte[] resultado = hash.ComputeHash(e.GetBytes(texto));

                foreach (byte b in resultado)
                    sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}

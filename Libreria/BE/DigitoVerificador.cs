using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace BE
{
    public class DigitoVerificador
    {
        public static string Hashear(string valor)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(valor));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        // DVH mixto: acepta strings y decimals
        // Concatena todos los valores en un string y lo hashea
        public static string CalcularDVH(params object[] valores)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object valor in valores)
            {
                if (valor == null)
                    sb.Append("null");
                else
                    sb.Append(valor.ToString().Trim().ToLowerInvariant());
                sb.Append("|");
            }
            return Hashear(sb.ToString());
        }

        public static bool VerificarDVH(string dvhAlmacenado, params object[] valores)
        {
            return dvhAlmacenado == CalcularDVH(valores);
        }

        public static string CalcularDVV(decimal[] valores)
        {
            decimal suma = 0;
            foreach (decimal valor in valores)
                suma += valor;
            return Hashear(suma.ToString());
        }

        public static bool VerificarDVV(string dvvAlmacenado, decimal[] valores)
        {
            return dvvAlmacenado == CalcularDVV(valores);
        }
    }
}
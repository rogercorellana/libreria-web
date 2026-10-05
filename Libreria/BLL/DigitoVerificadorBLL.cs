using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DAL;
using BE;

namespace BLL
{
    public class DigitoVerificadorBLL
    {
        private DigitoVerificadorDAL dvDAL = new DigitoVerificadorDAL();

        public void RecalcularDVV(string nombreTabla, string nombreColumna)
        {
            decimal[] valores = dvDAL.ObtenerValoresColumna(nombreTabla, nombreColumna);
            string nuevoDVV = DigitoVerificador.CalcularDVV(valores);
            dvDAL.ActualizarDVV(nombreTabla, nombreColumna, nuevoDVV);
        }

        public bool VerificarDVV(string nombreTabla, string nombreColumna)
        {
            decimal[] valores = dvDAL.ObtenerValoresColumna(nombreTabla, nombreColumna);
            string dvvAlmacenado = dvDAL.ObtenerDVV(nombreTabla, nombreColumna);

            if (dvvAlmacenado == null)
                return false;

            return DigitoVerificador.VerificarDVV(dvvAlmacenado, valores);
        }
    }
}
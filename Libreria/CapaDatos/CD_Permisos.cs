using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Permisos
    {
        public List<Permiso> ListarPorRol(int idRol)
        {
            List<Permiso> lista = new List<Permiso>();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string consulta = @"
                        SELECT
                            id_permiso,
                            id_rol,
                            menu
                        FROM dbo.Permiso
                        WHERE id_rol = @idRol
                        ORDER BY id_permiso;";

                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@idRol", SqlDbType.Int).Value = idRol;

                        conexion.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lista.Add(new Permiso
                                {
                                    IdPermiso = Convert.ToInt32(dr["id_permiso"]),
                                    IdRol = Convert.ToInt32(dr["id_rol"]),
                                    Menu = dr["menu"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Permiso>();
            }

            return lista;
        }
    }
}

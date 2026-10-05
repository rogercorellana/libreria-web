using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BE;



namespace DAL
{
    public class PlatoDAL
    {
        private AuditoriaDAL auditoriaDAL = new AuditoriaDAL();

        private string CalcularDVH(Plato plato)
        {
            return DigitoVerificador.CalcularDVH(
                plato.Nombre,
                plato.Descripcion ?? "",
                plato.PrecioVenta,
                plato.ID_Categoria,
                plato.Disponible ? 1 : 0
            );
        }

        private Plato MapearPlato(SqlDataReader reader)
        {
            return new Plato
            {
                ID_Plato = Convert.ToInt32(reader["ID_Plato"]),
                Nombre = reader["Nombre"].ToString(),
                Descripcion = reader["Descripcion"].ToString(),
                PrecioVenta = Convert.ToDecimal(reader["PrecioVenta"]),
                ID_Categoria = Convert.ToInt32(reader["ID_Categoria"]),
                Disponible = Convert.ToBoolean(reader["Disponible"]),
                DVH = reader["DVH"].ToString()
            };
        }

        public List<Plato> ObtenerTodos()
        {
            List<Plato> lista = new List<Plato>();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"SELECT P.*, C.Nombre AS NombreCategoria 
                                 FROM PLATO P 
                                 INNER JOIN CATEGORIA C ON P.ID_Categoria = C.ID_Categoria";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Plato p = MapearPlato(reader);
                    p.NombreCategoria = reader["NombreCategoria"].ToString();
                    lista.Add(p);
                }
            }
            return lista;
        }

        public List<Plato> ObtenerDisponibles()
        {
            List<Plato> lista = new List<Plato>();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"SELECT P.*, C.Nombre AS NombreCategoria 
                                 FROM PLATO P 
                                 INNER JOIN CATEGORIA C ON P.ID_Categoria = C.ID_Categoria
                                 WHERE P.Disponible = 1";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Plato p = MapearPlato(reader);
                    p.NombreCategoria = reader["NombreCategoria"].ToString();
                    lista.Add(p);
                }
            }
            return lista;
        }

        public Plato ObtenerPorID(int idPlato)
        {
            Plato plato = null;
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM PLATO WHERE ID_Plato = @ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", idPlato);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    plato = MapearPlato(reader);
            }
            return plato;
        }

        public bool Agregar(Plato plato)
        {
            try
            {
                string dvh = CalcularDVH(plato);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"INSERT INTO PLATO 
                        (Nombre, Descripcion, PrecioVenta, ID_Categoria, Disponible, DVH)
                        VALUES 
                        (@Nombre, @Descripcion, @PrecioVenta, @Categoria, @Disponible, @DVH)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Nombre", plato.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", plato.Descripcion ?? "");
                    cmd.Parameters.AddWithValue("@PrecioVenta", plato.PrecioVenta);
                    cmd.Parameters.AddWithValue("@Categoria", plato.ID_Categoria);
                    cmd.Parameters.AddWithValue("@Disponible", plato.Disponible);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public bool Modificar(Plato plato)
        {
            try
            {
                Plato anterior = ObtenerPorID(plato.ID_Plato);
                if (anterior != null)
                {
                    auditoriaDAL.GuardarValorAnterior("PLATO", plato.ID_Plato, "PrecioVenta", anterior.PrecioVenta.ToString());
                    auditoriaDAL.GuardarValorAnterior("PLATO", plato.ID_Plato, "Disponible", anterior.Disponible.ToString());
                    auditoriaDAL.GuardarValorAnterior("PLATO", plato.ID_Plato, "Nombre", anterior.Nombre);
                }

                string dvh = CalcularDVH(plato);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"UPDATE PLATO SET
                        Nombre       = @Nombre,
                        Descripcion  = @Descripcion,
                        PrecioVenta  = @PrecioVenta,
                        ID_Categoria = @Categoria,
                        Disponible   = @Disponible,
                        DVH          = @DVH
                        WHERE ID_Plato = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Nombre", plato.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", plato.Descripcion ?? "");
                    cmd.Parameters.AddWithValue("@PrecioVenta", plato.PrecioVenta);
                    cmd.Parameters.AddWithValue("@Categoria", plato.ID_Categoria);
                    cmd.Parameters.AddWithValue("@Disponible", plato.Disponible);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@ID", plato.ID_Plato);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public bool CambiarDisponibilidad(int idPlato, bool disponible)
        {
            try
            {
                Plato plato = ObtenerPorID(idPlato);
                auditoriaDAL.GuardarValorAnterior("PLATO", idPlato, "Disponible", plato.Disponible.ToString());
                plato.Disponible = disponible;
                string dvh = CalcularDVH(plato);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"UPDATE PLATO SET 
                        Disponible = @Disponible,
                        DVH        = @DVH
                        WHERE ID_Plato = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Disponible", disponible);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@ID", idPlato);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch { return false; }
        }

        public void RecalcularTodosDVH()
        {
            List<Plato> lista = ObtenerTodos();
            foreach (var p in lista)
            {
                string dvh = CalcularDVH(p);
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = "UPDATE PLATO SET DVH = @DVH WHERE ID_Plato = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@ID", p.ID_Plato);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

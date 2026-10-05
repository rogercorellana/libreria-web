using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BE;

namespace DAL
{
    public class PlatoIngredienteDAL
    {
        public List<PlatoIngrediente> ObtenerPorPlato(int idPlato)
        {
            List<PlatoIngrediente> lista = new List<PlatoIngrediente>();

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"SELECT PI.*, I.Nombre AS NombreIngrediente, I.UnidadMedida
                                 FROM PLATO_INGREDIENTE PI
                                 INNER JOIN INGREDIENTE I ON PI.ID_Ingrediente = I.ID_Ingrediente
                                 WHERE PI.ID_Plato = @IDPlato";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IDPlato", idPlato);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new PlatoIngrediente
                    {
                        ID_PlatoIngrediente = Convert.ToInt32(reader["ID_PlatoIngrediente"]),
                        ID_Plato = Convert.ToInt32(reader["ID_Plato"]),
                        ID_Ingrediente = Convert.ToInt32(reader["ID_Ingrediente"]),
                        NombreIngrediente = reader["NombreIngrediente"].ToString(),
                        UnidadMedida = reader["UnidadMedida"].ToString(),
                        CantidadUsada = Convert.ToDecimal(reader["CantidadUsada"])
                    });
                }
            }

            return lista;
        }

        public bool Agregar(PlatoIngrediente platoIngrediente)
        {
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = @"INSERT INTO PLATO_INGREDIENTE
                        (ID_Plato, ID_Ingrediente, CantidadUsada)
                        VALUES
                        (@Plato, @Ingrediente, @Cantidad)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Plato", platoIngrediente.ID_Plato);
                    cmd.Parameters.AddWithValue("@Ingrediente", platoIngrediente.ID_Ingrediente);
                    cmd.Parameters.AddWithValue("@Cantidad", platoIngrediente.CantidadUsada);
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch { return false; }
        }

        public bool Eliminar(int idPlatoIngrediente)
        {
            try
            {
                using (SqlConnection con = Conexion.ObtenerConexion())
                {
                    string query = "DELETE FROM PLATO_INGREDIENTE WHERE ID_PlatoIngrediente = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ID", idPlatoIngrediente);
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch { return false; }
        }

        public void DescontarStock(int idPedido)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                string query = @"UPDATE I SET I.StockActual = I.StockActual - (DP.Cantidad * PI.CantidadUsada)
                                 FROM INGREDIENTE I
                                 INNER JOIN PLATO_INGREDIENTE PI ON I.ID_Ingrediente = PI.ID_Ingrediente
                                 INNER JOIN DETALLE_PEDIDO DP    ON PI.ID_Plato = DP.ID_Plato
                                 WHERE DP.ID_Pedido = @IDPedido";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IDPedido", idPedido);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

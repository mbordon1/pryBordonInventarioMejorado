using System;
using System.Data.SqlClient;

namespace pryBordonInventarioMejorado
{
    internal class clsUsuarios
    {
        private string conexionBD = "Server=.;Database=Comercio;Trusted_Connection=True;";

        public clsUsuario VerificarCredenciales(string nombreUsuario, string contrasena)
        {
            using (SqlConnection conexion = new SqlConnection(conexionBD))
            {
                conexion.Open();

                string consulta = "SELECT * FROM Usuarios WHERE Usuario = @Usuario AND Contrasena = @Contrasena";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@Usuario", nombreUsuario);
                comando.Parameters.AddWithValue("@Contrasena", contrasena);

                SqlDataReader reader = comando.ExecuteReader();
                if (reader.Read())
                {
                    bool estado = Convert.ToBoolean(reader["Estado"]);

                    if (!estado)
                    {
                        // Usuario bloqueado
                        return null;
                    }

                    clsUsuario usuario = new clsUsuario
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        NombreUsuario = reader["Usuario"].ToString(),
                        Contrasena = reader["Contrasena"].ToString(),
                        Rol = reader["Rol"].ToString(),
                        Estado = estado,
                        FechaUltimaConexion = reader["FechaUltimaConexion"] != DBNull.Value
                            ? Convert.ToDateTime(reader["FechaUltimaConexion"])
                            : (DateTime?)null
                    };
                    return usuario;
                }

                return null;
            }
        }

        public void BloquearUsuario(string nombreUsuario)
        {
            using (SqlConnection conexion = new SqlConnection(conexionBD))
            {
                conexion.Open();
                string consulta = "UPDATE Usuarios SET Estado = 0 WHERE Usuario = @Usuario";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@Usuario", nombreUsuario);
                comando.ExecuteNonQuery();
            }
        }

        public void ActualizarUltimaConexion(int idUsuario)
        {
            using (SqlConnection conexion = new SqlConnection(conexionBD))
            {
                conexion.Open();
                string consulta = "UPDATE Usuarios SET FechaUltimaConexion = @Fecha WHERE Id = @Id";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@Fecha", DateTime.Now);
                comando.Parameters.AddWithValue("@Id", idUsuario);
                comando.ExecuteNonQuery();
            }
        }
    }
}




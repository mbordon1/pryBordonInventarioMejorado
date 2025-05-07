using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace pryBordonInventarioMejorado
{
    internal class clsUsuarios
    {
        private string conexionBD = "Server=localhost;Database=Comercio;Trusted_Connection=True;";
        public clsUsuario VerificarCredenciales(string nombreUsuario, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                //no se puede proceder con la verificación de credenciales.fin
                return null;
            }

            using (SqlConnection conexion = new SqlConnection(conexionBD))
            {
                conexion.Open();

                string consulta = "SELECT * FROM Usuarios WHERE Usuario = @Usuario";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@Usuario", nombreUsuario);

                using (SqlDataReader reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        bool estado = Convert.ToBoolean(reader["Estado"]);
                        string contrasenaDB = reader["Contrasena"].ToString();
                        int intentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]);
                        string rol = reader["Rol"].ToString();
                        DateTime? fechaBloqueo = reader["FechaBloqueo"] != DBNull.Value
                            ? Convert.ToDateTime(reader["FechaBloqueo"])
                            : (DateTime?)null;
                        DateTime? fechaUltimaConexion = reader["FechaUltimaConexion"] != DBNull.Value
                            ? Convert.ToDateTime(reader["FechaUltimaConexion"])
                            : (DateTime?)null;

                        reader.Close();

                        if (!estado)
                        {
                            if (fechaBloqueo.HasValue)
                            {
                                double minutosTranscurridos = (DateTime.Now - fechaBloqueo.Value).TotalMinutes;

                                if (minutosTranscurridos >= 15)
                                {
                                    DesbloquearUsuario(nombreUsuario);
                                }
                                else
                                {
                                    return null; //bloqueadooo
                                }
                            }
                            else
                            {
                                return null; 
                            }
                        }

                        if (contrasenaDB != contrasena)
                        {
                            intentosFallidos++;

                            SqlCommand cmdUpdate = new SqlCommand("UPDATE Usuarios SET IntentosFallidos = @Intentos WHERE Usuario = @Usuario", conexion);
                            cmdUpdate.Parameters.AddWithValue("@Intentos", intentosFallidos);
                            cmdUpdate.Parameters.AddWithValue("@Usuario", nombreUsuario);
                            cmdUpdate.ExecuteNonQuery();

                            if (intentosFallidos >= 3)
                            {
                                BloquearUsuario(nombreUsuario);
                            }

                            return null; //las credenciales no son correctas
                        }

                        ReiniciarIntentosFallidos(nombreUsuario);

                        return new clsUsuario
                        {
                            Id = id,
                            NombreUsuario = nombreUsuario,
                            Contrasena = contrasenaDB,
                            Rol = rol,
                            Estado = estado,
                            FechaUltimaConexion = fechaUltimaConexion
                        };
                    }
                }
            }
            return null; 
        }

        public void BloquearUsuario(string nombreUsuario)
        {
            using (SqlConnection conexion = new SqlConnection(conexionBD))
            {
                conexion.Open();
                string consulta = "UPDATE Usuarios SET Estado = 0, FechaBloqueo = @FechaBloqueo WHERE Usuario = @Usuario";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@Usuario", nombreUsuario);
                comando.Parameters.AddWithValue("@FechaBloqueo", DateTime.Now);
                comando.ExecuteNonQuery();
            }
        }

        public void DesbloquearUsuario(string nombreUsuario)
        {
            using (SqlConnection conexion = new SqlConnection(conexionBD))
            {
                conexion.Open();
                string consulta = "UPDATE Usuarios SET Estado = 1, IntentosFallidos = 0, FechaBloqueo = NULL WHERE Usuario = @Usuario";
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

        public void ReiniciarIntentosFallidos(string nombreUsuario)
        {
            using (SqlConnection conexion = new SqlConnection(conexionBD))
            {
                conexion.Open();
                string consulta = "UPDATE Usuarios SET IntentosFallidos = 0 WHERE Usuario = @Usuario";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@Usuario", nombreUsuario);
                comando.ExecuteNonQuery();
            }
        }
    }
}





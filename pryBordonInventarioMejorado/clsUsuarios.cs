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
                    clsUsuario usuario = new clsUsuario
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        NombreUsuario = reader["Usuario"].ToString(),
                        Contrasena = reader["Contrasena"].ToString(),  
                        Rol = reader["Rol"].ToString()
                    };
                    return usuario;
                }

                return null;  
            }
        }
    }
}




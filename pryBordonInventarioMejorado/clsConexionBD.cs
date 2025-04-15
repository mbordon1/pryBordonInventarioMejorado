using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Globalization;


namespace pyInventario
{
    public class conexionBD
    {
        public string cadenaConexion = "Server=localhost;Database=Comercio;Trusted_Connection=True;";

        private SqlConnection conexion;

        public string nombreBaseDeDatos;

        public conexionBD()
        {
            conexion = new SqlConnection(cadenaConexion); 
        }
        public void ConectarBD()
        {
            try
            {
                conexion = new SqlConnection(cadenaConexion);

                nombreBaseDeDatos = conexion.Database;

                conexion.Open();

                MessageBox.Show("Conectado a " + nombreBaseDeDatos);
            }
            catch (Exception error)
            {
                MessageBox.Show("Tiene un errorcito - " + error.Message);
            }
        }

        public void EjecutarComando(SqlCommand comando)
        {
            try
            {
                comando.Connection = conexion;
                conexion.Open();
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar comando: " + ex.Message);
            }
            finally
            {
                conexion.Close();
            }
        }

        public DataTable EjecutarConsulta(SqlCommand comando)
        {
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = conexion;
                conexion.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(comando);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar consulta: " + ex.Message);
            }
            finally
            {
                conexion.Close();
            }
            return dt;
        }
    }
}


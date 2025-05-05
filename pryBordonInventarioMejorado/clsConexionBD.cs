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

        public static string ObtenerCadenaConexion()
        {
            return "Server=localhost;Database=Comercio;Trusted_Connection=True;";
        }

        public void ConectarBD()
        {
            try
            {
                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                    nombreBaseDeDatos = conexion.Database;
                    MessageBox.Show("Conectado a " + nombreBaseDeDatos);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Error de conexión: " + error.Message);
            }
        }

        // Ejecutar comandos que no devuelvan datos (INSERT, UPDATE, DELETE)
        public void EjecutarComando(SqlCommand comando)
        {
            try
            {
                comando.Connection = conexion;

                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }

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

                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }

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

        // utilizado para consultas que devuelvan un solo valor (COUNT, MAX, etc.)
        public object EjecutarEscalar(SqlCommand comando)
        {
            object resultado = null;
            try
            {
                comando.Connection = conexion;

                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }

                resultado = comando.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar consulta escalar: " + ex.Message);
            }
            finally
            {
                conexion.Close();
            }
            return resultado;
        }
    }
}


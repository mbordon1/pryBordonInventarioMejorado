using pyInventario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace pryBordonInventarioMejorado
{
    public class clsContactosCRUD
    {
        private conexionBD BD = new conexionBD();

        private SqlCommand CrearComando(string consulta, Dictionary<string, object> parametros = null)
        {
            SqlCommand comando = new SqlCommand(consulta);
            if (parametros != null)
            {
                foreach (var param in parametros)
                {
                    comando.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }
            return comando;
        }

        public void AgregarContacto(clsContacto contacto)
        {
            string consulta = "INSERT INTO Contactos (NombreApellido, Telefono, Correo, CategoriaId) VALUES (@NombreApellido, @Telefono, @Correo, @CategoriaId)";
            SqlCommand comando = CrearComando(consulta, new Dictionary<string, object>
            {
                { "@NombreApellido", contacto.NombreApellido },
                { "@Telefono", contacto.Telefono },
                { "@Correo", contacto.Correo },
                { "@CategoriaId", contacto.CategoriaId }
            });
            BD.EjecutarComando(comando);
        }

        public void EliminarContacto(int id)
        {
            string consulta = "DELETE FROM Contactos WHERE Id = @Id";
            BD.EjecutarComando(CrearComando(consulta, new Dictionary<string, object> { { "@Id", id } }));
        }

        public void ModificarContacto(clsContacto contacto)
        {
            string consulta = @"UPDATE Contactos
                        SET NombreApellido = @NombreApellido,
                            Telefono = @Telefono,
                            Correo = @Correo,
                            CategoriaId = @CategoriaId
                        WHERE Id = @Id";

            SqlCommand comando = CrearComando(consulta, new Dictionary<string, object>
            {
            { "@Id", contacto.Id },
            { "@NombreApellido", contacto.NombreApellido },
            { "@Telefono", contacto.Telefono },
            { "@Correo", contacto.Correo },
            { "@CategoriaId", contacto.CategoriaId }
            });

            BD.EjecutarComando(comando);
        }

        public DataTable ObtenerContactos()
        {
            string consulta = @"
            SELECT c.Id, c.NombreApellido, c.Telefono, c.Correo, cat.Nombre AS Categoria, c.CategoriaId
            FROM Contactos c
            LEFT JOIN CategoriasContactos cat ON c.CategoriaId = cat.Id";
            return BD.EjecutarConsulta(CrearComando(consulta));
        }

        public DataTable BuscarPorNombre(string nombre)
        {
            string consulta = @"
                SELECT c.Id, c.NombreApellido, c.Telefono, c.Correo, cat.Nombre AS Categoria
                FROM Contactos c
                LEFT JOIN CategoriasContactos cat ON c.CategoriaId = cat.Id
                WHERE c.NombreApellido LIKE @nombre";
            return BD.EjecutarConsulta(CrearComando(consulta, new Dictionary<string, object>
            {
                { "@nombre", $"%{nombre}%" }
            }));
        }

        public void ActualizarCambiosDesdeDataTable(DataTable cambios)
        {
            using (SqlConnection conexion = new SqlConnection(BD.cadenaConexion))
            {
                conexion.Open();

                SqlDataAdapter adaptador = new SqlDataAdapter("SELECT Id, NombreApellido, Telefono, Correo, CategoriaId FROM Contactos", conexion);
                SqlCommandBuilder builder = new SqlCommandBuilder(adaptador);

                if (cambios.Columns.Contains("Categoria"))
                {
                    cambios.Columns.Remove("Categoria");
                }

                try
                {
                    adaptador.Update(cambios);
                }
                catch (DBConcurrencyException ex)
                {
                    MessageBox.Show("Error de concurrencia: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error general al actualizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public DataTable ObtenerCategorias()
        {
            string consulta = "SELECT DISTINCT Id, Nombre FROM CategoriasContactos";
            return BD.EjecutarConsulta(CrearComando(consulta));
        }

        public int ObtenerCategoriaIdPorNombre(string nombre)
        {
            string consulta = "SELECT Id FROM CategoriasContactos WHERE Nombre = @Nombre";
            SqlCommand comando = new SqlCommand(consulta);
            comando.Parameters.AddWithValue("@Nombre", nombre);
            DataTable resultado = BD.EjecutarConsulta(comando);

            if (resultado.Rows.Count > 0)
                return Convert.ToInt32(resultado.Rows[0]["Id"]);

            throw new Exception("Categoría no encontrada: " + nombre);
        }
    }
}




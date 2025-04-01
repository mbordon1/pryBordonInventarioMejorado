using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using System.Reflection;

namespace pyInventario
{
    public class conexionBD
    {
        OleDbCommand comando;
        OleDbConnection conexion;
        OleDbDataAdapter adaptador;
        public string cadena;

        public conexionBD()
        {
            cadena = @"Provider=Microsoft.ACE.OLEDB.16.0;Data Source=C:\\Users\\mbord\\Desktop\\pryBordonInventarioMejorado\\pryBordonInventarioMejorado\\bin\\Debug\\dbProducts.mdb;";
        }

        public void mostrarProductos(DataGridView dgvProductos)
        {
            try
            {
                conexion = new OleDbConnection(cadena);
                comando = new OleDbCommand();

                comando.Connection = conexion;
                comando.CommandType = CommandType.Text;
                comando.CommandText = "SELECT * FROM Productos";

                DataTable tablaProductos = new DataTable();

                adaptador = new OleDbDataAdapter(comando);
                adaptador.Fill(tablaProductos);

                dgvProductos.DataSource = tablaProductos; 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void agregarProductos(Productos producto)
        {
            if (string.IsNullOrWhiteSpace(producto.Nombre) || producto.Precio <= 0 || producto.Stock < 0)
            {
                MessageBox.Show("Por favor ingrese los datos correctamente.");
                return;
            }

            try
            {
                using (OleDbConnection conexion = new OleDbConnection(cadena))
                using (OleDbCommand comando = new OleDbCommand())
                {
                    comando.Connection = conexion;
                    comando.CommandType = CommandType.Text;
                    comando.CommandText = "INSERT INTO Productos(Nombre, Descripcion, Precio, Stock, Categoria) VALUES(?, ?, ?, ?, ?)";

                    comando.Parameters.AddWithValue("?", producto.Nombre);
                    comando.Parameters.AddWithValue("?", producto.Descripcion);
                    comando.Parameters.AddWithValue("?", producto.Precio);
                    comando.Parameters.AddWithValue("?", producto.Stock);
                    comando.Parameters.AddWithValue("?", producto.Categoria);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar el producto: {ex.Message}");
            }
        }

        public void modificarProductos(Productos producto)
        {
            try
            {
                using (OleDbConnection conexion = new OleDbConnection(cadena))
                using (OleDbCommand comando = new OleDbCommand())
                {
                    comando.Connection = conexion;
                    comando.CommandType = CommandType.Text;
                    comando.CommandText = "UPDATE Productos SET Nombre = ?, Descripcion = ?, Precio = ?, Stock = ?, Categoria = ? WHERE ID = ?";

                    comando.Parameters.AddWithValue("?", producto.Nombre);
                    comando.Parameters.AddWithValue("?", producto.Descripcion);
                    comando.Parameters.AddWithValue("?", producto.Precio);
                    comando.Parameters.AddWithValue("?", producto.Stock);
                    comando.Parameters.AddWithValue("?", producto.Categoria);
                    comando.Parameters.AddWithValue("?", producto.ID);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Producto modificado correctamente.");
                    }
                    else
                    {
                        MessageBox.Show("No se encontró un producto con ese ID.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar el producto: {ex.Message}");
            }
        }

        public void eliminarProducto(string ID)
        {
            OleDbConnection conexion = null;
            OleDbCommand comando = null;
            try
            {
                conexion = new OleDbConnection(cadena);
                comando = new OleDbCommand();
                comando.Connection = conexion;
                comando.CommandType = CommandType.Text;

                comando.CommandText = "SELECT COUNT(*) FROM Productos WHERE ID = ?";
                comando.Parameters.AddWithValue("?", ID);

                conexion.Open();

                int count = Convert.ToInt32(comando.ExecuteScalar());
                if (count == 0)
                {
                    MessageBox.Show("No se encontró el producto con ese código.");
                    return;
                }

                comando.CommandText = "DELETE FROM Productos WHERE ID = ?";
                int filasAfectadas = comando.ExecuteNonQuery();

                if (filasAfectadas > 0)
                {
                    MessageBox.Show("Producto eliminado con éxito.");
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el producto. Intenta de nuevo.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el producto: {ex.Message}");
            }
            finally
            {
                if (conexion != null && conexion.State == ConnectionState.Open)
                {
                    conexion.Close();
                }
            }
        }

        public void buscarProductoPorID(string id, DataGridView dgvProductos)
        {
            try
            {
                if (!int.TryParse(id, out int idNumerico))
                {
                    MessageBox.Show("Por favor ingrese un ID numérico válido.");
                    return;
                }

                using (OleDbConnection conexion = new OleDbConnection(cadena))
                using (OleDbCommand comando = new OleDbCommand())
                {
                    comando.Connection = conexion;
                    comando.CommandType = CommandType.Text;

                    comando.CommandText = "SELECT * FROM Productos WHERE ID = @ID";
                    comando.Parameters.AddWithValue("@ID", idNumerico);

                    DataTable tablaProductos = new DataTable();
                    using (OleDbDataAdapter adaptador = new OleDbDataAdapter(comando))
                    {
                        adaptador.Fill(tablaProductos);
                    }

                    if (tablaProductos.Rows.Count > 0)
                    {
                        dgvProductos.DataSource = tablaProductos;
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ningún producto con ese ID.");
                        dgvProductos.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el producto: {ex.Message}");
            }
        }

    }
}


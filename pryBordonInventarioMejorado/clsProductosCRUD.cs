using pyInventario;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pryBordonInventarioMejorado
{
    internal class clsProductosCRUD
    {
        private conexionBD Conexion;

        public clsProductosCRUD()
        {
            Conexion = new conexionBD();
        }

        public void AgregarProducto(clsProductos producto)
        {
            try
            {
                string query = "INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, CategoriaId) " +
                               "VALUES (@Nombre, @Descripcion, @Precio, @Stock, @CategoriaId)";

                SqlCommand comando = new SqlCommand(query);
                comando.Parameters.AddWithValue("@Nombre", producto.Nombre);
                comando.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                comando.Parameters.AddWithValue("@Precio", producto.Precio);
                comando.Parameters.AddWithValue("@Stock", producto.Stock);
                comando.Parameters.AddWithValue("@CategoriaId", producto.CategoriaId);

                Conexion.EjecutarComando(comando);
                MessageBox.Show("Producto agregado correctamente.");
            }
            catch (Exception error)
            {
                MessageBox.Show("Error al agregar producto: " + error.Message);
            }
        }


        public DataTable ObtenerProductos()
        {
            string query = "SELECT p.Codigo, p.Nombre, p.Descripcion, p.Precio, p.Stock, c.Nombre AS Categoria " +
                           "FROM Productos p INNER JOIN Categorias c ON p.CategoriaId = c.Id";
            SqlCommand comando = new SqlCommand(query);
            return Conexion.EjecutarConsulta(comando);
        }

        public DataTable BuscarProductoPorTexto(string texto)
        {
            string query = "SELECT p.Codigo, p.Nombre, p.Descripcion, p.Precio, p.Stock, c.Nombre AS Categoria " +
                           "FROM Productos p INNER JOIN Categorias c ON p.CategoriaId = c.Id " +
                           "WHERE p.Nombre LIKE @Texto OR p.Codigo LIKE @Texto";

            SqlCommand comando = new SqlCommand(query);
            comando.Parameters.AddWithValue("@Texto", "%" + texto + "%");
            return Conexion.EjecutarConsulta(comando);
        }

        public void ModificarProducto(clsProductos producto)
        {
            try
            {
                string query = "UPDATE Productos SET Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio, " +
                               "Stock = @Stock, CategoriaId = @CategoriaId WHERE Codigo = @Codigo";

                SqlCommand comando = new SqlCommand(query);
                comando.Parameters.AddWithValue("@Codigo", producto.Codigo);
                comando.Parameters.AddWithValue("@Nombre", producto.Nombre);
                comando.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                comando.Parameters.AddWithValue("@Precio", producto.Precio);
                comando.Parameters.AddWithValue("@Stock", producto.Stock);
                comando.Parameters.AddWithValue("@CategoriaId", producto.CategoriaId);

                Conexion.EjecutarComando(comando);
                MessageBox.Show("Producto modificado correctamente.");
            }
            catch (Exception error)
            {
                MessageBox.Show("Error al modificar producto: " + error.Message);
            }
        }

        public void EliminarProductoPorNombre(string nombreProducto)
        {
            try
            {
                string query = "DELETE FROM Productos WHERE Nombre = @Nombre";
                SqlCommand comando = new SqlCommand(query);
                comando.Parameters.AddWithValue("@Nombre", nombreProducto);

                Conexion.EjecutarComando(comando);
                MessageBox.Show("Producto eliminado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar producto: " + ex.Message);
            }
        }

        public DataTable ObtenerCategorias()
        {
            string query = "SELECT Id, Nombre FROM Categorias";
            SqlCommand comando = new SqlCommand(query);
            return Conexion.EjecutarConsulta(comando);
        }
    }

}


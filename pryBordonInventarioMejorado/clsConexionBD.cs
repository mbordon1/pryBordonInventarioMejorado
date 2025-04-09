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
        public string cadena;

        public conexionBD()
        {
            cadena = "Server=localhost;Database=Productos;Trusted_Connection=True;";
        }

        public void mostrarProductos(DataGridView dgvProductos)
        {
            try
            {
                string consulta = @"
                SELECT 
                p.Codigo,
                p.Nombre,
                p.Descripcion,
                p.Precio,
                p.Stock,
                c.Nombre AS Categoria
                FROM Productos p
                INNER JOIN Categorias c ON p.CategoriaId = c.Id";

                using (SqlConnection conexion = new SqlConnection(cadena))
                using (SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion))
                {
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);
                    dgvProductos.DataSource = tabla;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los productos: {ex.Message}");
            }
        }

        public void agregarProducto(clsProductos producto)
        {
            using (SqlConnection conexion = new SqlConnection(cadena))
            {
                conexion.Open();
                string consulta = @"INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, CategoriaId) 
                            VALUES (@Nombre, @Descripcion, @Precio, @Stock, @CategoriaId)";

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    comando.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    comando.Parameters.AddWithValue("@Precio", producto.Precio);
                    comando.Parameters.AddWithValue("@Stock", producto.Stock);
                    comando.Parameters.AddWithValue("@CategoriaId", producto.CategoriaId);
                    comando.ExecuteNonQuery();
                }
            }
        }

        public void modificarProducto(clsProductos producto)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                using (SqlCommand comando = new SqlCommand())
                {
                    comando.Connection = conexion;
                    comando.CommandType = CommandType.Text;
                    comando.CommandText = @"
                UPDATE Productos 
                SET Nombre = @Nombre, 
                    Descripcion = @Descripcion, 
                    Precio = @Precio, 
                    Stock = @Stock, 
                    CategoriaId = @CategoriaId 
                WHERE Codigo = @Codigo";

                    comando.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    comando.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    comando.Parameters.AddWithValue("@Precio", producto.Precio);
                    comando.Parameters.AddWithValue("@Stock", producto.Stock);
                    comando.Parameters.AddWithValue("@CategoriaId", producto.CategoriaId); 
                    comando.Parameters.AddWithValue("@Codigo", producto.Codigo); 

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Producto modificado correctamente.");
                    }
                    else
                    {
                        MessageBox.Show("No se encontró un producto con ese Código.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar el producto: {ex.Message}");
            }
        }

        public void eliminarProducto(string codigo)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                using (SqlCommand comando = new SqlCommand())
                {
                    comando.Connection = conexion;
                    comando.CommandType = CommandType.Text;

                    comando.CommandText = "SELECT COUNT(*) FROM Productos WHERE Codigo = @Codigo";
                    comando.Parameters.AddWithValue("@Codigo", codigo);

                    conexion.Open();

                    int count = Convert.ToInt32(comando.ExecuteScalar());
                    if (count == 0)
                    {
                        MessageBox.Show("No se encontró el producto con ese código.");
                        return;
                    }

                    comando.CommandText = "DELETE FROM Productos WHERE Codigo = @Codigo";

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el producto: {ex.Message}");
            }
        }

        public void buscarProductoPorTexto(string texto, DataGridView dgv)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                using (SqlCommand comando = new SqlCommand())
                {
                    comando.Connection = conexion;
                    comando.CommandType = CommandType.Text;

                    comando.CommandText = "SELECT * FROM Productos WHERE Nombre LIKE @Texto";
                    comando.Parameters.AddWithValue("@Texto", "%" + texto + "%");

                    conexion.Open();

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (!lector.HasRows)
                        {
                            MessageBox.Show("No se encontraron productos con ese nombre");
                            dgv.DataSource = null;
                            return;
                        }

                        DataTable tabla = new DataTable();
                        tabla.Load(lector);
                        dgv.DataSource = tabla;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el producto: {ex.Message}");
            }
        }

        public void GenerarVisualizarYDescargarReporteHTML()
        {
            try
            {
                string htmlContent = GenerarHTMLReport();

                MostrarEnNavegador(htmlContent);

                if (MessageBox.Show("¿Desea guardar una copia del reporte?", "Guardar reporte",
                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    GuardarReporte(htmlContent);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerarHTMLReport()
        {
            DataTable productos = new DataTable();

            using (SqlConnection conexion = new SqlConnection(cadena))
            using (SqlCommand comando = new SqlCommand(@"
            SELECT 
               p.Codigo,
               p.Nombre,
               p.Descripcion,
               p.Precio,
               p.Stock,
               c.Nombre AS Categoria
           FROM Productos p
           INNER JOIN Categorias c ON p.CategoriaId = c.Id
           ORDER BY c.Nombre, p.Nombre", conexion))
            using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
            {
                adaptador.Fill(productos);
            }

            decimal valorTotalInventario = 0;
            int totalProductos = productos.Rows.Count;
            var categorias = productos.AsEnumerable()
                .Select(row => row.Field<string>("Categoria"))
                .Distinct()
                .OrderBy(cat => cat)
                .ToList();

            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder.AppendLine("<!DOCTYPE html>");
            htmlBuilder.AppendLine("<html lang='es'>");
            htmlBuilder.AppendLine("<head>");
            htmlBuilder.AppendLine("  <meta charset='UTF-8'>");
            htmlBuilder.AppendLine("  <title>Reporte de Inventario</title>");
            htmlBuilder.AppendLine("  <style>");
            htmlBuilder.AppendLine("    body { font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; }");
            htmlBuilder.AppendLine("    h1 { text-align: center; color: #2c3e50; }");
            htmlBuilder.AppendLine("    .categoria { font-size: 20px; font-weight: bold; background-color: #34495e; color: white; padding: 10px; margin-top: 30px; border-radius: 5px; }");
            htmlBuilder.AppendLine("    table { width: 100%; border-collapse: collapse; margin-top: 10px; background-color: white; box-shadow: 0 0 5px rgba(0,0,0,0.1); }");
            htmlBuilder.AppendLine("    th, td { border: 1px solid #ccc; padding: 10px; text-align: center; }");
            htmlBuilder.AppendLine("    th { background-color: #2980b9; color: white; }");
            htmlBuilder.AppendLine("    tr:nth-child(even) { background-color: #f9f9f9; }");
            htmlBuilder.AppendLine("    .bajo-stock { background-color: #e74c3c; color: white; font-weight: bold; }");
            htmlBuilder.AppendLine("    .resumen { margin-top: 40px; background-color: #ecf0f1; padding: 20px; border-radius: 10px; }");
            htmlBuilder.AppendLine("    .resumen h3 { color: #2c3e50; }");
            htmlBuilder.AppendLine("    .resumen p { font-size: 16px; }");
            htmlBuilder.AppendLine("  </style>");
            htmlBuilder.AppendLine("</head>");
            htmlBuilder.AppendLine("<body>");
            htmlBuilder.AppendLine("<h1>Reporte de Inventario</h1>");

            foreach (var categoria in categorias)
            {
                var productosCategoria = productos.Select($"Categoria = '{categoria}'");
                decimal valorCategoria = productosCategoria.Sum(p =>
                Convert.ToDecimal(p["Precio"]) * Convert.ToInt32(p["Stock"]));

                htmlBuilder.AppendLine($"  <div class='categoria'>{categoria.ToUpper()} (Total: {valorCategoria.ToString("C", new CultureInfo("es-AR"))})</div>");
                htmlBuilder.AppendLine("  <table>");
                htmlBuilder.AppendLine("    <tr>");
                htmlBuilder.AppendLine("      <th>Código</th>");
                htmlBuilder.AppendLine("      <th>Nombre</th>");
                htmlBuilder.AppendLine("      <th>Descripción</th>");
                htmlBuilder.AppendLine("      <th>Precio</th>");
                htmlBuilder.AppendLine("      <th>Stock</th>");
                htmlBuilder.AppendLine("      <th>Valor Total</th>");
                htmlBuilder.AppendLine("    </tr>");

                foreach (DataRow row in productosCategoria)
                {
                    decimal precio = Convert.ToDecimal(row["Precio"]);
                    int stock = Convert.ToInt32(row["Stock"]);
                    decimal valorTotal = precio * stock;
                    valorTotalInventario += valorTotal;

                    bool bajoStock = stock < 10;
                    string claseStock = bajoStock ? "class='bajo-stock'" : "";

                    htmlBuilder.AppendLine("    <tr>");
                    htmlBuilder.AppendLine($"      <td>{row["Codigo"]}</td>");
                    htmlBuilder.AppendLine($"      <td>{row["Nombre"]}</td>");
                    htmlBuilder.AppendLine($"      <td>{row["Descripcion"]}</td>");
                    htmlBuilder.AppendLine($"      <td>{precio.ToString("C", new CultureInfo("es-AR"))}</td>");
                    htmlBuilder.AppendLine($"      <td {claseStock}>{stock}</td>");
                    htmlBuilder.AppendLine($"      <td>{valorTotal.ToString("C", new CultureInfo("es-AR"))}</td>");
                    htmlBuilder.AppendLine("    </tr>");
                }

                htmlBuilder.AppendLine("  </table>");
            }

            htmlBuilder.AppendLine("  <div class='resumen'>");
            htmlBuilder.AppendLine("    <h3>Resumen General</h3>");
            htmlBuilder.AppendLine($"    <p><strong>Total de Productos:</strong> {totalProductos}</p>");
            htmlBuilder.AppendLine($"    <p><strong>Categorías:</strong> {categorias.Count}</p>");
            htmlBuilder.AppendLine($"    <p><strong>Valor Total del Inventario:</strong> {valorTotalInventario.ToString("C", new CultureInfo("es-AR"))}</p>");
            htmlBuilder.AppendLine("  </div>");

            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");

            return htmlBuilder.ToString();
        }

        private void MostrarEnNavegador(string htmlContent)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), $"ReporteInventario_{DateTime.Now:yyyyMMddHHmmss}.html");
            File.WriteAllText(tempFile, htmlContent);

            Process.Start(new ProcessStartInfo(tempFile) { UseShellExecute = true });
        }

        private void GuardarReporte(string htmlContent)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Archivo HTML|*.html";
            saveDialog.Title = "Guardar Reporte de Productos";
            saveDialog.FileName = $"ReporteInventario_{DateTime.Now:yyyyMMdd}.html";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveDialog.FileName, htmlContent);
                MessageBox.Show($"Reporte guardado en:\n{saveDialog.FileName}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}



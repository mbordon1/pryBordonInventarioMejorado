using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Forms;
using pyInventario;

public class clsReporteHTML
{
    private conexionBD conexion;

    public clsReporteHTML()
    {
        conexion = new conexionBD(); 
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

        conexion.ConectarBD(); 

        using (SqlConnection conexionBD = new SqlConnection(conexion.cadenaConexion)) 
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
        ORDER BY c.Nombre, p.Nombre", conexionBD))
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



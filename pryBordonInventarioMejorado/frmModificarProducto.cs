using pyInventario;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pryBordonInventarioMejorado
{
    public partial class frmModificarProducto : Form
    {
        private clsProductosCRUD productosCRUD;
        private DataTable categorias;
        private List<DataRow> productosOriginales;  // Lista para guardar los productos originales- Sin modificacion

        public frmModificarProducto()
        {
            InitializeComponent();
            productosCRUD = new clsProductosCRUD();
        }

        private void frmModificarProducto_Load(object sender, EventArgs e)
        {
            CargarCategorias();
            CargarProductos();

            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.MultiSelect = false;
            dgvProductos.ReadOnly = false;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.Columns["Codigo"].ReadOnly = true;
        }

        private void CargarCategorias()
        {
            try
            {
                categorias = productosCRUD.ObtenerCategorias();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar categorías: " + ex.Message);
            }
        }

        private void CargarProductos()
        {
            DataTable productos = productosCRUD.ObtenerProductos();
            dgvProductos.DataSource = productos;
            dgvProductos.ClearSelection();
            productosOriginales = productos.AsEnumerable().ToList();  // Guardar el estado original de los productos
            ReemplazarColumnaCategoriaConComboBox();
        }

        private void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            bool cambiosDetectados = false;

            // Recorre todas las filas para detectar cambios
            foreach (DataGridViewRow fila in dgvProductos.Rows)
            {
                if (fila.IsNewRow) continue;  // Ignorar la fila de nueva

                int codigoProducto = Convert.ToInt32(fila.Cells["Codigo"].Value);
                var productoOriginal = productosOriginales.FirstOrDefault(p => Convert.ToInt32(p["Codigo"]) == codigoProducto);

                if (productoOriginal != null)
                {
                    bool haCambiado = false;
                    string nombreProducto = fila.Cells["Nombre"].Value?.ToString();

                    // Comparar cada celda para ver si hubo cambios
                    if (productoOriginal["Nombre"].ToString() != nombreProducto)
                        haCambiado = true;
                    else if (productoOriginal["Descripcion"].ToString() != fila.Cells["Descripcion"].Value?.ToString())
                        haCambiado = true;
                    else if (Convert.ToDecimal(productoOriginal["Precio"]) != Convert.ToDecimal(fila.Cells["Precio"].Value))
                        haCambiado = true;
                    else if (Convert.ToInt32(productoOriginal["Stock"]) != Convert.ToInt32(fila.Cells["Stock"].Value))
                        haCambiado = true;
                    else if (Convert.ToInt32(productoOriginal["CategoriaId"]) != ObtenerCategoriaIdDesdeNombre(fila.Cells["Categoria"].Value?.ToString()))
                        haCambiado = true;

                    // Si hubo cambios, se guarda el producto
                    if (haCambiado)
                    {
                        cambiosDetectados = true;
                        clsProductos productoModificado = new clsProductos()
                        {
                            Codigo = codigoProducto,
                            Nombre = fila.Cells["Nombre"].Value?.ToString(),
                            Descripcion = fila.Cells["Descripcion"].Value?.ToString(),
                            Precio = Convert.ToDecimal(fila.Cells["Precio"].Value),
                            Stock = Convert.ToInt32(fila.Cells["Stock"].Value),
                            CategoriaId = ObtenerCategoriaIdDesdeNombre(fila.Cells["Categoria"].Value?.ToString())
                        };

                        try
                        {
                            productosCRUD.ModificarProducto(productoModificado);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al guardar cambios: " + ex.Message);
                            return;
                        }
                    }
                }
            }

            if (cambiosDetectados)
            {
                MessageBox.Show("Cambios guardados correctamente.");
                CargarProductos();  
            }
            else
            {
                MessageBox.Show("No se detectaron cambios.");
            }
        }

        private int ObtenerCategoriaIdDesdeNombre(string nombreCategoria)
        {
            if (categorias != null)
            {
                foreach (DataRow fila in categorias.Rows)
                {
                    if (fila["Nombre"].ToString().Equals(nombreCategoria, StringComparison.OrdinalIgnoreCase))
                    {
                        return Convert.ToInt32(fila["Id"]);
                    }
                }
            }
            throw new Exception("Categoría no encontrada.");
        }

        private void ReemplazarColumnaCategoriaConComboBox()
        {
            int indiceColumna = dgvProductos.Columns["Categoria"].Index;

            DataGridViewComboBoxColumn comboCategoria = new DataGridViewComboBoxColumn
            {
                Name = "Categoria",
                HeaderText = "Categoría",
                DataPropertyName = "Categoria",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                FlatStyle = FlatStyle.Flat,
                Width = dgvProductos.Columns["Categoria"].Width
            };

            foreach (DataRow fila in categorias.Rows)
            {
                comboCategoria.Items.Add(fila["Nombre"].ToString());
            }

            dgvProductos.Columns.Remove("Categoria");
            dgvProductos.Columns.Insert(indiceColumna, comboCategoria);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}






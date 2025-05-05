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
        DataTable productosOriginales; // esta lista guarda los productos originales - sin modificacion

        public frmModificarProducto()
        {
            InitializeComponent();
            productosCRUD = new clsProductosCRUD();
            productosOriginales = new DataTable();
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
            dgvProductos.Columns["Precio"].ReadOnly = false;  
            dgvProductos.Columns["Stock"].ReadOnly = false;  
            dgvProductos.Columns["Categoria"].ReadOnly = false;  
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
            DataTable dt = productosCRUD.ObtenerProductos();
            productosOriginales = dt.Copy(); // copia para comparar despues 
            dgvProductos.DataSource = dt;

            ReemplazarColumnaCategoriaConComboBox(); 
        }

        private void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            bool cambiosDetectados = false;

            for (int i = 0; i < dgvProductos.Rows.Count; i++)
            {
                if (dgvProductos.Rows[i].IsNewRow) continue;

                DataGridViewRow fila = dgvProductos.Rows[i];
                DataRow filaOriginal = productosOriginales.Rows[i];

                int codigo = Convert.ToInt32(fila.Cells["Codigo"].Value);
                string nombre = fila.Cells["Nombre"].Value?.ToString();
                string descripcion = fila.Cells["Descripcion"].Value?.ToString();
                decimal precio = Convert.ToDecimal(fila.Cells["Precio"].Value);
                int stock = Convert.ToInt32(fila.Cells["Stock"].Value);
                int categoriaId = ObtenerCategoriaIdDesdeNombre(fila.Cells["Categoria"].Value?.ToString());

                bool haCambiado =
                    filaOriginal["Nombre"].ToString() != nombre ||
                    filaOriginal["Descripcion"].ToString() != descripcion ||
                    Convert.ToDecimal(filaOriginal["Precio"]) != precio ||
                    Convert.ToInt32(filaOriginal["Stock"]) != stock ||
                    Convert.ToInt32(filaOriginal["CategoriaId"]) != categoriaId;

                if (haCambiado)
                {
                    clsProductos productoModificado = new clsProductos
                    {
                        Codigo = codigo,
                        Nombre = nombre,
                        Descripcion = descripcion,
                        Precio = precio,
                        Stock = stock,
                        CategoriaId = categoriaId
                    };

                    productosCRUD.ModificarProducto(productoModificado);
                    cambiosDetectados = true;
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

            dgvProductos.Columns.RemoveAt(indiceColumna);
            dgvProductos.Columns.Insert(indiceColumna, comboCategoria);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}






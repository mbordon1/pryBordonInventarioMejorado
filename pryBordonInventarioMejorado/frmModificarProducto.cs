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
        public frmModificarProducto()
        {
            InitializeComponent();
        }

        private void frmModificarProducto_Load(object sender, EventArgs e)
        {
            conexionBD BD = new conexionBD();
            BD.mostrarProductos(dgvProductos);

            // Carga de categorías desde la base de datos (nueva mejora)
            using (SqlConnection conexion = new SqlConnection(BD.cadena))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, Nombre FROM Categorias", conexion);
                SqlDataAdapter adaptador = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adaptador.Fill(dt);

                cmbCategorias.DisplayMember = "Nombre";
                cmbCategorias.ValueMember = "Id";
                cmbCategorias.DataSource = dt;
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCodigo.Text))
                {
                    MessageBox.Show("Ingrese un código de producto para actualizar.");
                    return;
                }

                if (cmbCategorias.SelectedIndex == -1)
                {
                    MessageBox.Show("Seleccione una categoría válida.");
                    return;
                }

                int codigoProducto = Convert.ToInt32(txtCodigo.Text);
                int categoriaId = Convert.ToInt32(cmbCategorias.SelectedValue);

                clsProductos productoActualizado = new clsProductos()
                {
                    Codigo = codigoProducto,
                    Nombre = txtNombre.Text,
                    Descripcion = txtDescripcion.Text,
                    Precio = Convert.ToDecimal(numPrecio.Value),
                    Stock = Convert.ToInt32(numStock.Value),
                    CategoriaId = categoriaId
                };

                conexionBD BD = new conexionBD();
                BD.modificarProducto(productoActualizado);
                BD.mostrarProductos(dgvProductos);

                MessageBox.Show("Producto modificado con éxito.");
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar el producto: {ex.Message}");
            }
        }
        private void LimpiarCampos()
        {
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            numPrecio.Value = 0;
            numStock.Value = 0;
            cmbCategorias.SelectedIndex = -1;
        }
    }
}


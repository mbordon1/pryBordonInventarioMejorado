using pyInventario;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pryBordonInventarioMejorado
{
    public partial class frmEliminarProducto : Form
    {
        private clsProductosCRUD productosCRUD = new clsProductosCRUD();

        public frmEliminarProducto()
        {
            InitializeComponent();
        }

        private void frmEliminarProducto_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Por favor, seleccione la fila del producto que desea eliminar");
            CargarProductos();
        }

        private void CargarProductos()
        {
            DataTable productos = productosCRUD.ObtenerProductos();
            if (productos != null)
            {
                dgvProductos.DataSource = productos;
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0)
            {
                btnEliminar.Enabled = false;
                MessageBox.Show("Seleccione un producto de la lista para eliminar.");
                return;
            }
            else
            {
                btnEliminar.Enabled=true;
                DataGridViewRow filaSeleccionada = dgvProductos.SelectedRows[0];

                string nombreProducto = filaSeleccionada.Cells["Nombre"].Value.ToString();

                DialogResult confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea eliminar el producto '{nombreProducto}'?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirmacion == DialogResult.Yes)
                {
                    productosCRUD.EliminarProductoPorNombre(nombreProducto);

                    CargarProductos();
                }
            }    
        }
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}


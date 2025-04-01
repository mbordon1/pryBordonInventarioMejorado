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
        public frmEliminarProducto()
        {
            InitializeComponent();
        }

        private void frmEliminarProducto_Load(object sender, EventArgs e)
        {
            conexionBD BD = new conexionBD();
            BD.mostrarProductos(dgvProductos);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                conexionBD BD = new conexionBD();
                string ID = txtCodigoEliminar.Text;

                if (string.IsNullOrEmpty(ID))
                {
                    MessageBox.Show("Por favor ingrese un código de producto.");
                    return;
                }

                BD.eliminarProducto(ID);
                BD.mostrarProductos(dgvProductos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el producto: {ex.Message}");
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

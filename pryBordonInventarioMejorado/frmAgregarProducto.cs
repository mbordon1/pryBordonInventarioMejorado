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
    public partial class frmAgregarProducto : Form
    {
        public frmAgregarProducto()
        {
            InitializeComponent();
        }

        private void frmAgregarProducto_Load(object sender, EventArgs e)
        {
            conexionBD BD = new conexionBD();
            BD.mostrarProductos(dgvProductos);

            string[] categorias = new string[] { "Electronicos", "Hogar", "Deportes", "Libros", "Accesorios", "Herramientas", "Cocina", "Oficina" };

            foreach (var categoria in categorias)
            {
                cmbCategorias.Items.Add(categoria);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                Productos productoNuevo = new Productos();
                conexionBD BD = new conexionBD();

                productoNuevo.Nombre = txtNombre.Text;
                productoNuevo.Descripcion = txtDescripcion.Text;
                productoNuevo.Precio = Convert.ToInt32(numPrecio.Value);
                productoNuevo.Stock = Convert.ToInt32(numStock.Value);
                productoNuevo.Categoria = cmbCategorias.Text;

                BD.agregarProductos(productoNuevo);
                BD.mostrarProductos(dgvProductos);

                MessageBox.Show("Producto agregado con éxito.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}");
            }

            txtNombre.Text = "";
            txtDescripcion.Text = "";
            numPrecio.Value = 0;
            numStock.Value = 0;           
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

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
    public partial class frmAgregarProducto : Form
    {
        private clsProductosCRUD productosBD = new clsProductosCRUD();
        private ToolTip toolTip1 = new ToolTip();

        public frmAgregarProducto()
        {
            InitializeComponent();

            numStock.TextAlign = HorizontalAlignment.Right;  
            numPrecio.TextAlign = HorizontalAlignment.Right;
            numStock.DecimalPlaces = 0; 
            numPrecio.DecimalPlaces = 2;
        }
        
        private void frmAgregarProducto_Load(object sender, EventArgs e)
        {
            CargarCategorias();
            CargarProductos();     
        }

        public void CargarCategorias()
        {
            DataTable categorias = productosBD.ObtenerCategorias();
            if (categorias != null)
            {
                DataRow row = categorias.NewRow();
                row["Nombre"] = "Seleccionar categoría";
                categorias.Rows.InsertAt(row, 0); 

                cmbCategorias.DisplayMember = "Nombre";
                cmbCategorias.ValueMember = "Id";
                cmbCategorias.DataSource = categorias;

                cmbCategorias.SelectedIndex = -1;
            }
        }

        public void CargarProductos()
        {
            DataTable productos = productosBD.ObtenerProductos();
            if (productos != null)
            {
                dgvProductos.DataSource = productos;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (ValidarCampos())
            {
                try
                {
                    clsProductos nuevoProducto = new clsProductos
                    {
                        Nombre = txtNombre.Text.Trim(),
                        Descripcion = txtDescripcion.Text.Trim(),
                        Precio = numPrecio.Value > 0 ? numPrecio.Value : 0,
                        Stock = numStock.Value > 0 ? (int)numStock.Value : 0,
                        CategoriaId = Convert.ToInt32(cmbCategorias.SelectedValue)  
                    };

                    productosBD.AgregarProducto(nuevoProducto);
                    CargarProductos();  
                    LimpiarCampos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocurrió un error: {ex.Message}");
                }
            }
        }

        private bool ValidarTexto(TextBox txt)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.BackColor = Color.FromArgb(255, 200, 200);  
                txt.ForeColor = Color.Black;    
                toolTip1.SetToolTip(txt, "Debe ingresar un valor");  
                return false;
            }
            else
            {
                txt.BackColor = Color.FromArgb(200, 255, 200);  
                txt.ForeColor = Color.Black;  
                toolTip1.SetToolTip(txt, "Campo correcto"); 
                return true;
            }
        }

        private bool ValidarNumero(NumericUpDown num)
        {
            if (num.Value <= 0)
            {
                num.BackColor = Color.FromArgb(255, 200, 200); 
                num.ForeColor = Color.Black;  
                toolTip1.SetToolTip(num, "Debe ingresar un valor mayor a 0");
                return false;
            }

            num.BackColor = Color.FromArgb(200, 255, 200);  
            num.ForeColor = Color.Black;  
            return true;
        }

        private bool ValidarCombo(ComboBox cmb)
        {
            if (cmb.SelectedIndex == -1) 
            {
                cmb.BackColor = Color.FromArgb(255, 200, 200);  
                cmb.ForeColor = Color.Black;
                toolTip1.SetToolTip(cmb, "Debe seleccionar una categoría");
                return false;
            }

            cmb.BackColor = Color.FromArgb(200, 255, 200); 
            cmb.ForeColor = Color.Black;
            return true;
        }

        private bool ValidarCampos()
        {
            if (!ValidarTexto(txtNombre)) return false;
            if (!ValidarTexto(txtDescripcion)) return false;
            if (!ValidarNumero(numPrecio)) return false;
            if (!ValidarNumero(numStock)) return false;
            if (!ValidarCombo(cmbCategorias)) return false;

            return true;
        }

        public void LimpiarCampos()
        {
            txtNombre.Clear();
            txtDescripcion.Clear();
            numPrecio.Value = 0;
            numStock.Value = 0;
            cmbCategorias.SelectedIndex = -1;
        }

        private void VerificarFormularioCompleto()
        {
            bool formularioCompleto =
                !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                !string.IsNullOrWhiteSpace(txtDescripcion.Text) &&
                numPrecio.Value > 0 &&
                numStock.Value > 0 &&
                cmbCategorias.SelectedIndex > -1;

            btnAgregar.Enabled = formularioCompleto;
        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {
            ValidarTexto(txtNombre);  
            VerificarFormularioCompleto();
        }

        private void txtDescripcion_TextChanged(object sender, EventArgs e)
        {
            ValidarTexto(txtDescripcion);  
            VerificarFormularioCompleto();
        }

        private void numPrecio_ValueChanged(object sender, EventArgs e)
        {
            ValidarNumero(numPrecio); 
            VerificarFormularioCompleto();
        }

        private void numStock_ValueChanged(object sender, EventArgs e)
        {
            ValidarNumero(numStock);  
            VerificarFormularioCompleto();
        }

        private void cmbCategorias_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidarCombo(cmbCategorias);  
            VerificarFormularioCompleto();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using pyInventario;
using System.Windows.Forms.DataVisualization.Charting;

namespace pryBordonInventarioMejorado
{
    public partial class frmMenuPrincipal : Form
    {
        private clsUsuario usuarioActual; 
        

        public frmMenuPrincipal(clsUsuario usuario)
        {
            InitializeComponent();
            usuarioActual = usuario;

            lblUsuario.Text = $"Bienvenido, {usuarioActual.NombreUsuario} ({usuarioActual.Rol})";

            AplicarPermisosPorRol();
        }

        private void frmMenuPrincipal_Load(object sender, EventArgs e)
        {
            conexionBD BD = new conexionBD();
            MostrarGraficoProductos();          
        }

        private void AplicarPermisosPorRol()
        {
            if (usuarioActual.Rol == "Empleado")
            {
                btnEliminar.Visible = false;
                btnModificarProducto.Visible = false;
            }
        }

        private void AbrirFormulario(Form formulario)
        {
            formulario.ShowDialog();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMaximizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            btnMaximizar.Visible = false;
            btnRestaurar.Visible = true;
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            btnRestaurar.Visible = false;
            btnMaximizar.Visible = true;
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // Movimiento del formulario
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void panelTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new frmProductos());
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new frmAgregarProducto());
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new frmEliminarProducto());
        }

        private void btnModificarProducto_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new frmModificarProducto());
        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
            clsReporteHTML reporte = new clsReporteHTML();
            reporte.GenerarVisualizarYDescargarReporteHTML();
        }

        private void btnCerrarSesion_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            frmInicioDeSesion login = new frmInicioDeSesion();
            login.Show();
        }

        private void btnContactos_Click(object sender, EventArgs e)
        {
            frmContactos Contactos = new frmContactos();
            Contactos.Show();      
        }

        private void MostrarGraficoProductos()
        {
            clsProductosCRUD crud = new clsProductosCRUD();
            DataTable datos = crud.ObtenerCantidadProductosPorCategoria();
            int total = crud.ObtenerTotalProductos();
            int bajoStock = crud.ObtenerProductosConBajoStock();

            chartProductos.Series.Clear();

            lblTotalProductos.Text = $"Total: {total} productos";
            lblBajoStock.Text = $"Con Bajo Stock: {bajoStock}";

            Series serie = new Series
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            foreach (DataRow fila in datos.Rows)
            {
                string categoria = fila["Categoria"].ToString();
                int cantidad = Convert.ToInt32(fila["Cantidad"]);
                serie.Points.AddXY(categoria, cantidad);
            }

            chartProductos.Series.Add(serie);
        }


        private void panelTitulo_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}


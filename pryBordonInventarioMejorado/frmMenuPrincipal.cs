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


namespace pryBordonInventarioMejorado
{
    public partial class frmMenuPrincipal : Form
    {
        public frmMenuPrincipal()
        {
            InitializeComponent();
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void Form1_Load(object sender, EventArgs e)
        {
            conexionBD BD = new conexionBD();            
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
            btnMaximizar .Visible = true;
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnModificarProducto_Click(object sender, EventArgs e)
        {
            frmModificarProducto v = new frmModificarProducto();
            v.ShowDialog();
        }
      
        private void panelTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            frmProductos v = new frmProductos();
            v.ShowDialog();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAgregarProducto v = new frmAgregarProducto();
            v.ShowDialog();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            frmEliminarProducto v = new frmEliminarProducto();
            v.ShowDialog();
        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
            clsReporteHTML reporte = new clsReporteHTML();
            reporte.GenerarVisualizarYDescargarReporteHTML();
        }   
    }
}

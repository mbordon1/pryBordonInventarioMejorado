﻿using pyInventario;
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

namespace pryBordonInventarioMejorado
{
    public partial class frmProductos : Form
    {
        public frmProductos()
        {
            InitializeComponent();
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void frmProductos_Load(object sender, EventArgs e)
        {
            conexionBD BD = new conexionBD();
            BD.mostrarProductos(dgvProductos);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
          this.Close(); 
        }

        private void btnMaximizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            btnMaximizar.Visible = false;
            btnRestaurar.Visible = true;
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            btnRestaurar.Visible = false;
            btnMaximizar.Visible = true;
        }

        private void panelTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void txtBuscarProducto_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string texto = txtBuscarProducto.Text.Trim();
                conexionBD BD = new conexionBD();

                if (string.IsNullOrEmpty(texto))
                {
                    BD.mostrarProductos(dgvProductos);
                }
                else
                {
                    BD.buscarProductoPorTexto(texto, dgvProductos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar producto: {ex.Message}");
            }
        }
    }
}

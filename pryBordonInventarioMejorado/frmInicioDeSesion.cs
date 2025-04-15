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
    public partial class frmInicioDeSesion : Form
    {
        private clsUsuarios usuarios;

        public frmInicioDeSesion()
        {
            InitializeComponent();
            usuarios = new clsUsuarios();
        }

        private void frmInicioDeSesion_Load(object sender, EventArgs e)
        {
            txtUsuario.Focus();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string nombreUsuario = txtUsuario.Text;
            string contrasena = txtContrasena.Text;

            clsUsuario usuario = usuarios.VerificarCredenciales(nombreUsuario, contrasena);

            if (usuario != null)
            {
                this.Hide();
                frmMenuPrincipal menuPrincipal = new frmMenuPrincipal(usuario);
                menuPrincipal.Show();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error de autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            frmRegistroUsuario Registrar = new frmRegistroUsuario();
            Registrar.Show();
            this.Hide();
        }

        private void chkMostrarCont_CheckedChanged(object sender, EventArgs e)
        {
            txtContrasena.PasswordChar = chkMostrarCont.Checked ? '\0' : '*';
        }

        private void txtContrasena_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnIngresar.PerformClick(); 
            }
        }
    }
}

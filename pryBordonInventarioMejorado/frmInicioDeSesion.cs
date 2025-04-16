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
            txtUsuario.Text = "Usuario";
            txtUsuario.ForeColor = Color.Gray;

            txtContrasena.Text = "Contraseña";
            txtContrasena.ForeColor = Color.Gray;
            txtContrasena.PasswordChar = '\0'; 

            txtUsuario.GotFocus += QuitarTextoUsuario;
            txtUsuario.LostFocus += PonerTextoUsuario;

            txtContrasena.GotFocus += QuitarTextoContrasena;
            txtContrasena.LostFocus += PonerTextoContrasena;
        }

        private async void btnIngresar_Click(object sender, EventArgs e)
        {
            string nombreUsuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text;

            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                lblEstado.Text = "Por favor, complete ambos campos.";
                lblEstado.ForeColor = Color.OrangeRed;
                await ShakeForm();
                return;
            }

            progressBarLogin.Visible = true;
            lblEstado.Text = "Verificando credenciales...";
            lblEstado.ForeColor = Color.Gray;
            Application.DoEvents(); 

            clsUsuario usuario = usuarios.VerificarCredenciales(nombreUsuario, contrasena);

            progressBarLogin.Visible = false;

            if (usuario != null)
            {
                lblEstado.Text = "Inicio de sesión exitoso.";
                lblEstado.ForeColor = Color.Green;
                this.Hide();
                frmMenuPrincipal menuPrincipal = new frmMenuPrincipal(usuario);
                menuPrincipal.FormClosed += (s, args) => this.Close();
                menuPrincipal.Show();
            }
            else
            {
                lblEstado.Text = "Usuario o contraseña incorrectos.";
                lblEstado.ForeColor = Color.Red;
                await ShakeForm();
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
            chkMostrarCont.Text = chkMostrarCont.Checked ? "Ocultar contraseña" : "Mostrar contraseña";
        }

        private void txtContrasena_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnIngresar.PerformClick(); 
            }
        }

        private void QuitarTextoUsuario(object sender, EventArgs e)
        {
            if (txtUsuario.Text == "Usuario")
            {
                txtUsuario.Text = "";
                txtUsuario.ForeColor = Color.Black;
            }
        }

        private void PonerTextoUsuario(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                txtUsuario.Text = "Usuario";
                txtUsuario.ForeColor = Color.Gray;
            }
        }

        private void QuitarTextoContrasena(object sender, EventArgs e)
        {
            if (txtContrasena.Text == "Contraseña")
            {
                txtContrasena.Text = "";
                txtContrasena.ForeColor = Color.Black;
                txtContrasena.PasswordChar = '*';
            }
        }

        private void PonerTextoContrasena(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                txtContrasena.PasswordChar = '\0';
                txtContrasena.Text = "Contraseña";
                txtContrasena.ForeColor = Color.Gray;
            }
        }
        private async Task ShakeForm()
        {
            var original = this.Location;
            var rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                this.Location = new Point(original.X + rnd.Next(-5, 5), original.Y + rnd.Next(-5, 5));
                await Task.Delay(20);
            }
            this.Location = original;
        }
    }
}

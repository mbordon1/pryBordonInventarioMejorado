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
    public partial class frmInicioDeSesion : Form
    {
        private clsUsuarios gestorUsuarios = new clsUsuarios();

        public frmInicioDeSesion()
        {
            InitializeComponent();
            gestorUsuarios = new clsUsuarios();
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
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || usuario == "Usuario" ||
                string.IsNullOrEmpty(contrasena) || contrasena == "Contraseña")
            {
                MessageBox.Show("Por favor, complete todos los campos para iniciar sesión.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            clsUsuario usuarioVerificado = gestorUsuarios.VerificarCredenciales(usuario, contrasena);

            if (usuarioVerificado != null)
            {
                MessageBox.Show("Bienvenido/a " + usuarioVerificado.NombreUsuario);

                gestorUsuarios.ActualizarUltimaConexion(usuarioVerificado.Id);

                txtUsuario.Clear();
                txtContrasena.Clear();

                this.Hide();
                frmMenuPrincipal frm = new frmMenuPrincipal(usuarioVerificado);
                frm.Show();
            }
            else
            {
                MessageBox.Show("Credenciales incorrectas o usuario bloqueado.", "Error de autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtContrasena.Clear();
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

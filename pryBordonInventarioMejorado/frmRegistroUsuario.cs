using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace pryBordonInventarioMejorado
{
    public partial class frmRegistroUsuario : Form
    {
        SqlConnection Conexion = new SqlConnection(@"Server=localhost;Database=Comercio;Trusted_Connection=True;");

        public frmRegistroUsuario()
        {
            InitializeComponent();
        }

        private void frmRegistroUsuario_Load(object sender, EventArgs e)
        {

        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            frmInicioDeSesion InicioDeSesion = new frmInicioDeSesion();
            InicioDeSesion.Show();
            this.Hide();
        }

        private void lblSalir_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void chkMostrarCont_CheckedChanged(object sender, EventArgs e)
        {
            txtContraseñaReg.PasswordChar = chkMostrarCont.Checked ? '\0' : '*';
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (txtUsuarioReg.Text == "" || txtContraseñaReg.Text == "")
            {
                MessageBox.Show("Por favor llene todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                clsUsuarios usuarios = new clsUsuarios();

                try
                {
                    if (usuarios.VerificarCredenciales(txtUsuarioReg.Text.Trim(), txtContraseñaReg.Text.Trim()) != null)
                    {
                        MessageBox.Show("El usuario ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        // registra el nuevo usuario con la contraseña en texto claro
                        using (SqlConnection conexion = new SqlConnection(@"Server=localhost;Database=Comercio;Trusted_Connection=True;"))
                        {
                            conexion.Open();
                            string query = "INSERT INTO Usuarios (Usuario, Contrasena, Rol) VALUES (@Usuario, @Contrasena, @Rol)";
                            SqlCommand comando = new SqlCommand(query, conexion);
                            comando.Parameters.AddWithValue("@Usuario", txtUsuarioReg.Text.Trim());
                            comando.Parameters.AddWithValue("@Contrasena", txtContraseñaReg.Text.Trim());
                            comando.Parameters.AddWithValue("@Rol", "Usuario");  //por defecto rol como usuario

                            comando.ExecuteNonQuery();
                        }

                        MessageBox.Show("Registro exitoso!", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        frmInicioDeSesion InicioDeSesion = new frmInicioDeSesion();
                        InicioDeSesion.Show();
                        this.Hide();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtContraseñaReg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnRegistrar.PerformClick(); 
            }
        }
    }
}




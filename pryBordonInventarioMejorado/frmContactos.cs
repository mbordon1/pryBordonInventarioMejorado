using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace pryBordonInventarioMejorado
{
    public partial class frmContactos : Form
    {
        clsContactosCRUD crudContactos = new clsContactosCRUD();
        DataTable tablaContactos;

        public frmContactos()
        {
            InitializeComponent();
        }

        private void frmContactos_Load(object sender, EventArgs e)
        {
            InicializarControles();
            CargarCategorias();
            CargarContactos();
        }

        private void InicializarControles()
        {
            cmbBuscar.TextChanged += cmbBuscar_TextChanged;
            cmbBuscar.SelectedIndex = -1;

            SimularPlaceholder(txtNombre, "Ej: Juan Pérez");
            SimularPlaceholder(txtTelefono, "Ej: 3511234567");
            SimularPlaceholder(txtCorreo, "Ej: ejemplo@mail.com");
        }

        private void SimularPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.ForeColor = Color.Gray;

            textBox.Enter += (sender, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        private void CargarCategorias()
        {
            DataTable categorias = crudContactos.ObtenerCategorias();
            cmbCategoria.DataSource = categorias;
            cmbCategoria.DisplayMember = "Nombre";
            cmbCategoria.ValueMember = "Id";
            cmbCategoria.SelectedIndex = -1;
        }

        private void CargarContactos()
        {
            tablaContactos = crudContactos.ObtenerContactos();
            dgvContactos.DataSource = tablaContactos;

            dgvContactos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvContactos.Columns["Id"].Visible = false;

            cmbBuscar.Items.Clear();
            HashSet<string> nombres = new HashSet<string>();
            foreach (DataRow fila in tablaContactos.Rows)
            {
                string nombre = fila["NombreApellido"].ToString();
                if (!nombres.Contains(nombre))
                {
                    nombres.Add(nombre);
                    cmbBuscar.Items.Add(nombre);
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            clsContacto nuevo = new clsContacto()
            {
                NombreApellido = txtNombre.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Correo = txtCorreo.Text.Trim(),
                CategoriaId = Convert.ToInt32(cmbCategoria.SelectedValue)
            };

            crudContactos.AgregarContacto(nuevo);
            MessageBox.Show("✅ Contacto agregado correctamente.", "Agregado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarCampos();
            CargarContactos();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                cmbCategoria.SelectedIndex == -1)
            {
                MessageBox.Show("⚠️ Completá todos los campos antes de agregar.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            cmbCategoria.SelectedIndex = -1;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvContactos.CurrentRow != null)
            {
                int id = Convert.ToInt32(dgvContactos.CurrentRow.Cells["Id"].Value);

                var confirm = MessageBox.Show("¿Estás seguro de que querés eliminar este contacto?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    crudContactos.EliminarContacto(id);
                    CargarContactos();
                }
            }
        }

        private void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            DataTable cambios = (DataTable)dgvContactos.DataSource;

            DataRow[] filasModificadas = cambios.Select("RowState = 'Modified'");

            if (filasModificadas.Length > 0)
            {
                crudContactos.ActualizarCambiosDesdeDataTable(cambios);
                MessageBox.Show("💾Cambios guardados correctamente", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information); 
            }
            else
            {
                MessageBox.Show("No hay cambios para guardar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cmbBuscar_TextChanged(object sender, EventArgs e)
        {
            string textoBusqueda = cmbBuscar.Text.Trim();
            if (!string.IsNullOrEmpty(textoBusqueda))
                dgvContactos.DataSource = crudContactos.BuscarPorNombre(textoBusqueda);
            else
                CargarContactos();

            if (dgvContactos.Columns.Contains("Id"))
                dgvContactos.Columns["Id"].Visible = false;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnVerTodos_Click(object sender, EventArgs e)
        {
            CargarContactos();
        }

        private void dgvContactos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvContactos.Columns[e.ColumnIndex].Name != "Id")
            {
                DataGridViewRow row = dgvContactos.Rows[e.RowIndex];
                if (row != null)
                {
                    DataRow dataRow = ((DataRowView)row.DataBoundItem).Row;
                    dataRow.SetModified();
                }
            }
        }

        private void dgvContactos_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvContactos.Rows[e.RowIndex];
            DataRow dataRow = ((DataRowView)row.DataBoundItem).Row;

            dataRow.AcceptChanges();
        }
    }
}





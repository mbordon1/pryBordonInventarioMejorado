using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace pryBordonInventarioMejorado
{
    public partial class frmContactos : Form
    {
        clsContactosCRUD crudContactos = new clsContactosCRUD();
        DataTable tablaContactos;
        DataTable contactosOriginales;

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

            contactosOriginales = tablaContactos.Copy();

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
            bool cambiosDetectados = false;

            foreach (DataGridViewRow fila in dgvContactos.Rows)
            {
                if (fila.IsNewRow) continue;

                int id = Convert.ToInt32(fila.Cells["Id"].Value);
                var contactoOriginal = contactosOriginales.Rows
                    .Cast<DataRow>()
                    .FirstOrDefault(r => Convert.ToInt32(r["Id"]) == id);

                if (contactoOriginal != null)
                {
                    bool haCambiado = false;

                    string nombre = fila.Cells["NombreApellido"].Value?.ToString();
                    string telefono = fila.Cells["Telefono"].Value?.ToString();
                    string correo = fila.Cells["Correo"].Value?.ToString();
                    string categoriaNombre = fila.Cells["Categoria"].Value?.ToString();
                    int categoriaId = crudContactos.ObtenerCategoriaIdPorNombre(categoriaNombre);

                    if (contactoOriginal["NombreApellido"].ToString() != nombre ||
                        contactoOriginal["Telefono"].ToString() != telefono ||
                        contactoOriginal["Correo"].ToString() != correo ||
                        Convert.ToInt32(contactoOriginal["CategoriaId"]) != categoriaId)
                    {
                        haCambiado = true;
                    }

                    if (haCambiado)
                    {
                        cambiosDetectados = true;

                        clsContacto contactoModificado = new clsContacto()
                        {
                            Id = id,
                            NombreApellido = nombre,
                            Telefono = telefono,
                            Correo = correo,
                            CategoriaId = categoriaId
                        };

                        try
                        {
                            crudContactos.ModificarContacto(contactoModificado);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al guardar cambios: " + ex.Message);
                            return;
                        }
                    }
                }
            }
            if (cambiosDetectados)
            {
                MessageBox.Show("✅ Cambios guardados correctamente.");
                CargarContactos();
            }
            else
            {
                MessageBox.Show("No se detectaron cambios.");
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
    }
}
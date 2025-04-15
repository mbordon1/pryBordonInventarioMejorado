using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pryBordonInventarioMejorado
{
    public class clsContacto
    {
        public int Id { get; set; }
        public string NombreApellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public int CategoriaId { get; set; }
    }
}

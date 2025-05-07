using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pryBordonInventarioMejorado
{
    public class clsUsuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
        public string Rol { get; set; }
        public bool Estado { get; set; } // true = activo || false = bloqueado
        public DateTime? FechaUltimaConexion { get; set; }

        public clsUsuario() { }
    }
}

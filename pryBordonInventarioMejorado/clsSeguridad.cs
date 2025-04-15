using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace pryBordonInventarioMejorado
{
    public class clsSeguridad
    {
        public static string GenerarSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);
                return Convert.ToBase64String(salt);
            }
        }

        public static string ObtenerHashConSalt(string contrasena, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] contrasenaBytes = Encoding.UTF8.GetBytes(contrasena);
                byte[] saltBytes = Convert.FromBase64String(salt);

                byte[] contrasenaConSalt = new byte[contrasenaBytes.Length + saltBytes.Length];
                Array.Copy(contrasenaBytes, 0, contrasenaConSalt, 0, contrasenaBytes.Length);
                Array.Copy(saltBytes, 0, contrasenaConSalt, contrasenaBytes.Length, saltBytes.Length);

                byte[] hashBytes = sha256.ComputeHash(contrasenaConSalt);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerificarContrasena(string contrasena, string salt, string hashAlmacenado)
        {
            string hashGenerado = ObtenerHashConSalt(contrasena, salt);
            return hashGenerado.Equals(hashAlmacenado);
        }
    }
}

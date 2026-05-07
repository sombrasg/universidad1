using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class DepartamentosController : Controller
    {
        private readonly string _cadenaConexion;

        public DepartamentosController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        public IActionResult Index()
        {
            List<Departamento> lista = new List<Departamento>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                // Ahora pedimos las 5 columnas que ya tienes en MySQL
                string query = "SELECT id, nombre_departamento, extension_telefonica, nombre_responsable, ubicacion FROM departamentos";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Departamento
                            {
                                Id = reader.GetInt32("id"),
                                NombreDepartamento = reader.GetString("nombre_departamento"),
                                ExtensionTelefonica = reader.IsDBNull(reader.GetOrdinal("extension_telefonica")) ? "N/A" : reader.GetString("extension_telefonica"),
                                NombreResponsable = reader.IsDBNull(reader.GetOrdinal("nombre_responsable")) ? "Sin asignar" : reader.GetString("nombre_responsable"),
                                Ubicacion = reader.GetString("ubicacion")
                            });
                        }
                    }
                }
            }
            return View(lista);
        }
    }
}
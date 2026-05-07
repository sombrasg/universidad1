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

        // GET: Departamentos (Muestra la lista)
        public IActionResult Index()
        {
            List<Departamento> lista = new List<Departamento>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
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

        // --- INICIA PROCEDIMIENTO PARA GUARDAR NUEVO DEPARTAMENTO ---

        // 1. GET: Departamentos/Create (Muestra el formulario vacío)
        public IActionResult Create()
        {
            return View();
        }

        // 2. POST: Departamentos/Create (Guarda en MySQL)
        [HttpPost]
        public IActionResult Create(Departamento departamento)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = @"INSERT INTO departamentos (nombre_departamento, extension_telefonica, nombre_responsable, ubicacion) 
                                 VALUES (@nombre, @extension, @responsable, @ubicacion)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", departamento.NombreDepartamento);
                    cmd.Parameters.AddWithValue("@extension", departamento.ExtensionTelefonica);
                    cmd.Parameters.AddWithValue("@responsable", departamento.NombreResponsable);
                    cmd.Parameters.AddWithValue("@ubicacion", departamento.Ubicacion);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class AlumnosController : Controller
    {
        private readonly string _cadenaConexion;

        public AlumnosController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        // GET: Alumnos (Muestra la lista con SELECT)
        public IActionResult Index()
        {
            List<Alumno> listaAlumnos = new List<Alumno>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT id, matricula, CONCAT(nombre, ' ', apellido_paterno, ' ', IFNULL(apellido_materno, '')) AS NombreCompleto, correo FROM alumnos";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaAlumnos.Add(new Alumno
                            {
                                Id = reader.GetInt32("id"),
                                Matricula = reader.GetString("matricula"),
                                NombreCompleto = reader.GetString("NombreCompleto"),
                                Correo = reader.GetString("correo")
                            });
                        }
                    }
                }
            }

            return View(listaAlumnos);
        }

        // 1. GET: Alumnos/Create (Muestra el formulario web en blanco)
        public IActionResult Create()
        {
            return View();
        }

        // 2. POST: Alumnos/Create (Recibe los datos del formulario y hace el INSERT en MySQL)
        [HttpPost]
        public IActionResult Create(Alumno alumno)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();

                // Query documentable para tu proyecto: Acción de insertar[cite: 1]
                string query = "INSERT INTO alumnos (matricula, nombre, apellido_paterno, apellido_materno, correo) VALUES (@matricula, @nombre, @paterno, @materno, @correo)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    // Asignamos los valores usando parámetros por seguridad
                    cmd.Parameters.AddWithValue("@matricula", alumno.Matricula);
                    cmd.Parameters.AddWithValue("@nombre", alumno.Nombre);
                    cmd.Parameters.AddWithValue("@paterno", alumno.ApellidoPaterno);

                    // Manejo especial por si el alumno no tiene apellido materno (acepta nulos)
                    if (string.IsNullOrEmpty(alumno.ApellidoMaterno))
                    {
                        cmd.Parameters.AddWithValue("@materno", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@materno", alumno.ApellidoMaterno);
                    }

                    cmd.Parameters.AddWithValue("@correo", alumno.Correo);

                    // Ejecutamos la inserción en la base de datos
                    cmd.ExecuteNonQuery();
                }
            }

            // Después de guardar exitosamente, lo redirigimos a la tabla principal (Index)
            return RedirectToAction("Index");
        }
    }
}
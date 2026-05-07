using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class InscripcionesController : Controller
    {
        private readonly string _cadenaConexion;

        public InscripcionesController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        // Método auxiliar para cargar los 3 menús desplegables
        private void CargarListasDesplegables()
        {
            List<SelectListItem> alumnos = new();
            List<SelectListItem> carreras = new();
            List<SelectListItem> periodos = new();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();

                // 1. Alumnos (Mostramos Matrícula + Nombre para que sea fácil identificarlo)
                using (MySqlCommand cmd = new MySqlCommand("SELECT id, CONCAT(matricula, ' - ', nombre, ' ', apellido_paterno) AS info_alumno FROM alumnos", conexion))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                    while (reader.Read()) alumnos.Add(new SelectListItem { Value = reader.GetInt32("id").ToString(), Text = reader.GetString("info_alumno") });

                // 2. Carreras
                using (MySqlCommand cmd = new MySqlCommand("SELECT id, nombre_carrera FROM carreras", conexion))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                    while (reader.Read()) carreras.Add(new SelectListItem { Value = reader.GetInt32("id").ToString(), Text = reader.GetString("nombre_carrera") });

                // 3. Periodos
                using (MySqlCommand cmd = new MySqlCommand("SELECT id, clave_periodo FROM periodos_academicos", conexion))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                    while (reader.Read()) periodos.Add(new SelectListItem { Value = reader.GetInt32("id").ToString(), Text = reader.GetString("clave_periodo") });
            }

            ViewBag.Alumnos = alumnos;
            ViewBag.Carreras = carreras;
            ViewBag.Periodos = periodos;
        }

        // --- 1. LECTURA (INDEX) ---
        public IActionResult Index()
        {
            List<Inscripcion> lista = new List<Inscripcion>();
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = @"
                    SELECT 
                        i.id, 
                        a.matricula, CONCAT(a.nombre, ' ', a.apellido_paterno) AS nombre_alumno, 
                        c.nombre_carrera, 
                        p.clave_periodo 
                    FROM inscripciones i
                    INNER JOIN alumnos a ON i.alumno_id = a.id
                    INNER JOIN carreras c ON i.carrera_id = c.id
                    INNER JOIN periodos_academicos p ON i.periodo_id = p.id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Inscripcion
                            {
                                Id = reader.GetInt32("id"),
                                Matricula = reader.GetString("matricula"),
                                NombreAlumno = reader.GetString("nombre_alumno"),
                                NombreCarrera = reader.GetString("nombre_carrera"),
                                ClavePeriodo = reader.GetString("clave_periodo")
                            });
                        }
                    }
                }
            }
            return View(lista);
        }

        // --- 2. CREAR (CREATE) ---
        public IActionResult Create()
        {
            CargarListasDesplegables();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Inscripcion inscripcion)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "INSERT INTO inscripciones (alumno_id, carrera_id, periodo_id) VALUES (@alumId, @carrId, @perId)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@alumId", inscripcion.AlumnoId);
                    cmd.Parameters.AddWithValue("@carrId", inscripcion.CarreraId);
                    cmd.Parameters.AddWithValue("@perId", inscripcion.PeriodoId);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // --- 3. EDITAR (EDIT) ---
        public IActionResult Edit(int id)
        {
            Inscripcion inscripcion = new Inscripcion();
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT * FROM inscripciones WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inscripcion.Id = reader.GetInt32("id");
                            inscripcion.AlumnoId = reader.GetInt32("alumno_id");
                            inscripcion.CarreraId = reader.GetInt32("carrera_id");
                            inscripcion.PeriodoId = reader.GetInt32("periodo_id");
                        }
                    }
                }
            }
            CargarListasDesplegables();
            return View(inscripcion);
        }

        [HttpPost]
        public IActionResult Edit(Inscripcion inscripcion)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "UPDATE inscripciones SET alumno_id=@alumId, carrera_id=@carrId, periodo_id=@perId WHERE id=@id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", inscripcion.Id);
                    cmd.Parameters.AddWithValue("@alumId", inscripcion.AlumnoId);
                    cmd.Parameters.AddWithValue("@carrId", inscripcion.CarreraId);
                    cmd.Parameters.AddWithValue("@perId", inscripcion.PeriodoId);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
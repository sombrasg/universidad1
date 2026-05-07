using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class AsistenciaController : Controller
    {
        private readonly string _cadenaConexion;

        public AsistenciaController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        // --- 1. LISTADO (INDEX) ---
        public IActionResult Index()
        {
            List<Asistencia> lista = new();
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                string query = @"SELECT a.*, al.nombre, g.clave_grupo, m.nombre_materia 
                                 FROM asistencia a
                                 JOIN alumnos al ON a.alumno_id = al.id
                                 JOIN grupos g ON a.grupo_id = g.id
                                 JOIN materias m ON g.materia_id = m.id
                                 ORDER BY a.fecha DESC";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                using (MySqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new Asistencia
                        {
                            Id = (int)r["id"],
                            Fecha = (DateTime)r["fecha"],
                            Estado = r["estado"].ToString(),
                            Observaciones = r["observaciones"].ToString(),
                            NombreAlumno = r["nombre"].ToString(),
                            ClaveGrupo = r["clave_grupo"].ToString(),
                            NombreMateria = r["nombre_materia"].ToString()
                        });
                    }
                }
            }
            return View(lista);
        }

        // --- 2. CAPTURA (CREATE) ---
        public IActionResult Create()
        {
            List<SelectListItem> grupos = new();
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                string q = "SELECT g.id, g.clave_grupo, m.nombre_materia FROM grupos g JOIN materias m ON g.materia_id = m.id";
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                using (MySqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) grupos.Add(new SelectListItem { Value = r["id"].ToString(), Text = $"{r["clave_grupo"]} - {r["nombre_materia"]}" });
            }
            ViewBag.Grupos = grupos;
            return View();
        }

        // --- MÉTODO AJAX: Obtiene solo alumnos del grupo seleccionado ---
        [HttpGet]
        public JsonResult GetAlumnosPorGrupo(int grupoId)
        {
            List<SelectListItem> alumnos = new();
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                string q = @"SELECT al.id, al.nombre FROM alumnos al 
                             JOIN asignacion_alumnos_grupo aag ON al.id = aag.alumno_id 
                             WHERE aag.grupo_id = @gid";
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    cmd.Parameters.AddWithValue("@gid", grupoId);
                    using (MySqlDataReader r = cmd.ExecuteReader())
                        while (r.Read()) alumnos.Add(new SelectListItem { Value = r["id"].ToString(), Text = r["nombre"].ToString() });
                }
            }
            return Json(alumnos);
        }

        [HttpPost]
        public IActionResult Create(Asistencia a)
        {
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                string q = "INSERT INTO asistencia (alumno_id, grupo_id, fecha, estado, observaciones) VALUES (@alu, @gru, @fec, @est, @obs)";
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    cmd.Parameters.AddWithValue("@alu", a.AlumnoId);
                    cmd.Parameters.AddWithValue("@gru", a.GrupoId);
                    cmd.Parameters.AddWithValue("@fec", a.Fecha);
                    cmd.Parameters.AddWithValue("@est", a.Estado);
                    cmd.Parameters.AddWithValue("@obs", a.Observaciones ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
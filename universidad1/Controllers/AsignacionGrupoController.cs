using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    [Route("AsignacionGrupo")] // Ruta base del controlador
    public class AsignacionesController : Controller
    {
        private readonly string _cadenaConexion;

        public AsignacionesController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        private void CargarListas()
        {
            List<SelectListItem> alumnos = new();
            List<SelectListItem> grupos = new();

            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT id, matricula, nombre FROM alumnos", con))
                using (MySqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) alumnos.Add(new SelectListItem { Value = r["id"].ToString(), Text = $"{r["matricula"]} - {r["nombre"]}" });

                string qGrupos = @"SELECT g.id, g.clave_grupo, m.nombre_materia 
                                   FROM grupos g 
                                   JOIN materias m ON g.materia_id = m.id";
                using (MySqlCommand cmd = new MySqlCommand(qGrupos, con))
                using (MySqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) grupos.Add(new SelectListItem { Value = r["id"].ToString(), Text = $"{r["clave_grupo"]} (@{r["nombre_materia"]})" });
            }
            ViewBag.Alumnos = alumnos;
            ViewBag.Grupos = grupos;
        }

        // --- ACCIONES ---

        [HttpGet("")] // Esto hace que responda a: /AsignacionGrupo
        public IActionResult Index()
        {
            List<AsignacionAlumnoGrupo> lista = new();
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                string query = @"SELECT aag.id, a.nombre, a.matricula, g.clave_grupo, m.nombre_materia 
                                 FROM asignacion_alumnos_grupo aag
                                 JOIN alumnos a ON aag.alumno_id = a.id
                                 JOIN grupos g ON aag.grupo_id = g.id
                                 JOIN materias m ON g.materia_id = m.id";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                using (MySqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new AsignacionAlumnoGrupo
                        {
                            Id = (int)r["id"],
                            NombreAlumno = r["nombre"].ToString(),
                            Matricula = r["matricula"].ToString(),
                            ClaveGrupo = r["clave_grupo"].ToString(),
                            NombreMateria = r["nombre_materia"].ToString()
                        });
                    }
                }
            }
            return View(lista);
        }

        [HttpGet("Create")] // Esto hace que responda a: /AsignacionGrupo/Create
        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        [HttpPost("Create")] // La versión POST también vive en: /AsignacionGrupo/Create
        public IActionResult Create(AsignacionAlumnoGrupo a)
        {
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();

                string check = "SELECT COUNT(*) FROM asignacion_alumnos_grupo WHERE alumno_id = @alu AND grupo_id = @gru";
                using (MySqlCommand cmdCheck = new MySqlCommand(check, con))
                {
                    cmdCheck.Parameters.AddWithValue("@alu", a.AlumnoId);
                    cmdCheck.Parameters.AddWithValue("@gru", a.GrupoId);
                    if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                    {
                        ModelState.AddModelError("", "Este alumno ya pertenece a este grupo.");
                        CargarListas();
                        return View(a);
                    }
                }

                string q = "INSERT INTO asignacion_alumnos_grupo (alumno_id, grupo_id) VALUES (@alu, @gru)";
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    cmd.Parameters.AddWithValue("@alu", a.AlumnoId);
                    cmd.Parameters.AddWithValue("@gru", a.GrupoId);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
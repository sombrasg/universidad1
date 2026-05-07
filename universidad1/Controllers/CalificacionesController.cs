using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class CalificacionesController : Controller
    {
        private readonly string _cadenaConexion;

        public CalificacionesController(string cadenaConexion)
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

                // Traemos los grupos con el nombre de la materia para que el docente sepa qué califica
                string qGrupos = @"SELECT g.id, g.clave_grupo, m.nombre_materia 
                                   FROM grupos g 
                                   JOIN materias m ON g.materia_id = m.id";
                using (MySqlCommand cmd = new MySqlCommand(qGrupos, con))
                using (MySqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) grupos.Add(new SelectListItem { Value = r["id"].ToString(), Text = $"{r["clave_grupo"]} - {r["nombre_materia"]}" });
            }
            ViewBag.Alumnos = alumnos;
            ViewBag.Grupos = grupos;
        }

        public IActionResult Index()
        {
            List<Calificacion> lista = new();
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                string query = @"SELECT c.*, a.nombre, g.clave_grupo, m.nombre_materia 
                                 FROM calificaciones c
                                 JOIN alumnos a ON c.alumno_id = a.id
                                 JOIN grupos g ON c.grupo_id = g.id
                                 JOIN materias m ON g.materia_id = m.id";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                using (MySqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new Calificacion
                        {
                            Id = (int)r["id"],
                            Parcial1 = (decimal)r["parcial_1"],
                            Parcial2 = (decimal)r["parcial_2"],
                            PromedioFinal = (decimal)r["promedio_final"],
                            EstatusAprobacion = r["estatus_aprobacion"].ToString(),
                            NombreAlumno = r["nombre"].ToString(),
                            ClaveGrupo = r["clave_grupo"].ToString(),
                            NombreMateria = r["nombre_materia"].ToString()
                        });
                    }
                }
            }
            return View(lista);
        }

        public IActionResult Create() { CargarListas(); return View(); }

        [HttpPost]
        public IActionResult Create(Calificacion c)
        {
            // Lógica de Negocio: Cálculo automático
            c.PromedioFinal = (c.Parcial1 + c.Parcial2) / 2;
            c.EstatusAprobacion = c.PromedioFinal >= 6 ? "Aprobado" : "Reprobado";

            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                string q = @"INSERT INTO calificaciones (alumno_id, grupo_id, parcial_1, parcial_2, promedio_final, estatus_aprobacion, fecha_registro) 
                             VALUES (@alu, @gru, @p1, @p2, @prom, @est, NOW())";
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    cmd.Parameters.AddWithValue("@alu", c.AlumnoId);
                    cmd.Parameters.AddWithValue("@gru", c.GrupoId);
                    cmd.Parameters.AddWithValue("@p1", c.Parcial1);
                    cmd.Parameters.AddWithValue("@p2", c.Parcial2);
                    cmd.Parameters.AddWithValue("@prom", c.PromedioFinal);
                    cmd.Parameters.AddWithValue("@est", c.EstatusAprobacion);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
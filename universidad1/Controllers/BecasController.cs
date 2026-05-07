using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class BecasController : Controller
    {
        private readonly string _cadenaConexion;

        public BecasController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        private void CargarListas()
        {
            List<SelectListItem> alumnos = new();
            List<SelectListItem> periodos = new();

            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT id, matricula, nombre FROM alumnos", con))
                using (MySqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) alumnos.Add(new SelectListItem { Value = r["id"].ToString(), Text = $"{r["matricula"]} - {r["nombre"]}" });

                using (MySqlCommand cmd = new MySqlCommand("SELECT id, clave_periodo FROM periodos_academicos", con))
                using (MySqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) periodos.Add(new SelectListItem { Value = r["id"].ToString(), Text = r["clave_periodo"].ToString() });
            }
            ViewBag.Alumnos = alumnos;
            ViewBag.Periodos = periodos;
        }

        // --- MÉTODO API PARA AJAX: Obtiene el promedio del alumno ---
        [HttpGet]
        public JsonResult GetPromedioAlumno(int id)
        {
            decimal promedio = 0;
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                // Buscamos el promedio más reciente en la tabla de calificaciones
                string query = "SELECT AVG(promedio_final) FROM calificaciones WHERE alumno_id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var result = cmd.ExecuteScalar();
                    promedio = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
            return Json(new { promedio = promedio });
        }

        public IActionResult Index()
        {
            List<BecaAcademica> lista = new();
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                string query = @"SELECT b.*, a.nombre, p.clave_periodo 
                                 FROM becas_academicas b
                                 JOIN alumnos a ON b.alumno_id = a.id
                                 JOIN periodos_academicos p ON b.periodo_id = p.id";
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                using (MySqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new BecaAcademica
                        {
                            Id = (int)r["id"],
                            NombreBeca = r["nombre_beca"].ToString(),
                            PorcentajeDescuento = (decimal)r["porcentaje_descuento"],
                            EstatusActiva = Convert.ToBoolean(r["estatus_activa"]),
                            NombreAlumno = r["nombre"].ToString(),
                            ClavePeriodo = r["clave_periodo"].ToString()
                        });
                    }
                }
            }
            return View(lista);
        }

        public IActionResult Create() { CargarListas(); return View(); }

        [HttpPost]
        public IActionResult Create(BecaAcademica b)
        {
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                string q = @"INSERT INTO becas_academicas (alumno_id, nombre_beca, porcentaje_descuento, periodo_id, estatus_activa) 
                             VALUES (@alu, @nom, @por, @per, @est)";
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    cmd.Parameters.AddWithValue("@alu", b.AlumnoId);
                    cmd.Parameters.AddWithValue("@nom", b.NombreBeca);
                    cmd.Parameters.AddWithValue("@por", b.PorcentajeDescuento);
                    cmd.Parameters.AddWithValue("@per", b.PeriodoId);
                    cmd.Parameters.AddWithValue("@est", b.EstatusActiva);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class PagosController : Controller
    {
        private readonly string _cadenaConexion;

        public PagosController(string cadenaConexion)
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

        [HttpGet]
        public JsonResult GetDescuentoAlumno(int alumnoId, int periodoId)
        {
            decimal porcentaje = 0;
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                // Buscamos si existe una beca ACTIVA para este alumno en este periodo específico
                string query = @"SELECT porcentaje_descuento 
                                 FROM becas_academicas 
                                 WHERE alumno_id = @alu AND periodo_id = @per AND estatus_activa = 1 
                                 LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@alu", alumnoId);
                    cmd.Parameters.AddWithValue("@per", periodoId);
                    var result = cmd.ExecuteScalar();
                    porcentaje = result != null ? Convert.ToDecimal(result) : 0;
                }
            }
            return Json(new { porcentaje = porcentaje });
        }

        // --- LISTADO DE PAGOS (INDEX) ---
        public IActionResult Index()
        {
            List<Pago> lista = new();
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                string query = @"SELECT p.*, a.nombre, a.matricula, per.clave_periodo 
                                 FROM pagos p
                                 JOIN alumnos a ON p.alumno_id = a.id
                                 JOIN periodos_academicos per ON p.periodo_id = per.id
                                 ORDER BY p.fecha_pago DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                using (MySqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new Pago
                        {
                            Id = (int)r["id"],
                            Concepto = r["concepto"].ToString(),
                            Monto = (decimal)r["monto"],
                            FechaPago = (DateTime)r["fecha_pago"],
                            MetodoPago = r["metodo_pago"].ToString(),
                            ReferenciaBancaria = r["referencia_bancaria"].ToString(),
                            NombreAlumno = r["nombre"].ToString(),
                            Matricula = r["matricula"].ToString(),
                            ClavePeriodo = r["clave_periodo"].ToString()
                        });
                    }
                }
            }
            return View(lista);
        }

        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        // --- PROCESAR EL PAGO (POST) ---
        [HttpPost]
        public IActionResult Create(Pago p)
        {
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                // Usamos NOW() para que la fecha sea la del servidor y sea inalterable
                string q = @"INSERT INTO pagos (alumno_id, periodo_id, concepto, monto, fecha_pago, metodo_pago, referencia_bancaria) 
                             VALUES (@alu, @per, @con, @mon, NOW(), @met, @ref)";

                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    cmd.Parameters.AddWithValue("@alu", p.AlumnoId);
                    cmd.Parameters.AddWithValue("@per", p.PeriodoId);
                    cmd.Parameters.AddWithValue("@con", p.Concepto);
                    cmd.Parameters.AddWithValue("@mon", p.Monto); // Aquí ya llegará el monto con descuento aplicado desde la vista
                    cmd.Parameters.AddWithValue("@met", p.MetodoPago);
                    cmd.Parameters.AddWithValue("@ref", p.ReferenciaBancaria);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class HorariosController : Controller
    {
        private readonly string _cadenaConexion;

        public HorariosController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        private void CargarListas()
        {
            List<SelectListItem> grupos = new();
            List<SelectListItem> aulas = new();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                // Usamos clave_grupo (con 'o')
                using (MySqlCommand cmd = new MySqlCommand("SELECT id, clave_grupo FROM grupos", conexion))
                using (MySqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) grupos.Add(new SelectListItem { Value = r["id"].ToString(), Text = r["clave_grupo"].ToString() });

                using (MySqlCommand cmd = new MySqlCommand("SELECT id, codigo_aula FROM Aulas", conexion))
                using (MySqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) aulas.Add(new SelectListItem { Value = r["id"].ToString(), Text = r["codigo_aula"].ToString() });
            }
            ViewBag.Grupos = grupos;
            ViewBag.Aulas = aulas;
        }

        public IActionResult Index()
        {
            List<Horario> lista = new();
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                // Ahora que agregaste la columna aula_id a horarios, h.aula_id ya no dará error
                string query = @"SELECT h.*, g.clave_grupo, a.codigo_aula 
                                 FROM horarios h
                                 JOIN grupos g ON h.grupo_id = g.id
                                 JOIN Aulas a ON h.aula_id = a.id";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                using (MySqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new Horario
                        {
                            Id = Convert.ToInt32(r["id"]),
                            DiaSemana = r["dia_semana"].ToString(),
                            HoraInicio = (TimeSpan)r["hora_inicio"],
                            HoraFin = (TimeSpan)r["hora_fin"],
                            ClaveGrupo = r["clave_grupo"].ToString(),
                            CodigoAula = r["codigo_aula"].ToString()
                        });
                    }
                }
            }
            return View(lista);
        }

        public IActionResult Create() { CargarListas(); return View(); }

        [HttpPost]
        public IActionResult Create(Horario h)
        {
            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                con.Open();
                // Esta inserción ahora funcionará porque ya existe la columna aula_id
                string q = "INSERT INTO horarios (grupo_id, aula_id, dia_semana, hora_inicio, hora_fin) VALUES (@g, @a, @d, @hi, @hf)";
                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    cmd.Parameters.AddWithValue("@g", h.GrupoId);
                    cmd.Parameters.AddWithValue("@a", h.AulaId);
                    cmd.Parameters.AddWithValue("@d", h.DiaSemana);
                    cmd.Parameters.AddWithValue("@hi", h.HoraInicio);
                    cmd.Parameters.AddWithValue("@hf", h.HoraFin);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
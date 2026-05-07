using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class BusquedaController : Controller
    {
        private readonly string _con;

        public BusquedaController(string cadenaConexion) => _con = cadenaConexion;

        public IActionResult Index(string q)
        {
            var model = new BusquedaViewModel { TerminoBusqueda = q };

            using (MySqlConnection con = new MySqlConnection(_con))
            {
                con.Open();

                if (!string.IsNullOrEmpty(q))
                {
                    string sql = @"SELECT a.id, CONCAT(a.nombre, ' ', a.apellido_paterno) AS full_nombre, c.nombre_carrera 
                                 FROM alumnos a 
                                 JOIN inscripciones i ON a.id = i.alumno_id
                                 JOIN carreras c ON i.carrera_id = c.id 
                                 WHERE a.nombre LIKE @q OR a.apellido_paterno LIKE @q";

                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@q", "%" + q + "%");
                        using (var r = cmd.ExecuteReader())
                            while (r.Read()) model.Resultados.Add(new AlumnoResultado
                            {
                                Id = (int)r["id"],
                                Nombre = r["full_nombre"].ToString(),
                                Carrera = r["nombre_carrera"].ToString()
                            });
                    }
                }

                string sqlH = @"SELECT a.id, CONCAT(a.nombre, ' ', a.apellido_paterno) AS full_nombre, c.nombre_carrera 
                                FROM historial_busqueda h 
                                JOIN alumnos a ON h.alumno_id = a.id 
                                JOIN inscripciones i ON a.id = i.alumno_id
                                JOIN carreras c ON i.carrera_id = c.id 
                                ORDER BY h.fecha_consulta DESC LIMIT 5";

                using (MySqlCommand cmd = new MySqlCommand(sqlH, con))
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) model.HistorialReciente.Add(new AlumnoResultado
                    {
                        Id = (int)r["id"],
                        Nombre = r["full_nombre"].ToString(),
                        Carrera = r["nombre_carrera"].ToString()
                    });
            }
            return View(model);
        }

        public IActionResult RegistrarConsulta(int id)
        {
            using (MySqlConnection con = new MySqlConnection(_con))
            {
                con.Open();
                string sql = "INSERT INTO historial_busqueda (alumno_id) VALUES (@id)";
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index", "Alumnos");
        }
    }
}
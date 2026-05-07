using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            List<Inscripcion> lista = new List<Inscripcion>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                // TRIPLE JOIN: Conectamos inscripciones con alumnos, carreras y periodos
                string query = @"
                    SELECT 
                        i.id, 
                        CONCAT(a.nombre, ' ', a.apellido_paterno) AS nombre_alumno,
                        c.nombre_carrera,
                        p.clave_periodo
                    FROM inscripciones i
                    INNER JOIN alumnos a ON i.alumno_id = a.id
                    INNER JOIN carreras c ON i.carrera_id = c.id
                    INNER JOIN periodos_academicos p ON i.periodo_id = p.id
                    ORDER BY p.clave_periodo DESC, a.apellido_paterno ASC";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Inscripcion
                            {
                                Id = reader.GetInt32("id"),
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
    }
}
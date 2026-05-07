using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            List<Horario> lista = new List<Horario>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                // Usamos GROUP_CONCAT para unir todos los horarios de la semana en una sola celda
                string query = @"
                    SELECT 
                        g.id AS grupo_id,
                        g.clave_grupo, 
                        m.nombre_materia, 
                        CONCAT(p.nombre, ' ', p.apellido_paterno) AS nombre_profesor,
                        GROUP_CONCAT(CONCAT(h.dia_semana, ' (', DATE_FORMAT(h.hora_inicio, '%H:%i'), ' - ', DATE_FORMAT(h.hora_fin, '%H:%i'), ')') SEPARATOR '|') AS dias_y_horas
                    FROM horarios h
                    INNER JOIN grupos g ON h.grupo_id = g.id
                    INNER JOIN materias m ON g.materia_id = m.id
                    INNER JOIN profesores p ON g.profesor_id = p.id
                    GROUP BY g.id, g.clave_grupo, m.nombre_materia, p.nombre, p.apellido_paterno";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Horario
                            {
                                GrupoId = reader.GetInt32("grupo_id"),
                                ClaveGrupo = reader.GetString("clave_grupo"),
                                NombreMateria = reader.GetString("nombre_materia"),
                                NombreProfesor = reader.GetString("nombre_profesor"),
                                DiasYHoras = reader.IsDBNull(reader.GetOrdinal("dias_y_horas")) ? "Sin asignar" : reader.GetString("dias_y_horas")
                            });
                        }
                    }
                }
            }

            return View(lista);
        }
    }
}
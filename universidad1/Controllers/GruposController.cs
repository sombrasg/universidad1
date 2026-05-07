using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class GruposController : Controller
    {
        private readonly string _cadenaConexion;

        public GruposController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        public IActionResult Index()
        {
            List<Grupo> lista = new List<Grupo>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                // MEGA JOIN: Unimos grupos con materias, profesores, aulas y periodos
                string query = @"
                    SELECT 
                        g.id, 
                        g.clave_grupo, 
                        m.nombre_materia, 
                        CONCAT(p.nombre, ' ', p.apellido_paterno) AS nombre_profesor, 
                        a.codigo_aula, 
                        pa.clave_periodo
                    FROM grupos g
                    INNER JOIN materias m ON g.materia_id = m.id
                    INNER JOIN profesores p ON g.profesor_id = p.id
                    INNER JOIN aulas a ON g.aula_id = a.id
                    INNER JOIN periodos_academicos pa ON g.periodo_id = pa.id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Grupo
                            {
                                Id = reader.GetInt32("id"),
                                ClaveGrupo = reader.GetString("clave_grupo"),
                                NombreMateria = reader.GetString("nombre_materia"),
                                NombreProfesor = reader.GetString("nombre_profesor"),
                                Aula = reader.GetString("codigo_aula"),
                                Periodo = reader.GetString("clave_periodo")
                            });
                        }
                    }
                }
            }

            return View(lista);
        }
    }
}
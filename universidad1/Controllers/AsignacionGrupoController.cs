using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class AsignacionGrupoController : Controller
    {
        private readonly string _cadenaConexion;

        public AsignacionGrupoController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        public IActionResult Index()
        {
            List<AsignacionGrupo> lista = new List<AsignacionGrupo>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                // TRIPLE JOIN: Asignación -> Alumnos -> Grupos -> Materias
                string query = @"
                    SELECT 
                        asig.id, 
                        CONCAT(alu.nombre, ' ', alu.apellido_paterno) AS nombre_alumno,
                        gru.clave_grupo,
                        mat.nombre_materia
                    FROM asignacion_alumnos_grupo asig
                    INNER JOIN alumnos alu ON asig.alumno_id = alu.id
                    INNER JOIN grupos gru ON asig.grupo_id = gru.id
                    INNER JOIN materias mat ON gru.materia_id = mat.id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new AsignacionGrupo
                            {
                                Id = reader.GetInt32("id"),
                                NombreAlumno = reader.GetString("nombre_alumno"),
                                ClaveGrupo = reader.GetString("clave_grupo"),
                                NombreMateria = reader.GetString("nombre_materia")
                            });
                        }
                    }
                }
            }
            return View(lista);
        }
    }
}
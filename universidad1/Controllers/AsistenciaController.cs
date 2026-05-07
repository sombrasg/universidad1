using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class AsistenciaController : Controller
    {
        private readonly string _cadenaConexion;

        public AsistenciaController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        public IActionResult Index()
        {
            List<Asistencia> lista = new List<Asistencia>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = @"
                    SELECT 
                        asi.id, 
                        CONCAT(alu.nombre, ' ', alu.apellido_paterno) AS nombre_alumno,
                        gru.clave_grupo,
                        asi.fecha,
                        asi.estado,
                        asi.observaciones
                    FROM asistencia asi
                    INNER JOIN alumnos alu ON asi.alumno_id = alu.id
                    INNER JOIN grupos gru ON asi.grupo_id = gru.id
                    ORDER BY asi.fecha DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Asistencia
                            {
                                Id = reader.GetInt32("id"),
                                NombreAlumno = reader.GetString("nombre_alumno"),
                                ClaveGrupo = reader.GetString("clave_grupo"),
                                Fecha = reader.GetDateTime("fecha"),
                                Estado = reader.GetString("estado"),
                                Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? "-" : reader.GetString("observaciones")
                            });
                        }
                    }
                }
            }
            return View(lista);
        }
    }
}
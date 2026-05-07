using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class PeriodosController : Controller
    {
        private readonly string _cadenaConexion;

        public PeriodosController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        public IActionResult Index()
        {
            List<Periodo> lista = new List<Periodo>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();

                // CORRECCIÓN: Cambiamos 'estatus_activo' por 'activo'
                string query = "SELECT id, clave_periodo, fecha_inicio, fecha_fin, activo FROM periodos_academicos ORDER BY fecha_inicio DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Periodo
                            {
                                Id = reader.GetInt32("id"),
                                ClavePeriodo = reader.GetString("clave_periodo"),
                                FechaInicio = reader.GetDateTime("fecha_inicio"),
                                FechaFin = reader.GetDateTime("fecha_fin"),
                                // CORRECCIÓN: Leemos la columna 'activo'
                                EstatusActivo = reader.GetBoolean("activo")
                            });
                        }
                    }
                }
            }

            return View(lista);
        }
    }
}
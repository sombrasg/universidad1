using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            List<Beca> lista = new List<Beca>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                // Query con doble JOIN para traer nombres en lugar de puros IDs
                string query = @"
                    SELECT 
                        b.id, 
                        CONCAT(a.nombre, ' ', a.apellido_paterno) AS nombre_alumno, 
                        b.nombre_beca, 
                        b.porcentaje_descuento, 
                        p.clave_periodo, 
                        b.estatus_activa
                    FROM becas_academicas b
                    INNER JOIN alumnos a ON b.alumno_id = a.id
                    INNER JOIN periodos_academicos p ON b.periodo_id = p.id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Beca
                            {
                                Id = reader.GetInt32("id"),
                                NombreAlumno = reader.GetString("nombre_alumno"),
                                NombreBeca = reader.GetString("nombre_beca"),
                                PorcentajeDescuento = reader.GetDecimal("porcentaje_descuento"),
                                ClavePeriodo = reader.GetString("clave_periodo"),
                                EstatusActiva = reader.GetBoolean("estatus_activa")
                            });
                        }
                    }
                }
            }
            return View(lista);
        }
    }
}
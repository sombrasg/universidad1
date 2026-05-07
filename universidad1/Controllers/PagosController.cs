using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            List<Pago> lista = new List<Pago>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                // DOBLE JOIN: Conectamos la tabla de pagos con alumnos y periodos
                string query = @"
                    SELECT 
                        p.id, 
                        CONCAT(a.nombre, ' ', a.apellido_paterno) AS nombre_alumno,
                        pa.clave_periodo,
                        p.concepto, 
                        p.monto, 
                        p.fecha_pago, 
                        p.metodo_pago, 
                        p.referencia_bancaria
                    FROM pagos p
                    INNER JOIN alumnos a ON p.alumno_id = a.id
                    INNER JOIN periodos_academicos pa ON p.periodo_id = pa.id
                    ORDER BY p.fecha_pago DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Pago
                            {
                                Id = reader.GetInt32("id"),
                                NombreAlumno = reader.GetString("nombre_alumno"),
                                ClavePeriodo = reader.GetString("clave_periodo"),
                                Concepto = reader.GetString("concepto"),
                                Monto = reader.GetDecimal("monto"),
                                FechaPago = reader.GetDateTime("fecha_pago"),
                                MetodoPago = reader.IsDBNull(reader.GetOrdinal("metodo_pago")) ? null : reader.GetString("metodo_pago"),
                                ReferenciaBancaria = reader.IsDBNull(reader.GetOrdinal("referencia_bancaria")) ? null : reader.GetString("referencia_bancaria")
                            });
                        }
                    }
                }
            }

            return View(lista);
        }
    }
}
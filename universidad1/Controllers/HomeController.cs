using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _cadenaConexion;

        // El constructor recibe la cadena de conexión configurada en tu Program.cs
        public HomeController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        public IActionResult Index()
        {
            // Instanciamos el modelo que llevará los datos a la vista
            DashboardViewModel data = new DashboardViewModel();

            using (MySqlConnection con = new MySqlConnection(_cadenaConexion))
            {
                try
                {
                    con.Open();

                    // 1. OBTENER TOTAL DE ALUMNOS
                    string qAlumnos = "SELECT COUNT(*) FROM alumnos";
                    using (MySqlCommand cmd = new MySqlCommand(qAlumnos, con))
                        data.TotalAlumnos = Convert.ToInt32(cmd.ExecuteScalar());

                    // 2. OBTENER TOTAL EN CAJA (Suma de la tabla pagos)
                    string qPagos = "SELECT IFNULL(SUM(monto), 0) FROM pagos";
                    using (MySqlCommand cmd = new MySqlCommand(qPagos, con))
                        data.CajaTotal = Convert.ToDecimal(cmd.ExecuteScalar());

                    // 3. OBTENER TOTAL DE CARRERAS
                    string qCarreras = "SELECT COUNT(*) FROM carreras";
                    using (MySqlCommand cmd = new MySqlCommand(qCarreras, con))
                        data.TotalCarreras = Convert.ToInt32(cmd.ExecuteScalar());

                    // 4. OBTENER GRUPOS ACTIVOS
                    string qGrupos = "SELECT COUNT(*) FROM grupos";
                    using (MySqlCommand cmd = new MySqlCommand(qGrupos, con))
                        data.GruposActivos = Convert.ToInt32(cmd.ExecuteScalar());

                    // 5. OBTENER LAS ÚLTIMAS 5 INSCRIPCIONES (Con Join para nombres)
                    string qRecientes = @"SELECT a.nombre, c.nombre_carrera, i.fecha_inscripcion 
                                          FROM inscripciones i
                                          JOIN alumnos a ON i.alumno_id = a.id
                                          JOIN carreras c ON i.carrera_id = c.id
                                          ORDER BY i.fecha_inscripcion DESC LIMIT 5";

                    using (MySqlCommand cmd = new MySqlCommand(qRecientes, con))
                    using (MySqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            data.Recientes.Add(new InscripcionDetalle
                            {
                                Alumno = r["nombre"].ToString(),
                                Carrera = r["nombre_carrera"].ToString(),
                                Fecha = Convert.ToDateTime(r["fecha_inscripcion"]).ToString("dd/MM/yyyy")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error en Dashboard: " + ex.Message);
                }
            }

            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
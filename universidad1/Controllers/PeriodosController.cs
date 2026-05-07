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

        // --- 1. LECTURA (INDEX) ---
        public IActionResult Index()
        {
            List<Periodo> lista = new List<Periodo>();
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT id, clave_periodo, fecha_inicio, fecha_fin, activo FROM periodos_academicos";

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
                                Activo = reader.GetBoolean("activo")
                            });
                        }
                    }
                }
            }
            return View(lista);
        }

        // --- 2. CREAR (CREATE) ---
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Periodo periodo)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = @"INSERT INTO periodos_academicos (clave_periodo, fecha_inicio, fecha_fin, activo) 
                                 VALUES (@clave, @inicio, @fin, @activo)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@clave", periodo.ClavePeriodo);
                    cmd.Parameters.AddWithValue("@inicio", periodo.FechaInicio.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@fin", periodo.FechaFin.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@activo", periodo.Activo);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // --- 3. EDITAR (EDIT) ---
        public IActionResult Edit(int id)
        {
            Periodo periodo = new Periodo();
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT * FROM periodos_academicos WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            periodo.Id = reader.GetInt32("id");
                            periodo.ClavePeriodo = reader.GetString("clave_periodo");
                            periodo.FechaInicio = reader.GetDateTime("fecha_inicio");
                            periodo.FechaFin = reader.GetDateTime("fecha_fin");
                            periodo.Activo = reader.GetBoolean("activo");
                        }
                    }
                }
            }
            return View(periodo);
        }

        [HttpPost]
        public IActionResult Edit(Periodo periodo)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = @"UPDATE periodos_academicos 
                                 SET clave_periodo=@clave, fecha_inicio=@inicio, fecha_fin=@fin, activo=@activo 
                                 WHERE id=@id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", periodo.Id);
                    cmd.Parameters.AddWithValue("@clave", periodo.ClavePeriodo);
                    cmd.Parameters.AddWithValue("@inicio", periodo.FechaInicio.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@fin", periodo.FechaFin.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@activo", periodo.Activo);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
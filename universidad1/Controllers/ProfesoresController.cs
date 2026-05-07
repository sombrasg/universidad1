using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class ProfesoresController : Controller
    {
        private readonly string _cadenaConexion;

        public ProfesoresController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        public IActionResult Index()
        {
            List<Profesor> lista = new List<Profesor>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                // INNER JOIN para mostrar el nombre del departamento en lugar del ID
                string query = @"
                    SELECT 
                        p.id, p.numero_empleado, p.nombre, p.apellido_paterno, p.apellido_materno, 
                        p.correo, p.telefono, p.especialidad, d.nombre_departamento
                    FROM profesores p
                    LEFT JOIN departamentos d ON p.departamento_id = d.id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Profesor
                            {
                                Id = reader.GetInt32("id"),
                                NumeroEmpleado = reader.GetString("numero_empleado"),
                                Nombre = reader.GetString("nombre"),
                                ApellidoPaterno = reader.GetString("apellido_paterno"),
                                ApellidoMaterno = reader.IsDBNull(reader.GetOrdinal("apellido_materno")) ? null : reader.GetString("apellido_materno"),
                                Correo = reader.GetString("correo"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
                                Especialidad = reader.IsDBNull(reader.GetOrdinal("especialidad")) ? null : reader.GetString("especialidad"),
                                NombreDepartamento = reader.IsDBNull(reader.GetOrdinal("nombre_departamento")) ? "Sin Asignar" : reader.GetString("nombre_departamento")
                            });
                        }
                    }
                }
            }

            return View(lista);
        }
    }
}
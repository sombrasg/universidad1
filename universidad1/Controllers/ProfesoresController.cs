using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // Método auxiliar para no repetir código: Obtiene la lista de departamentos para los menús desplegables
        private List<SelectListItem> ObtenerDepartamentos()
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT id, nombre_departamento FROM departamentos";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new SelectListItem
                            {
                                Value = reader.GetInt32("id").ToString(),
                                Text = reader.GetString("nombre_departamento")
                            });
                        }
                    }
                }
            }
            return lista;
        }

        // --- 1. LECTURA (INDEX) ---
        public IActionResult Index()
        {
            List<Profesor> lista = new List<Profesor>();
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
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
                                ApellidoMaterno = reader.IsDBNull(reader.GetOrdinal("apellido_materno")) ? "" : reader.GetString("apellido_materno"),
                                Correo = reader.IsDBNull(reader.GetOrdinal("correo")) ? "" : reader.GetString("correo"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? "" : reader.GetString("telefono"),
                                Especialidad = reader.IsDBNull(reader.GetOrdinal("especialidad")) ? "" : reader.GetString("especialidad"),
                                NombreDepartamento = reader.IsDBNull(reader.GetOrdinal("nombre_departamento")) ? "Sin Asignar" : reader.GetString("nombre_departamento")
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
            ViewBag.Departamentos = ObtenerDepartamentos();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Profesor profesor)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = @"INSERT INTO profesores 
                                (numero_empleado, nombre, apellido_paterno, apellido_materno, correo, telefono, especialidad, departamento_id) 
                                VALUES (@num, @nom, @pat, @mat, @correo, @tel, @esp, @depId)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@num", profesor.NumeroEmpleado);
                    cmd.Parameters.AddWithValue("@nom", profesor.Nombre);
                    cmd.Parameters.AddWithValue("@pat", profesor.ApellidoPaterno);
                    cmd.Parameters.AddWithValue("@mat", string.IsNullOrEmpty(profesor.ApellidoMaterno) ? (object)DBNull.Value : profesor.ApellidoMaterno);
                    cmd.Parameters.AddWithValue("@correo", string.IsNullOrEmpty(profesor.Correo) ? (object)DBNull.Value : profesor.Correo);
                    cmd.Parameters.AddWithValue("@tel", string.IsNullOrEmpty(profesor.Telefono) ? (object)DBNull.Value : profesor.Telefono);
                    cmd.Parameters.AddWithValue("@esp", string.IsNullOrEmpty(profesor.Especialidad) ? (object)DBNull.Value : profesor.Especialidad);
                    cmd.Parameters.AddWithValue("@depId", profesor.DepartamentoId);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // --- 3. EDITAR (EDIT) ---
        public IActionResult Edit(int id)
        {
            Profesor profesor = new Profesor();
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT * FROM profesores WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            profesor.Id = reader.GetInt32("id");
                            profesor.NumeroEmpleado = reader.GetString("numero_empleado");
                            profesor.Nombre = reader.GetString("nombre");
                            profesor.ApellidoPaterno = reader.GetString("apellido_paterno");
                            profesor.ApellidoMaterno = reader.IsDBNull(reader.GetOrdinal("apellido_materno")) ? null : reader.GetString("apellido_materno");
                            profesor.Correo = reader.IsDBNull(reader.GetOrdinal("correo")) ? null : reader.GetString("correo");
                            profesor.Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono");
                            profesor.Especialidad = reader.IsDBNull(reader.GetOrdinal("especialidad")) ? null : reader.GetString("especialidad");
                            profesor.DepartamentoId = reader.GetInt32("departamento_id");
                        }
                    }
                }
            }

            // Pasamos los departamentos a la vista para que el menú desplegable funcione al editar
            ViewBag.Departamentos = ObtenerDepartamentos();
            return View(profesor);
        }

        [HttpPost]
        public IActionResult Edit(Profesor profesor)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = @"UPDATE profesores SET 
                                numero_empleado=@num, nombre=@nom, apellido_paterno=@pat, apellido_materno=@mat, 
                                correo=@correo, telefono=@tel, especialidad=@esp, departamento_id=@depId 
                                WHERE id=@id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", profesor.Id);
                    cmd.Parameters.AddWithValue("@num", profesor.NumeroEmpleado);
                    cmd.Parameters.AddWithValue("@nom", profesor.Nombre);
                    cmd.Parameters.AddWithValue("@pat", profesor.ApellidoPaterno);
                    cmd.Parameters.AddWithValue("@mat", string.IsNullOrEmpty(profesor.ApellidoMaterno) ? (object)DBNull.Value : profesor.ApellidoMaterno);
                    cmd.Parameters.AddWithValue("@correo", string.IsNullOrEmpty(profesor.Correo) ? (object)DBNull.Value : profesor.Correo);
                    cmd.Parameters.AddWithValue("@tel", string.IsNullOrEmpty(profesor.Telefono) ? (object)DBNull.Value : profesor.Telefono);
                    cmd.Parameters.AddWithValue("@esp", string.IsNullOrEmpty(profesor.Especialidad) ? (object)DBNull.Value : profesor.Especialidad);
                    cmd.Parameters.AddWithValue("@depId", profesor.DepartamentoId);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
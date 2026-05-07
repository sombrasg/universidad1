using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class AlumnosController : Controller
    {
        private readonly string _cadenaConexion;

        public AlumnosController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        // GET: Alumnos (Muestra la lista)
        public IActionResult Index()
        {
            List<Alumno> listaAlumnos = new List<Alumno>();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT id, matricula, CONCAT(nombre, ' ', apellido_paterno, ' ', IFNULL(apellido_materno, '')) AS NombreCompleto, correo FROM alumnos";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaAlumnos.Add(new Alumno
                            {
                                Id = reader.GetInt32("id"),
                                Matricula = reader.GetString("matricula"),
                                NombreCompleto = reader.GetString("NombreCompleto"),
                                Correo = reader.IsDBNull(reader.GetOrdinal("correo")) ? "" : reader.GetString("correo")
                            });
                        }
                    }
                }
            }

            return View(listaAlumnos);
        }

        // 1. GET: Alumnos/Create (Formulario en blanco)
        public IActionResult Create()
        {
            return View();
        }

        // 2. POST: Alumnos/Create (Recibe TODOS los datos y hace el INSERT)
        [HttpPost]
        public IActionResult Create(Alumno alumno)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();

                // Query actualizado con los 3 nuevos campos
                string query = @"INSERT INTO alumnos 
                                (matricula, nombre, apellido_paterno, apellido_materno, fecha_nacimiento, correo, telefono, direccion) 
                                VALUES 
                                (@matricula, @nombre, @paterno, @materno, @fecha_nacimiento, @correo, @telefono, @direccion)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@matricula", alumno.Matricula);
                    cmd.Parameters.AddWithValue("@nombre", alumno.Nombre);
                    cmd.Parameters.AddWithValue("@paterno", alumno.ApellidoPaterno);

                    cmd.Parameters.AddWithValue("@materno", string.IsNullOrEmpty(alumno.ApellidoMaterno) ? (object)DBNull.Value : alumno.ApellidoMaterno);

                    // Manejo de la fecha
                    cmd.Parameters.AddWithValue("@fecha_nacimiento", alumno.FechaNacimiento.HasValue ? alumno.FechaNacimiento.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@correo", string.IsNullOrEmpty(alumno.Correo) ? (object)DBNull.Value : alumno.Correo);
                    cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(alumno.Telefono) ? (object)DBNull.Value : alumno.Telefono);
                    cmd.Parameters.AddWithValue("@direccion", string.IsNullOrEmpty(alumno.Direccion) ? (object)DBNull.Value : alumno.Direccion);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        // 1. GET: Alumnos/Edit/5 (Busca al alumno y carga TODOS sus datos)
        public IActionResult Edit(int id)
        {
            Alumno alumnoEncontrado = new Alumno();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                // Query actualizado para traer los nuevos campos
                string query = "SELECT id, matricula, nombre, apellido_paterno, apellido_materno, fecha_nacimiento, correo, telefono, direccion FROM alumnos WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            alumnoEncontrado.Id = reader.GetInt32("id");
                            alumnoEncontrado.Matricula = reader.GetString("matricula");
                            alumnoEncontrado.Nombre = reader.GetString("nombre");
                            alumnoEncontrado.ApellidoPaterno = reader.GetString("apellido_paterno");

                            alumnoEncontrado.ApellidoMaterno = reader.IsDBNull(reader.GetOrdinal("apellido_materno")) ? null : reader.GetString("apellido_materno");
                            alumnoEncontrado.Correo = reader.IsDBNull(reader.GetOrdinal("correo")) ? null : reader.GetString("correo");

                            // Lectura de los nuevos campos
                            alumnoEncontrado.FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("fecha_nacimiento")) ? (DateTime?)null : reader.GetDateTime("fecha_nacimiento");
                            alumnoEncontrado.Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono");
                            alumnoEncontrado.Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString("direccion");
                        }
                    }
                }
            }

            return View(alumnoEncontrado);
        }

        // 2. POST: Alumnos/Edit/5 (Actualiza TODOS los campos en MySQL)
        [HttpPost]
        public IActionResult Edit(Alumno alumno)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                // Query actualizado
                string query = @"UPDATE alumnos 
                                 SET matricula=@matricula, nombre=@nombre, apellido_paterno=@paterno, apellido_materno=@materno, 
                                     fecha_nacimiento=@fecha_nacimiento, correo=@correo, telefono=@telefono, direccion=@direccion 
                                 WHERE id=@id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", alumno.Id);
                    cmd.Parameters.AddWithValue("@matricula", alumno.Matricula);
                    cmd.Parameters.AddWithValue("@nombre", alumno.Nombre);
                    cmd.Parameters.AddWithValue("@paterno", alumno.ApellidoPaterno);

                    cmd.Parameters.AddWithValue("@materno", string.IsNullOrEmpty(alumno.ApellidoMaterno) ? (object)DBNull.Value : alumno.ApellidoMaterno);
                    cmd.Parameters.AddWithValue("@fecha_nacimiento", alumno.FechaNacimiento.HasValue ? alumno.FechaNacimiento.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@correo", string.IsNullOrEmpty(alumno.Correo) ? (object)DBNull.Value : alumno.Correo);
                    cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(alumno.Telefono) ? (object)DBNull.Value : alumno.Telefono);
                    cmd.Parameters.AddWithValue("@direccion", string.IsNullOrEmpty(alumno.Direccion) ? (object)DBNull.Value : alumno.Direccion);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        // 1. GET: Alumnos/Delete/5
        public IActionResult Delete(int id)
        {
            Alumno alumnoEncontrado = new Alumno();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT id, matricula, nombre, apellido_paterno, apellido_materno, correo FROM alumnos WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            alumnoEncontrado.Id = reader.GetInt32("id");
                            alumnoEncontrado.Matricula = reader.GetString("matricula");
                            alumnoEncontrado.Nombre = reader.GetString("nombre");
                            alumnoEncontrado.ApellidoPaterno = reader.GetString("apellido_paterno");
                            alumnoEncontrado.ApellidoMaterno = reader.IsDBNull(reader.GetOrdinal("apellido_materno")) ? null : reader.GetString("apellido_materno");
                            alumnoEncontrado.Correo = reader.IsDBNull(reader.GetOrdinal("correo")) ? null : reader.GetString("correo");
                        }
                    }
                }
            }

            return View(alumnoEncontrado);
        }

        // 2. POST: Alumnos/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "DELETE FROM alumnos WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }
    }
}
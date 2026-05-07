using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using universidad1.Models;

namespace universidad1.Controllers
{
    public class GruposController : Controller
    {
        private readonly string _cadenaConexion;

        public GruposController(string cadenaConexion)
        {
            _cadenaConexion = cadenaConexion;
        }

        // Función maestra para cargar los 4 menús desplegables
        private void CargarListasDesplegables()
        {
            List<SelectListItem> materias = new();
            List<SelectListItem> profesores = new();
            List<SelectListItem> aulas = new();
            List<SelectListItem> periodos = new();

            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();

                // 1. Materias
                using (MySqlCommand cmd = new MySqlCommand("SELECT id, nombre_materia FROM materias", conexion))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                    while (reader.Read()) materias.Add(new SelectListItem { Value = reader.GetInt32("id").ToString(), Text = reader.GetString("nombre_materia") });

                // 2. Profesores
                using (MySqlCommand cmd = new MySqlCommand("SELECT id, CONCAT(nombre, ' ', apellido_paterno) AS nombre_completo FROM profesores", conexion))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                    while (reader.Read()) profesores.Add(new SelectListItem { Value = reader.GetInt32("id").ToString(), Text = reader.GetString("nombre_completo") });

                // 3. Aulas
                using (MySqlCommand cmd = new MySqlCommand("SELECT id, codigo_aula FROM Aulas", conexion))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                    while (reader.Read()) aulas.Add(new SelectListItem { Value = reader.GetInt32("id").ToString(), Text = reader.GetString("codigo_aula") });

                // 4. Periodos
                using (MySqlCommand cmd = new MySqlCommand("SELECT id, clave_periodo FROM periodos_academicos", conexion))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                    while (reader.Read()) periodos.Add(new SelectListItem { Value = reader.GetInt32("id").ToString(), Text = reader.GetString("clave_periodo") });
            }

            ViewBag.Materias = materias;
            ViewBag.Profesores = profesores;
            ViewBag.Aulas = aulas;
            ViewBag.Periodos = periodos;
        }

        // --- 1. LECTURA (INDEX con SUPER JOIN de 5 TABLAS) ---
        public IActionResult Index()
        {
            List<Grupo> lista = new List<Grupo>();
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = @"
                    SELECT 
                        g.id, g.clave_grupo, 
                        m.nombre_materia, 
                        CONCAT(p.nombre, ' ', p.apellido_paterno) AS nombre_profesor, 
                        a.codigo_aula, 
                        pa.clave_periodo 
                    FROM grupos g
                    LEFT JOIN materias m ON g.materia_id = m.id
                    LEFT JOIN profesores p ON g.profesor_id = p.id
                    LEFT JOIN Aulas a ON g.aula_id = a.id
                    LEFT JOIN periodos_academicos pa ON g.periodo_id = pa.id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Grupo
                            {
                                Id = reader.GetInt32("id"),
                                ClaveGrupo = reader.GetString("clave_grupo"),
                                NombreMateria = reader.IsDBNull(reader.GetOrdinal("nombre_materia")) ? "N/A" : reader.GetString("nombre_materia"),
                                NombreProfesor = reader.IsDBNull(reader.GetOrdinal("nombre_profesor")) ? "N/A" : reader.GetString("nombre_profesor"),
                                CodigoAula = reader.IsDBNull(reader.GetOrdinal("codigo_aula")) ? "N/A" : reader.GetString("codigo_aula"),
                                ClavePeriodo = reader.IsDBNull(reader.GetOrdinal("clave_periodo")) ? "N/A" : reader.GetString("clave_periodo")
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
            CargarListasDesplegables();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Grupo grupo)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = @"INSERT INTO grupos (clave_grupo, materia_id, profesor_id, aula_id, periodo_id) 
                                 VALUES (@clave, @matId, @profId, @aulaId, @perId)";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@clave", grupo.ClaveGrupo);
                    cmd.Parameters.AddWithValue("@matId", grupo.MateriaId);
                    cmd.Parameters.AddWithValue("@profId", grupo.ProfesorId);
                    cmd.Parameters.AddWithValue("@aulaId", grupo.AulaId);
                    cmd.Parameters.AddWithValue("@perId", grupo.PeriodoId);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // --- 3. EDITAR (EDIT) ---
        public IActionResult Edit(int id)
        {
            Grupo grupo = new Grupo();
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT * FROM grupos WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            grupo.Id = reader.GetInt32("id");
                            grupo.ClaveGrupo = reader.GetString("clave_grupo");
                            grupo.MateriaId = reader.GetInt32("materia_id");
                            grupo.ProfesorId = reader.GetInt32("profesor_id");
                            grupo.AulaId = reader.GetInt32("aula_id");
                            grupo.PeriodoId = reader.GetInt32("periodo_id");
                        }
                    }
                }
            }
            CargarListasDesplegables();
            return View(grupo);
        }

        [HttpPost]
        public IActionResult Edit(Grupo grupo)
        {
            using (MySqlConnection conexion = new MySqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = @"UPDATE grupos SET 
                                 clave_grupo=@clave, materia_id=@matId, profesor_id=@profId, aula_id=@aulaId, periodo_id=@perId 
                                 WHERE id=@id";

                using (MySqlCommand cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", grupo.Id);
                    cmd.Parameters.AddWithValue("@clave", grupo.ClaveGrupo);
                    cmd.Parameters.AddWithValue("@matId", grupo.MateriaId);
                    cmd.Parameters.AddWithValue("@profId", grupo.ProfesorId);
                    cmd.Parameters.AddWithValue("@aulaId", grupo.AulaId);
                    cmd.Parameters.AddWithValue("@perId", grupo.PeriodoId);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace universidad1.Models
{
    public class Asistencia
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public int GrupoId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public string? Estado { get; set; } // Presente, Ausente, Retardo
        public string? Observaciones { get; set; }

        // Propiedades para JOINs
        public string? NombreAlumno { get; set; }
        public string? ClaveGrupo { get; set; }
        public string? NombreMateria { get; set; }
    }
}
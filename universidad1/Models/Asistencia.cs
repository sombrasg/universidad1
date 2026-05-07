namespace universidad1.Models
{
    public class Asistencia
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? Estado { get; set; } // Presente, Retardo, Falta
        public string? Observaciones { get; set; }

        // Propiedades para los JOINs
        public string? NombreAlumno { get; set; }
        public string? ClaveGrupo { get; set; }
    }
}
namespace universidad1.Models
{
    public class Inscripcion
    {
        public int Id { get; set; }

        // Variables para guardar los nombres reales gracias a los JOINs
        public string? NombreAlumno { get; set; }
        public string? NombreCarrera { get; set; }
        public string? ClavePeriodo { get; set; }
    }
}
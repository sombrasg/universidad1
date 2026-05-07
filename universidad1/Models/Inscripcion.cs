namespace universidad1.Models
{
    public class Inscripcion
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public int CarreraId { get; set; }
        public int PeriodoId { get; set; }

        public string? Matricula { get; set; }
        public string? NombreAlumno { get; set; }
        public string? NombreCarrera { get; set; }
        public string? ClavePeriodo { get; set; }
    }
}
namespace universidad1.Models
{
    public class AsignacionGrupo
    {
        public int Id { get; set; }
        // Propiedades que traeremos mediante JOINs
        public string? NombreAlumno { get; set; }
        public string? ClaveGrupo { get; set; }
        public string? NombreMateria { get; set; }
    }
}
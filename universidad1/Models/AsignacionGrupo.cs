namespace universidad1.Models
{
    public class AsignacionAlumnoGrupo
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public int GrupoId { get; set; }

        public string? NombreAlumno { get; set; }
        public string? Matricula { get; set; }
        public string? ClaveGrupo { get; set; }
        public string? NombreMateria { get; set; }
    }
}
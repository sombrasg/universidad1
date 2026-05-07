namespace universidad1.Models
{
    public class Calificacion
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public int GrupoId { get; set; }
        public decimal Parcial1 { get; set; }
        public decimal Parcial2 { get; set; }
        public decimal PromedioFinal { get; set; }
        public string? EstatusAprobacion { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Para los JOINs
        public string? NombreAlumno { get; set; }
        public string? ClaveGrupo { get; set; }
        public string? NombreMateria { get; set; }
    }
}
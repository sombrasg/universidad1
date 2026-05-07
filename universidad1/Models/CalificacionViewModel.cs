namespace universidad1.Models
{
    public class CalificacionViewModel
    {
        public int Id { get; set; }
        public string? NombreAlumno { get; set; }  // <-- Solo agregamos el ? aquí
        public string? NombreMateria { get; set; } // <-- Y el ? aquí
        public decimal? Parcial1 { get; set; }
        public decimal? Parcial2 { get; set; }
        public decimal? PromedioFinal { get; set; }
        public string? EstatusAprobacion { get; set; }
    }
}
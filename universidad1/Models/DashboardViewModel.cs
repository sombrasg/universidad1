namespace universidad1.Models
{
    public class DashboardViewModel
    {
        public int TotalAlumnos { get; set; }
        public decimal CajaTotal { get; set; }
        public int TotalCarreras { get; set; }
        public int GruposActivos { get; set; }

        public List<InscripcionDetalle> Recientes { get; set; } = new();
    }

    public class InscripcionDetalle
    {
        public string? Alumno { get; set; }
        public string? Carrera { get; set; }
        public string? Fecha { get; set; }
    }
}
namespace universidad1.Models
{
    public class Periodo
    {
        public int Id { get; set; }
        public string? ClavePeriodo { get; set; } // Ej: "2026-1" o "2026-A"
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool EstatusActivo { get; set; }
    }
}
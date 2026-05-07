namespace universidad1.Models
{
    public class Periodo
    {
        public int Id { get; set; }
        public string? ClavePeriodo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
    }
}
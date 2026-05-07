namespace universidad1.Models
{
    public class Beca
    {
        public int Id { get; set; }
        public string? NombreBeca { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public bool EstatusActiva { get; set; }

        // Variables para los nombres de las otras tablas
        public string? NombreAlumno { get; set; }
        public string? ClavePeriodo { get; set; }
    }
}
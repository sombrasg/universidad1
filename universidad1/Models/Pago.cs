namespace universidad1.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public string? Concepto { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string? MetodoPago { get; set; }
        public string? ReferenciaBancaria { get; set; }

        // Campos adicionales para los INNER JOIN
        public string? NombreAlumno { get; set; }
        public string? ClavePeriodo { get; set; }
    }
}
namespace universidad1.Models
{
    public class BecaAcademica
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public string? NombreBeca { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public int PeriodoId { get; set; }
        public bool EstatusActiva { get; set; }

        public string? NombreAlumno { get; set; }
        public string? ClavePeriodo { get; set; }
        public decimal PromedioActual { get; set; } // Para mostrar cuánto trae el alumno
    }
}
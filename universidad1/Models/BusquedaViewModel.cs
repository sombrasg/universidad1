namespace universidad1.Models
{
    public class BusquedaViewModel
    {
        public string? TerminoBusqueda { get; set; }
        public List<AlumnoResultado> Resultados { get; set; } = new();
        public List<AlumnoResultado> HistorialReciente { get; set; } = new();
    }

    public class AlumnoResultado
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Carrera { get; set; }
        public string? Matricula { get; set; }
    }
}
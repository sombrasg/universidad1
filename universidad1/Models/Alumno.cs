namespace universidad1.Models
{
    public class Alumno
    {
        public int Id { get; set; }
        public string Matricula { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
    }
}
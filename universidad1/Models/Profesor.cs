namespace universidad1.Models
{
    public class Profesor
    {
        public int Id { get; set; }
        public string NumeroEmpleado { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Especialidad { get; set; }

        // Campo para el JOIN
        public string? NombreDepartamento { get; set; }
    }
}
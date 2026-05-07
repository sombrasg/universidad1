namespace universidad1.Models
{
    public class Profesor
    {
        public int Id { get; set; }
        public string? NumeroEmpleado { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Especialidad { get; set; }

       
        public int DepartamentoId { get; set; }
        public string? NombreDepartamento { get; set; } // Para mostrar el nombre en la tabla
    }
}
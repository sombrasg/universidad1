namespace universidad1.Models
{
    public class Horario
    {
        // Usamos el ID del grupo para poder abrir la ventana flotante correcta
        public int GrupoId { get; set; }
        public string? ClaveGrupo { get; set; }
        public string? NombreMateria { get; set; }
        public string? NombreProfesor { get; set; }

        // Aquí guardaremos todos los días juntos separados por una barra "|"
        public string? DiasYHoras { get; set; }
    }
}
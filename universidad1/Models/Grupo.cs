namespace universidad1.Models
{
    public class Grupo
    {
        public int Id { get; set; }
        public string? ClaveGrupo { get; set; }

        // Variables para los 4 INNER JOINS
        public string? NombreMateria { get; set; }
        public string? NombreProfesor { get; set; }
        public string? Aula { get; set; }
        public string? Periodo { get; set; }
    }
}
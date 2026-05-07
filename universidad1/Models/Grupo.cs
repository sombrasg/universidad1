namespace universidad1.Models
{
    public class Grupo
    {
        public int Id { get; set; }
        public string? ClaveGrupo { get; set; }
        public int MateriaId { get; set; }
        public int ProfesorId { get; set; }
        public int AulaId { get; set; }
        public int PeriodoId { get; set; }

        public string? NombreMateria { get; set; }
        public string? NombreProfesor { get; set; }
        public string? CodigoAula { get; set; }
        public string? ClavePeriodo { get; set; }
    }
}
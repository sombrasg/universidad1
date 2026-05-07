namespace universidad1.Models
{
    public class Horario
    {
        public int Id { get; set; }
        public int GrupoId { get; set; }
        public int AulaId { get; set; }
        public string? DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public string? ClaveGrupo { get; set; }
        public string? CodigoAula { get; set; }
    }
}
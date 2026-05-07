namespace universidad1.Models
{
    public class Carrera
    {
        public int Id { get; set; }
        public string? ClaveCarrera { get; set; }
        public string? NombreCarrera { get; set; }
        public int DuracionSemestres { get; set; }
        public bool EstatusActivo { get; set; }
    }
}
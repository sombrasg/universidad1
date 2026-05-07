namespace universidad1.Models
{
    public class Materia
    {
        public int Id { get; set; }
        public string? Clave { get; set; }
        public string? NombreMateria { get; set; }
        public int Creditos { get; set; }
        public int HorasTeoricas { get; set; }
        public int HorasPracticas { get; set; }
        public int SemestreSugerido { get; set; }
        public int CarreraId { get; set; } // <--- Asegúrate de tener esta
        public string? NombreCarrera { get; set; }
    }
}
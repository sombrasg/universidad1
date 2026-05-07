namespace universidad1.Models
{
    public class Aula
    {
        public int Id { get; set; }
        public string? CodigoAula { get; set; }
        public string? Edificio { get; set; } // ¡Agregamos el edificio que vi en tu foto!
        public int Capacidad { get; set; }
        public string? TipoAula { get; set; }
    }
}
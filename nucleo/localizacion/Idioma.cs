
namespace Primerjuego2D.nucleo.localizacion;

public sealed class Idioma
{
    public string Codigo { get; }
    public string Nombre { get; }

    private Idioma(string codigo, string nombre)
    {
        Codigo = codigo;
        Nombre = nombre;
    }

    public static readonly Idioma ES = new("es", "Español");

    public static readonly Idioma EN = new("en", "Inglés");
}
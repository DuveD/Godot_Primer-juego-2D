
namespace Primerjuego2D.nucleo.localizacion;

public sealed class Idioma
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string TagNombre { get; }

    private Idioma(string codigo, string nombre, string tagNombre)
    {
        Codigo = codigo;
        Nombre = nombre;
        TagNombre = tagNombre;
    }

    public static readonly Idioma ES = new("es", "Español", "Idioma.es.nombre");

    public static readonly Idioma EN = new("en", "Inglés", "Idioma.en.nombre");
}
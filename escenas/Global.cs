using Godot;
using Primerjuego2D.escenas.sistema;
using Primerjuego2D.nucleo.configuracion;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas;

public partial class Global : Node
{
    public static Global Instancia { get; private set; }

    public GestorColor _GestorColor { get; private set; }
    public static GestorColor GestorColor => Global.Instancia._GestorColor;

    public GestorAudio _GestorAudio { get; private set; }
    public static GestorAudio GestorAudio => Global.Instancia._GestorAudio;

    public Global()
    {
        Ajustes.CargarAjustes();

        // Informar idioma.
        Idioma idioma = Ajustes.Idioma;
        GestorIdioma.CambiarIdioma(idioma);
    }

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        InicializarValoresEstaticos();
    }

    private void InicializarValoresEstaticos()
    {
        Global.Instancia = this;

        _GestorColor = GetNode<GestorColor>("GestorColor");
        _GestorAudio = GetNode<GestorAudio>("GestorAudio");
    }
}
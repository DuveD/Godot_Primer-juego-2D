using Godot;
using Primerjuego2D.nucleo.configuracion;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class MenuPrincipal : Control
{
    public const long ID_OPCION_CASTELLANO = 0;
    public const long ID_OPCION_INGLES = 1;

    [Signal]
    public delegate void BotonEmpezarPartidaPulsadoEventHandler();

    ColorRect _Fondo;
    private ColorRect Fondo => _Fondo ??= GetNode<ColorRect>("Fondo");

    private MenuButton _MenuButtonLenguaje;
    private MenuButton MenuButtonLenguaje => _MenuButtonLenguaje ??= GetNode<MenuButton>("MenuButtonLenguaje");

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        InicializarMenuButtonLenguaje();
    }

    private void InicializarMenuButtonLenguaje()
    {
        PopupMenu popupMenu = this.MenuButtonLenguaje.GetPopup();
        popupMenu.IdPressed += MenuButtonLenguajeIdPressed;

        Idioma idioma = GestorIdioma.ObtenerIdiomaDeSistema();
        if (idioma.Codigo == Idioma.ES.Codigo)
            MenuButtonLenguajeIdPressed(ID_OPCION_CASTELLANO);
        else if (idioma.Codigo == Idioma.EN.Codigo)
            MenuButtonLenguajeIdPressed(ID_OPCION_INGLES);
        else
            MenuButtonLenguajeIdPressed(ID_OPCION_CASTELLANO);
    }

    private void MenuButtonLenguajeIdPressed(long id)
    {
        LoggerJuego.Trace("Opción de 'MenuButtonLenguaje' pulsado.");

        // Obtenemos el PopupMenu del MenuButton
        var popupMenu = this.MenuButtonLenguaje.GetPopup();

        // Checkeamos el ítem del id
        UtilidadesNodos.CheckItemPorId(popupMenu, id);

        Idioma idioma;
        switch (id)
        {
            default:
            case ID_OPCION_CASTELLANO:
                idioma = Idioma.ES;
                break;
            case ID_OPCION_INGLES:
                idioma = Idioma.EN;
                break;
        }

        GestorIdioma.CambiarIdioma(idioma);
        Ajustes.Idioma = idioma;
    }

    private void OnButtonEmpezarPartidaPressed()
    {
        LoggerJuego.Trace("Botón 'ButtonEmpezarPartida' pulsado.");
        EmitSignal(SignalName.BotonEmpezarPartidaPulsado);
    }

    private void OnButtonCargarPartidaPressed()
    {
        LoggerJuego.Trace("Botón 'ButtonCargarPartida' pulsado.");
    }

    private void OnButtonSalirPressed()
    {
        LoggerJuego.Trace("Botón 'ButtonSalir' pulsado.");
        this.GetTree().Quit();
    }
}

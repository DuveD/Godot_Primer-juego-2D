using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.nucleo.configuracion;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class MenuPrincipal : Control
{
    private bool _navegacionPorTeclado = true;

    public const long ID_OPCION_CASTELLANO = 0;
    public const long ID_OPCION_INGLES = 1;

    [Signal]
    public delegate void BotonEmpezarPartidaPulsadoEventHandler();

    private ColorRect _Fondo;
    private ColorRect Fondo => _Fondo ??= GetNode<ColorRect>("Fondo");

    private MenuButton _MenuButtonLenguaje;
    private MenuButton MenuButtonLenguaje => _MenuButtonLenguaje ??= GetNode<MenuButton>("MenuButtonLenguaje");

    private List<Button> _BotonesMenu;

    private List<Button> BotonesMenu => _BotonesMenu ??= UtilidadesNodos.ObtenerNodosDeTipo<Button>(this);

    private ButtonEmpezarPartida _ButtonEmpezarPartida;
    private ButtonEmpezarPartida ButtonEmpezarPartida => _ButtonEmpezarPartida ??= BotonesMenu.OfType<ButtonEmpezarPartida>().FirstOrDefault();

    private Button _ultimoBotonConFocus = null;

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        InicializarMenuButtonLenguaje();

        this.ButtonEmpezarPartida.GrabFocus();
        this.ButtonEmpezarPartida.FocusEntered += () => this.ButtonEmpezarPartida.OnFocusedEntered();
        _ultimoBotonConFocus = this.ButtonEmpezarPartida;

        foreach (var boton in BotonesMenu)
            boton.FocusEntered += () => _ultimoBotonConFocus = boton;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (!_navegacionPorTeclado &&
                UtilidadesControles.IsActionPressed(@event, ConstantesAcciones.UP, ConstantesAcciones.RIGHT, ConstantesAcciones.DOWN, ConstantesAcciones.LEFT))
            {
                LoggerJuego.Trace("Activamos la navegación por teclado.");
                ActivarNavegacionTeclado();
            }
            return;
        }

        if (@event is InputEventMouse && _navegacionPorTeclado)
        {
            LoggerJuego.Trace("Desactivamos la navegación por teclado.");
            DesactivarNavegacionTeclado();
        }
    }
    private void ActivarNavegacionTeclado()
    {
        _navegacionPorTeclado = true;
        foreach (var boton in BotonesMenu)
            boton.FocusMode = FocusModeEnum.All;
        _ultimoBotonConFocus?.GrabFocus();
    }

    private void DesactivarNavegacionTeclado()
    {
        _navegacionPorTeclado = false;
        foreach (var boton in BotonesMenu)
            boton.FocusMode = FocusModeEnum.None;
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

    private void OnButtonEmpezarPartidaPressedAnimationEnd()
    {
        LoggerJuego.Trace("Botón 'ButtonEmpezarPartida' pulsado.");

        EmitSignal(SignalName.BotonEmpezarPartidaPulsado);
    }

    private void OnButtonCargarPartidaPressed()
    {
        LoggerJuego.Trace("Botón 'ButtonCargarPartida' pulsado.");

        Global.GestorAudio.ReproducirSonido("digital_click.mp3");
    }

    private async void OnButtonSalirPressed()
    {
        LoggerJuego.Trace("Botón 'ButtonSalir' pulsado.");

        Global.GestorAudio.ReproducirSonido("digital_click.mp3");
        await Task.Delay(500);

        this.GetTree().Quit();
    }


}

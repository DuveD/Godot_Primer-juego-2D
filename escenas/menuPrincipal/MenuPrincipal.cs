using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.nucleo.configuracion;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.modelos;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;

namespace Primerjuego2D.escenas.menuPrincipal;

[AtributoNivelLog(NivelLog.Info)]
public partial class MenuPrincipal : Control
{
    private bool _menuDesactivado = false;

    private bool _navegacionPorTeclado = true;

    public const long ID_OPCION_CASTELLANO = 0;

    public const long ID_OPCION_INGLES = 1;

    [Signal]
    public delegate void BotonEmpezarPartidaPulsadoEventHandler();

    private ColorRect _Fondo;
    private ColorRect Fondo => _Fondo ??= GetNode<ColorRect>("Fondo");

    private CenterContainer _ContenedorBotonesPrincipal;
    private CenterContainer ContenedorBotonesPrincipal => _ContenedorBotonesPrincipal ??= GetNode<CenterContainer>("ContenedorBotonesPrincipal");

    private CenterContainer _ContenedorMenuAjustes;
    private CenterContainer ContenedorMenuAjustes => _ContenedorMenuAjustes ??= GetNode<CenterContainer>("ContenedorMenuAjustes");

    private List<Button> _BotonesMenu;
    private List<Button> BotonesMenu => _BotonesMenu ??= UtilidadesNodos.ObtenerNodosDeTipo<Button>(this.ContenedorBotonesPrincipal);

    private ButtonEmpezarPartida _ButtonEmpezarPartida;
    private ButtonEmpezarPartida ButtonEmpezarPartida => _ButtonEmpezarPartida ??= BotonesMenu.OfType<ButtonEmpezarPartida>().FirstOrDefault();

    private CanvasLayer _CrtLayer;
    private CanvasLayer CrtLayer => _CrtLayer ??= GetNode<CanvasLayer>("CRTShutdown");

    private AnimationPlayer _AnimPlayer;
    private AnimationPlayer AnimPlayer => _AnimPlayer ??= CrtLayer.GetNode<AnimationPlayer>("AnimationPlayer");

    private Button _ultimoBotonConFocus = null;

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        ConfigurarBotonesMenu();
    }

    private void ConfigurarBotonesMenu()
    {
        this.ButtonEmpezarPartida.GrabFocusSilencioso();
        _ultimoBotonConFocus = this.ButtonEmpezarPartida;

        foreach (var boton in BotonesMenu)
        {
            boton.FocusEntered += () => _ultimoBotonConFocus = boton;
            boton.Pressed += DesactivarMenu;
        }
    }

    private void ActivarMenu()
    {
        LoggerJuego.Trace("Activamos el menú.");

        _menuDesactivado = false;
        foreach (var boton in BotonesMenu)
        {
            boton.MouseFilter = MouseFilterEnum.Pass; // Aceptamos clicks
            boton.FocusMode = FocusModeEnum.All;      // Aceptamos teclado
        }
    }

    private void DesactivarMenu()
    {
        LoggerJuego.Trace("Desactivamos el menú.");

        _menuDesactivado = true;
        foreach (var boton in BotonesMenu)
        {
            boton.MouseFilter = MouseFilterEnum.Ignore; // Ignora clicks
            boton.FocusMode = FocusModeEnum.None;       // Ignora teclado
        }
    }

    public override void _Input(InputEvent @event)
    {
        CambioMetodoImput(@event);
    }

    private void CambioMetodoImput(InputEvent @event)
    {
        if (_menuDesactivado)
        {
            LoggerJuego.Trace("EL menú está desactivado.");
            // Ignora cualquier input
            return;
        }

        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (!_navegacionPorTeclado &&
                UtilidadesControles.IsActionPressed(@event, ConstantesAcciones.UP, ConstantesAcciones.RIGHT, ConstantesAcciones.DOWN, ConstantesAcciones.LEFT))
            {
                LoggerJuego.Trace("Activamos la navegación por teclado.");
                ActivarNavegacionTeclado();
            }
        }
        else if (@event is InputEventMouse)
        {
            if (_navegacionPorTeclado)
            {
                LoggerJuego.Trace("Desactivamos la navegación por teclado.");
                DesactivarNavegacionTeclado();
            }
        }
    }

    private void ActivarNavegacionTeclado()
    {
        _navegacionPorTeclado = true;
        foreach (var boton in BotonesMenu)
            boton.FocusMode = FocusModeEnum.All;

        GrabFocusUltimoBotonConFocus();
    }

    private void GrabFocusUltimoBotonConFocus()
    {
        if (this._ultimoBotonConFocus is BotonMenuPrincipal botonMenuPrincipal)
        {
            botonMenuPrincipal.GrabFocusSilencioso();
        }
        else
        {
            this._ultimoBotonConFocus.GrabFocus();
        }
    }

    private void DesactivarNavegacionTeclado()
    {
        _navegacionPorTeclado = false;
        foreach (var boton in BotonesMenu)
            boton.FocusMode = FocusModeEnum.None;
    }

    private void OnButtonEmpezarPartidaPressedAnimationEnd()
    {
        LoggerJuego.Trace("Botón 'ButtonEmpezarPartida' pulsado.");

        EmitSignal(SignalName.BotonEmpezarPartidaPulsado);
    }

    private void OnButtonAjustesPressed()
    {
        LoggerJuego.Trace("Botón 'ButtonAjustes' pulsado.");

        Global.GestorAudio.ReproducirSonido("digital_click.mp3");

        this.ContenedorBotonesPrincipal.Visible = false;
        this.ContenedorMenuAjustes.Visible = true;
    }

    private async void OnButtonSalirPressed()
    {
        LoggerJuego.Trace("Botón 'ButtonSalir' pulsado.");

        Global.GestorAudio.ReproducirSonido("digital_click.mp3");
        Global.GestorAudio.PausarMusica(0.5f);

        CrtLayer.Visible = true;
        AnimPlayer.Play("ApagarTV");

        await ToSignal(AnimPlayer, "animation_finished");
        await Task.Delay(300);

        this.GetTree().Quit();
    }

    public void OnButtonAjustesAtrasPressed()
    {
        LoggerJuego.Trace("Botón Ajustes 'Atrás' pulsado.");

        Global.GestorAudio.ReproducirSonido("digital_click.mp3");

        this.ContenedorMenuAjustes.Visible = false;
        this.ContenedorBotonesPrincipal.Visible = true;

        ActivarMenu();
    }
}

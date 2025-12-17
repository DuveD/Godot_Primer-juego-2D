using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class ContenedorMenuPrincipal : CenterContainer
{
    private bool _menuDesactivado = false;

    [Signal]
    public delegate void BotonEmpezarPartidaPulsadoEventHandler();

    private List<Button> _BotonesMenu;
    private List<Button> BotonesMenu => _BotonesMenu ??= UtilidadesNodos.ObtenerNodosDeTipo<Button>(this);

    private ButtonEmpezarPartida _ButtonEmpezarPartida;
    public ButtonEmpezarPartida ButtonEmpezarPartida => _ButtonEmpezarPartida ??= BotonesMenu.OfType<ButtonEmpezarPartida>().FirstOrDefault();

    private ButtonAjustes _ButtonAjustes;
    public ButtonAjustes ButtonAjustes => _ButtonAjustes ??= BotonesMenu.OfType<ButtonAjustes>().FirstOrDefault();

    private CanvasLayer _CrtLayer;
    private CanvasLayer CrtLayer => _CrtLayer ??= GetNode<CanvasLayer>("../CRTShutdown");

    private AnimationPlayer _AnimPlayer;
    private AnimationPlayer AnimPlayer => _AnimPlayer ??= CrtLayer.GetNode<AnimationPlayer>("AnimationPlayer");

    private MenuPrincipal _MenuPrincipal;
    private MenuPrincipal MenuPrincipal => _MenuPrincipal ??= this.GetParent<MenuPrincipal>();

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        ConfigurarFocusBotones();
    }

    private void ConfigurarFocusBotones()
    {
        LoggerJuego.Trace("Configuramos el focus de los botones del menú.");

        foreach (var boton in BotonesMenu)
        {
            boton.FocusEntered += () => this.MenuPrincipal.UltimoElementoConFocus = boton;
            boton.Pressed += DesactivarFocusBotones;
        }
    }

    public void ActivarFocusBotones()
    {
        LoggerJuego.Trace("Activamos el focus de los botones del menú.");

        _menuDesactivado = false;
        foreach (var boton in BotonesMenu)
        {
            boton.MouseFilter = MouseFilterEnum.Pass; // Aceptamos clicks
            boton.FocusMode = FocusModeEnum.All;      // Aceptamos teclado
        }
    }

    public void ActivarNavegacionTeclado()
    {
        LoggerJuego.Trace("Activamos la navegación por teclado.");

        foreach (var boton in BotonesMenu)
            boton.FocusMode = FocusModeEnum.All;
    }

    public void DesactivarNavegacionTeclado()
    {
        LoggerJuego.Trace("Desactivamos la navegación por teclado.");

        foreach (var boton in BotonesMenu)
            boton.FocusMode = FocusModeEnum.None;
    }

    private void DesactivarFocusBotones()
    {
        LoggerJuego.Trace("Desactivamos el focus de los botones del menú.");

        _menuDesactivado = true;
        foreach (var boton in BotonesMenu)
        {
            boton.MouseFilter = MouseFilterEnum.Ignore; // Ignora clicks
            boton.FocusMode = FocusModeEnum.None;       // Ignora teclado
        }
    }

    private void OnButtonEmpezarPartidaPressedAnimationEnd()
    {
        LoggerJuego.Trace("Botón 'ButtonEmpezarPartida' pulsado.");

        EmitSignal(SignalName.BotonEmpezarPartidaPulsado);
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
}

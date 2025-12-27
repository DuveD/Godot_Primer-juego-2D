using Godot;
using Primerjuego2D.escenas.entidades.enemigo;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.objetos.moneda;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class Batalla : Node
{
    [Signal]
    public delegate void GameOverFinalizadoEventHandler();

    ColorRect _Fondo;
    private ColorRect Fondo => _Fondo ??= GetNode<ColorRect>("Fondo");

    private Jugador _Jugador;
    private Jugador Jugador => _Jugador ??= GetNode<Jugador>("Jugador");

    private Marker2D _StartPosition;
    private Marker2D StartPosition => _StartPosition ??= GetNode<Marker2D>("StartPosition");

    private BatallaControlador _BatallaControlador;
    private BatallaControlador BatallaControlador => _BatallaControlador ??= GetNode<BatallaControlador>("BatallaControlador");

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        ProcessMode = ProcessModeEnum.Pausable;

        this.NuevoJuego();
    }

    public void NuevoJuego()
    {
        LoggerJuego.Info("Nuevo juego.");

        this.Jugador.Start(this.StartPosition.Position);

        this.BatallaControlador.IniciarBatalla();
    }

    public async void InicioGameOver()
    {
        Global.GestorAudio.PausarMusica(2f);

        this.BatallaControlador.FinalizarBatalla();

        await UtilidadesNodos.EsperarSegundos(this, 2.0);
    }

    public async void FinGameOver()
    {
        EmitSignal(SignalName.GameOverFinalizado);
    }
}

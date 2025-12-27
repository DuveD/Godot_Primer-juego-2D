using Godot;
using Primerjuego2D.escenas.objetos.moneda;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class BatallaControlador : Node
{
    [Signal]
    public delegate void IniciandoBatallaEventHandler();

    [Signal]
    public delegate void BatallaIniciadaEventHandler();

    [Signal]
    public delegate void BatallaFinalizadaEventHandler();

    [Signal]
    public delegate void PausarBatallaEventHandler();

    [Signal]
    public delegate void RenaudarBatallaEventHandler();

    [Signal]
    public delegate void PuntuacionActualizadaEventHandler(int Puntuacion);

    public int Puntuacion { get; private set; } = 0;

    public bool BatallaEnCurso { get; private set; } = false;

    public bool JuegoPausado { get; set; } = false;

    private BatallaHUD _BatallaHUD;
    private BatallaHUD BatallaHUD => _BatallaHUD ??= GetNode<BatallaHUD>("../BatallaHUD");

    private SpawnEnemigos _SpawnEnemigos;
    private SpawnEnemigos SpawnEnemigos => _SpawnEnemigos ??= GetNode<SpawnEnemigos>("../SpawnEnemigos");

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            OnPauseButtonPressed();
        }
    }

    private void OnPauseButtonPressed()
    {
        if (!this.BatallaEnCurso)
            return;

        this.JuegoPausado = !JuegoPausado;

        UtilidadesNodos.PausarNodo(this, this.JuegoPausado);

        if (this.JuegoPausado)
        {
            LoggerJuego.Trace("Juego pausado.");
            EmitSignal(SignalName.PausarBatalla);
        }
        else
        {
            LoggerJuego.Trace("Juego renaudado.");
            EmitSignal(SignalName.RenaudarBatalla);
        }
    }

    public async void IniciarBatalla()
    {
        if (BatallaEnCurso)
            return;

        LoggerJuego.Info("Iniciamos la batalla.");
        EmitSignal(SignalName.IniciandoBatalla);

        this.Puntuacion = 0;
        this.BatallaEnCurso = true;

        EmitSignal(SignalName.PuntuacionActualizada, Puntuacion);

        await UtilidadesNodos.EsperarSegundos(this, 2.0);
        await UtilidadesNodos.EsperarRenaudar(this);

        LoggerJuego.Info("Batalla iniciada.");
        EmitSignal(SignalName.BatallaIniciada);
    }

    public void FinalizarBatalla()
    {
        if (!BatallaEnCurso)
            return;

        BatallaEnCurso = false;

        LoggerJuego.Info("Batalla finalizada.");
        EmitSignal(SignalName.BatallaFinalizada);
    }

    public void SumarPuntuacion(Moneda moneda)
    {
        this.Puntuacion += moneda.Valor;
        EmitSignal(SignalName.PuntuacionActualizada, Puntuacion);
    }
}

using Godot;
using Primerjuego2D.nucleo.configuracion;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class BatallaControlador : Node
{
    [Signal]
    public delegate void PauseBattleEventHandler();

    [Signal]
    public delegate void BatallaIniciadaEventHandler();

    [Signal]
    public delegate void BatallaFinalizadaEventHandler();

    public bool BatallaEnCurso { get; private set; } = false;

    public bool JuegoPausado { get; set; } = false;

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

        if (this.JuegoPausado)
            LoggerJuego.Trace("Juego pausado.");
        else
            LoggerJuego.Trace("Juego renaudado.");

        UtilidadesNodos.PausarNodo(this, this.JuegoPausado);

        EmitSignal(SignalName.PauseBattle);
    }

    public void IniciarBatalla()
    {
        if (BatallaEnCurso)
            return;

        BatallaEnCurso = true;
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
}

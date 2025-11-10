namespace Primerjuego2D.escenas.batalla;

using System;
using Godot;
using Primerjuego2D.nucleo.ajustes;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;

public partial class BatallaControlador : Node
{
    [Signal]
    public delegate void PauseBattleEventHandler();

    [Signal]
    public delegate void BatallaIniciadaEventHandler();

    [Signal]
    public delegate void BatallaFinalizadaEventHandler();

    public static bool BatallaEnCurso { get; private set; } = false;

    public override void _Ready()
    {
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(ConstantesAcciones.PAUSAR_JUEGO))
        {
            OnPauseButtonPressed();
        }
    }

    private void OnPauseButtonPressed()
    {
        bool pausarJuego = !Ajustes.JuegoPausado;

        if (pausarJuego)
            GD.Print("Juego pausado.");
        else
            GD.Print("Juego renaudado.");

        UtilidadesNodos.PausarNodo(this, pausarJuego);

        EmitSignal(SignalName.PauseBattle);
    }

    public void IniciarBatalla()
    {
        if (BatallaEnCurso)
            return;

        BatallaEnCurso = true;
        GD.Print("Batalla iniciada.");
        EmitSignal(SignalName.BatallaIniciada);
    }

    public void FinalizarBatalla()
    {
        if (!BatallaEnCurso)
            return;

        BatallaEnCurso = false;
        GD.Print("Batalla finalizada.");
        EmitSignal(SignalName.BatallaFinalizada);
    }
}

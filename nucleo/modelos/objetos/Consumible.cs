using Godot;
using Primerjuego2D.escenas.entidades.jugador;

namespace Primerjuego2D.nucleo.modelos.objetos;

public abstract partial class Consumible : Area2D
{
    private void OnBodyEntered(Node2D body)
    {
        if (body is Jugador jugador)
            OnRecogida(jugador);
    }

    public abstract void OnRecogida(Jugador jugador);
}
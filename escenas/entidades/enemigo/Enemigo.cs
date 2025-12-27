using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;

namespace Primerjuego2D.escenas.entidades.enemigo;

[AtributoNivelLog(NivelLog.Info)]
public partial class Enemigo : RigidBody2D
{
    public const string ANIMATION_FLY = "fly";

    public const string ANIMATION_SWIM = "swim";

    public const string ANIMATION_WALK = "walk";

    public const string GROUP_ENEMIES_NAME = "Enemies";

    private AnimatedSprite2D _AnimatedSprite2D;
    private AnimatedSprite2D AnimatedSprite2D => _AnimatedSprite2D ??= GetNode<AnimatedSprite2D>("AnimatedSprite2D");

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        UtilidadesNodos2D.AjustarZIndexNodo(this, ConstantesZIndex.ENEMIGOS);

        // Seleccionamos aleatoriamente una animación de entre todas las disponibles para enemigos.
        List<string> enemyAnimations = new() { ANIMATION_FLY, ANIMATION_SWIM, ANIMATION_WALK };
        int enemyAnimation = Randomizador.GetRandomInt(0, enemyAnimations.Count);

        this.AnimatedSprite2D.Animation = enemyAnimations[enemyAnimation];
        this.AnimatedSprite2D.Play();
    }

    private void EliminarEnemigo()
    {
        LoggerJuego.Trace("Enemigo Eliminado.");

        // Cuando el enemigo salga de la pantalla, se elimina a sí mismo.
        QueueFree();
    }
}

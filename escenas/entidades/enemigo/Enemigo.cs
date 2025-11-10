namespace Primerjuego2D.escenas.entidades.enemigo;

using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Primerjuego2D.nucleo.utilidades;

public partial class Enemigo : RigidBody2D
{
    public const string ANIMATION_FLY = "fly";

    public const string ANIMATION_SWIM = "swim";

    public const string ANIMATION_WALK = "walk";

    public const string GROUP_ENEMIES = "Enemies";

    private AnimatedSprite2D _AnimatedSprite2D;
    private AnimatedSprite2D AnimatedSprite2D => _AnimatedSprite2D ??= GetNode<AnimatedSprite2D>("AnimatedSprite2D");

    public static Array<Node> GetAllEnemies(Node parentNode) => parentNode.GetTree().GetNodesInGroup(GROUP_ENEMIES);

    public static void DeleteAllEnemies(Node parentNode) => parentNode.GetTree().CallGroup(GROUP_ENEMIES, Node.MethodName.QueueFree);

    public override void _Ready()
    {

        // Seleccionamos aleatoriamente una animación de entre todas las disponibles para enemigos.
        List<string> enemyAnimations = new() { ANIMATION_FLY, ANIMATION_SWIM, ANIMATION_WALK };
        int enemyAnimation = Randomizer.GetRandomInt(0, enemyAnimations.Count);

        this.AnimatedSprite2D.Animation = enemyAnimations[enemyAnimation];
        this.AnimatedSprite2D.Play();
    }

    private void OnVisibleOnScreenNotifier2DScreenExited()
    {
        QueueFree();
    }
}

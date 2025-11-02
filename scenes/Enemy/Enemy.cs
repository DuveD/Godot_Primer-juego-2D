using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Enemy : RigidBody2D
{
    public const string ANIMATION_FLY = "fly";

    public const string ANIMATION_SWIM = "swim";

    public const string ANIMATION_WALK = "walk";

    public const string GROUP_ENEMIES = "Enemies";

    public static Array<Node> GetAll(Node parentNode) => parentNode.GetTree().GetNodesInGroup(GROUP_ENEMIES);

    public static void DeleteAll(Node parentNode) => parentNode.GetTree().CallGroup(GROUP_ENEMIES, Node.MethodName.QueueFree);

    public override void _Ready()
    {
        AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>(nameof(AnimatedSprite2D));

        // Seleccionamos aleatoriamente una animación de entre todas las disponibles para enemigos.
        List<string> enemyAnimations = new () { ANIMATION_FLY, ANIMATION_SWIM, ANIMATION_WALK };
        int enemyAnimation = Randomizer.GetRandomInt(0, enemyAnimations.Count);

        animatedSprite2D.Animation = enemyAnimations[enemyAnimation];
        animatedSprite2D.Play();
    }

    private void OnVisibleOnScreenNotifier2DScreenExited()
    {
        QueueFree();
    }
}

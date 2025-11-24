using Godot;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.miscelaneo;

public partial class Fondo : Control
{
    private ColorRect _ColorFondo;

    private ColorRect ColorFondo => _ColorFondo ??= GetNode<ColorRect>("ColorFondo");

    private GpuParticles2D _GpuParticles2D;

    private GpuParticles2D GpuParticles2D => _GpuParticles2D ??= GetNode<GpuParticles2D>("ControlParticulas/GpuParticles2D");

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        this.ColorFondo.Color = Global.GestorColor.ColorFondo;
    }
}

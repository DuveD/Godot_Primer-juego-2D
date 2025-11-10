namespace Primerjuego2D.nucleo.utilidades;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.nucleo.ajustes;

public static class UtilidadesNodos
{
    /// <summary>
    /// Pausa o reanuda el nodo y todo su árbol.
    /// </summary>
    public static void PausarNodo(Node node, bool pausar, bool pausarJuego = true)
    {
        node.GetTree().Paused = pausar;
        Ajustes.JuegoPausado = pausar;
    }

    /// <summary>
    /// Devuelve si el nodo está pausado.
    /// </summary>
    public static bool NodoPausado(Node node)
    {
        return node.GetTree().Paused;
    }

    /// <summary>
    /// Espera una cantidad de segundos respetando o no la pausa del juego.
    /// </summary>
    public static async Task EsperarSegundos(Node node, double segundos, bool respetarPausa = true)
    {
        await node.ToSignal(node.GetTree().CreateTimer(segundos, respetarPausa), SceneTreeTimer.SignalName.Timeout);
    }

    /// <summary>
    /// Esconde todos los nodos hijos de un nodo padre, excepto los nodos indicados.
    /// </summary>
    public static void EsconderMenos(Node padre, params CanvasItem[] nodosExcluidos)
    {
        // Usamos HashSet para búsquedas O(1) en lugar de Array.IndexOf (O(n))
        var excluidos = new HashSet<CanvasItem>(nodosExcluidos);

        foreach (var hijo in padre.GetChildren())
        {
            if (hijo is CanvasItem canvasItem && !excluidos.Contains(canvasItem))
                canvasItem.Hide();
        }
    }

    public static void EsconderTodo(Node padre)
    {
        EsconderMenos(padre);
    }

    /// <summary>
    /// Muestra todos los nodos hijos de un nodo padre, excepto los nodos indicados.
    /// </summary>
    public static void MostrarMenos(Node padre, params CanvasItem[] nodosExcluidos)
    {
        // Usamos HashSet para búsquedas O(1) en lugar de Array.IndexOf (O(n))
        var excluidos = new HashSet<CanvasItem>(nodosExcluidos);

        foreach (var hijo in padre.GetChildren())
        {
            if (hijo is CanvasItem canvasItem && !excluidos.Contains(canvasItem))
                canvasItem.Show();
        }
    }

    /// <summary>
    /// Muestra todos los nodos hijos de un nodo padre.
    /// </summary>
    public static void MostrarTodo(Node padre)
    {
        MostrarMenos(padre);
    }

    /// <summary>
    /// Espera hasta que el nodo se reanude si está pausado.
    /// </summary>
    public static async Task EsperarRenaudar(Node node)
    {
        while (node.GetTree().Paused)
            await node.ToSignal(node.GetTree(), "process_frame");
    }
}
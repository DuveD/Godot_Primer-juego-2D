using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Primerjuego2D.nucleo.utilidades;

public static class UtilidadesNodos
{
    /// <summary>
    /// Pausa o reanuda el nodo y todo su árbol.
    /// </summary>
    public static void PausarNodo(Node node, bool pausar)
    {
        node.GetTree().Paused = pausar;
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


    /// <summary>
    /// Marca el ítem del PopupMenu correspondiente al ID dado y desmarca los demás.
    /// </summary>
    public static void CheckItemPorId(PopupMenu popupMenu, long id)
    {
        int index = popupMenu.GetItemIndex((int)id);

        for (int i = 0; i < popupMenu.ItemCount; i++)
        {
            if (i != index)
                popupMenu.SetItemChecked(i, false);
            else
                popupMenu.SetItemChecked(i, true);
        }
    }

    /// <summary>
    /// Obtiene la ruta de la escena correspondiente a la clase dada.
    /// </summary>
    public static string ObtenerRutaEscena<T>(string root = "res://") where T : Node
    {
        Type type = typeof(T);

        string path = "";

        if (!string.IsNullOrEmpty(type.Namespace))
        {
            // Dividimos el namespace por puntos
            var parts = type.Namespace.Split('.');

            // Eliminamos la primera parte (base del namespace)
            if (parts.Length > 1)
            {
                path = string.Join("/", parts, 1, parts.Length - 1) + "/";
            }
        }

        // Añadimos el nombre de la clase y la extensión
        path += type.Name + ".tscn";

        // Devolvemos la ruta completa respecto a res://
        return root + path;
    }

    /// <summary>
    /// Borra todos los nodos hijos del nodo dado.
    /// </summary>
    public static void BorrarHijos(Node node)
    {
        foreach (Node child in node.GetChildren())
            child.QueueFree();
    }

    /// <summary>
    /// Obtiene un nodo hijo por su nombre.
    /// </summary>
    public static T ObtenerNodoPorNombre<T>(Node nodoPadre, string nombre) where T : Node
    {
        if (nodoPadre.Name == nombre && nodoPadre is T match)
        {
            return match;
        }
        else if (nodoPadre.GetChildren().Count > 0)
        {
            foreach (Node child in nodoPadre.GetChildren())
            {
                var result = ObtenerNodoPorNombre<T>(child, nombre);
                if (result != null)
                    return result;
            }
        }

        return null;
    }


    /// <summary>
    /// Obtiene el primer nodo hijo del tipo indicado.
    /// </summary>
    public static T ObtenerNodoDeTipo<T>(Node nodoPadre, bool? visible = null) where T : Node
    {
        T resultado = null;

        if (nodoPadre == null)
            return resultado;

        if (nodoPadre is T nodoDelTipo)
        {
            if (visible == null || (nodoDelTipo is CanvasItem ci && ci.Visible == visible))
            {
                resultado = nodoDelTipo;
            }
        }

        foreach (Node hijo in nodoPadre.GetChildren())
        {
            resultado = ObtenerNodoDeTipo<T>(hijo, visible);
            if (resultado != null)
                break;
        }

        return resultado;
    }

    /// <summary>
    /// Obtiene todos los nodos hijos del tipo indicado.
    /// </summary>
    public static List<T> ObtenerNodosDeTipo<T>(Node nodoPadre, bool? visible = null) where T : Node
    {
        var resultado = new List<T>();

        if (nodoPadre == null)
            return resultado;

        if (nodoPadre is T nodoDelTipo)
        {
            if (visible == null || (nodoDelTipo is CanvasItem ci && ci.Visible == visible))
            {
                resultado.Add(nodoDelTipo);
            }
        }

        foreach (Node hijo in nodoPadre.GetChildren())
        {
            resultado.AddRange(ObtenerNodosDeTipo<T>(hijo, visible));
        }

        return resultado;
    }

    public static void PulsarBoton(Button boton)
    {
        boton?.EmitSignal(BaseButton.SignalName.Pressed);
    }
}
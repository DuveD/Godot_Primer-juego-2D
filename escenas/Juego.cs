using Godot;
using Primerjuego2D.escenas.batalla;
using Primerjuego2D.escenas.menuPrincipal;
using Primerjuego2D.escenas.sistema.camara;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas;

public partial class Juego : Control
{
    [Export]
    public CamaraPrincipal _Camara { get; set; }    // Nodo de la escena
    public static CamaraPrincipal Camara { get; private set; }

    private Control _ContenedorEscena;
    private Control ContenedorEscena => _ContenedorEscena ??= GetNode<Control>("ContenedorEscena");

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        Juego.Camara ??= _Camara;

        AjustaViewPortYCamara();

        CargarMenuPrincipal();
    }

    private void AjustaViewPortYCamara()
    {
        // Ajustamos el tamaño del juego al tamaño de la pantalla.
        this.Size = GetViewportRect().Size;

        // Ajustamos el tamaño de la cámara al tamaño del juego.
        Juego.Camara.AjustarCamara(this.Size);
        GetViewport().SizeChanged += () => Juego.Camara.AjustarCamara(this.Size);
    }

    public void CargarMenuPrincipal()
    {
        LoggerJuego.Trace("Cargando menú principal.");

        string rutaMenuprincipal = UtilidadesNodos.ObtenerRutaEscena<MenuPrincipal>();
        MenuPrincipal menuPrincipal = (MenuPrincipal)CambiarPantalla(rutaMenuprincipal);

        menuPrincipal.BotonEmpezarPartidaPulsado += CargarBatalla;
    }

    public void CargarBatalla()
    {
        LoggerJuego.Trace("Cargando batalla.");

        string rutaBatalla = UtilidadesNodos.ObtenerRutaEscena<Batalla>();
        Batalla batalla = (Batalla)CambiarPantalla(rutaBatalla);

        batalla.GameOverFinalizado += CargarMenuPrincipal;
    }

    public Node CambiarPantalla(string rutaEscena)
    {
        // Cargamos la escena desde la ruta proporcionada.
        PackedScene pantalla = ResourceLoader.Load<PackedScene>(rutaEscena);

        if (pantalla == null)
        {
            LoggerJuego.Error($"No se pudo cargar la escena en la ruta: {rutaEscena}");
            return null;
        }

        // Instanciamos la escena cargada.
        Node instanciaEscena = pantalla.Instantiate();
        if (instanciaEscena == null)
        {
            LoggerJuego.Error($"No se pudo instanciar la escena desde la ruta: {rutaEscena}");
            return null;
        }

        // Limpiamos el contenedor de escenas actual.
        UtilidadesNodos.BorrarHijos(this.ContenedorEscena);

        // Añadimos la nueva escena al contenedor.
        this.ContenedorEscena.AddChild(instanciaEscena);

        return instanciaEscena;
    }
}

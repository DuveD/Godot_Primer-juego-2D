using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.sistema;

public partial class GestorAudio : Node
{
	public const string RUTA_MUSICA = "res://recursos/audio/musica";

	public const string RUTA_SONIDOS = "res://recursos/audio/sonidos";

	public readonly string[] ExtensionesAceptadas = { ".mp3", ".wav", ".wav" };

	[Export] public int NumSfxPlayers { get; set; } = 5;

	[Export] public Node EffectsContainer { get; set; }
	[Export] public AudioStreamPlayer ReproductorMusica { get; set; }
	[Export] public AudioStreamPlayer ReproductorMusica2 { get; set; }

	private readonly List<AudioStreamPlayer> _sfxPlayers = new();
	private readonly Dictionary<string, AudioStream> _cacheMusica = new();
	private readonly Dictionary<string, AudioStream> _cacheSonidos = new();

	private float _posicionPausa = 0f;

	private bool _crossfadeEnProceso = false;

	private Queue<string> _colaCrossfade = new();

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		InicializarSfxPool();
		CargarRecursosAudio();
	}

	private void InicializarSfxPool()
	{
		_sfxPlayers.Clear();
		for (int i = 0; i < NumSfxPlayers; i++)
		{
			var player = new AudioStreamPlayer();
			EffectsContainer.AddChild(player);
			_sfxPlayers.Add(player);
		}
	}

	private void CargarRecursosAudio()
	{
		// Cargar música
		var archivosMusica = DirAccess.Open(RUTA_MUSICA).GetFiles();
		archivosMusica = FiltrarExtensionesAceptadas(archivosMusica);
		foreach (string nombreArchivoMusica in archivosMusica)
		{
			var musica = ObtenerMusica(nombreArchivoMusica);
			if (musica != null)
				_cacheMusica[nombreArchivoMusica] = musica;
		}

		// Cargar efectos
		var archivosSonidos = DirAccess.Open(RUTA_SONIDOS).GetFiles();
		archivosSonidos = FiltrarExtensionesAceptadas(archivosSonidos);
		foreach (string nombreArchivoSonido in archivosSonidos)
		{
			var sonido = ObtenerSonido(nombreArchivoSonido);
			if (sonido != null)
				_cacheSonidos[nombreArchivoSonido] = sonido;
		}
	}

	private string[] FiltrarExtensionesAceptadas(string[] nombresArchivos)
	{
		if (nombresArchivos == null)
			return [];

		return nombresArchivos.Where(nombreArchivo => ExtensionesAceptadas.Contains(System.IO.Path.GetExtension(nombreArchivo).ToLower())).ToArray();
	}
	private AudioStream ObtenerSonido(string nombreArchivoSonido)
	{
		return ObtenerAudio(RUTA_SONIDOS + "/" + nombreArchivoSonido);
	}

	private AudioStream ObtenerMusica(string nombreArchivoMusica)
	{
		return ObtenerAudio(RUTA_MUSICA + "/" + nombreArchivoMusica);
	}

	private AudioStream ObtenerAudio(string rutaArchivoAudio)
	{
		return ResourceLoader.Load<AudioStream>(rutaArchivoAudio);
	}

	private AudioStreamPlayer BuscarPlayerLibre()
	{
		foreach (var p in _sfxPlayers)
			if (!p.Playing)
				return p;
		return _sfxPlayers[0]; // pool circular
	}

	public void ReproducirSonido(string nombre)
	{
		if (!_cacheSonidos.TryGetValue(nombre, out var sonido))
		{
			LoggerJuego.Error($"Efecto no encontrado: {nombre}");
			return;
		}

		var player = BuscarPlayerLibre();
		player.Stream = sonido;
		player.Play();
	}

	public void ReproducirMusica(string nombre)
	{
		if (!_cacheMusica.TryGetValue(nombre, out var cancion))
		{
			LoggerJuego.Error($"Música no encontrada: {nombre}");
			return;
		}

		if (ReproductorMusica.Stream == cancion && ReproductorMusica.Playing)
			return;

		ReproductorMusica.Stream = cancion;
		ReproductorMusica.Play(_posicionPausa);
		_posicionPausa = 0f;
	}

	public void PauseMusic()
	{
		_posicionPausa = ReproductorMusica.GetPlaybackPosition();
		ReproductorMusica.Stop();
	}

	public void StopMusic()
	{
		_posicionPausa = 0f;
		ReproductorMusica.Stop();
	}

	public void Crossfade(string nuevaCancion, float duracion = 2.0f)
	{
		if (string.IsNullOrEmpty(nuevaCancion)) return;

		_colaCrossfade.Enqueue(nuevaCancion);
		if (!_crossfadeEnProceso)
		{
			_ = EjecutarCrossfade(duracion);
		}
	}

	private async System.Threading.Tasks.Task EjecutarCrossfade(float duracion)
	{
		while (_colaCrossfade.Count > 0)
		{
			string cancionNombre = _colaCrossfade.Dequeue();
			if (!_cacheMusica.TryGetValue(cancionNombre, out var cancion))
			{
				LoggerJuego.Error($"Música no encontrada: {cancionNombre}");
				continue;
			}

			_crossfadeEnProceso = true;

			ReproductorMusica2.Stream = cancion;
			ReproductorMusica2.VolumeDb = -80;
			ReproductorMusica2.Play();

			float tiempo = 0f;
			while (tiempo < duracion)
			{
				float t = tiempo / duracion;
				ReproductorMusica.VolumeDb = Mathf.Lerp(0, -80, t);
				ReproductorMusica2.VolumeDb = Mathf.Lerp(-80, 0, t);

				await ToSignal(GetTree(), "process_frame");
				tiempo += (float)GetProcessDeltaTime();
			}

			ReproductorMusica.Stop();
			ReproductorMusica.Stream = cancion;
			ReproductorMusica.VolumeDb = 0;

			(ReproductorMusica2, ReproductorMusica) = (ReproductorMusica, ReproductorMusica2);

			_crossfadeEnProceso = false;
		}
	}
}

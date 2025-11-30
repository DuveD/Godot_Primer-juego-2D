using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.nucleo.configuracion;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;

namespace Primerjuego2D.escenas.sistema;

[AtributoNivelLog(NivelLog.Info)]
public partial class GestorAudio : Node
{
	public const string RUTA_MUSICA = "res://recursos/audio/musica";

	public const string RUTA_SONIDOS = "res://recursos/audio/sonidos";

	public readonly string[] ExtensionesAceptadas = { ".mp3", ".wav", ".ogg" };

	private int _sfxIndex = 0;

	[Export]
	private int _numSfxPlayers { get; set; } = 5;

	private readonly List<AudioStreamPlayer> _sfxPlayers = new();

	private Node _EffectsContainer;
	private Node EffectsContainer => _EffectsContainer ??= GetNode<Node>("EffectsContainer");

	private AudioStreamPlayer _AudioStreamPlayer;
	private AudioStreamPlayer AudioStreamPlayer => _AudioStreamPlayer ??= GetNode<AudioStreamPlayer>("AudioStreamPlayer");

	private AudioStreamPlayer _AudioStreamPlayer2;
	private AudioStreamPlayer AudioStreamPlayer2 => _AudioStreamPlayer2 ??= GetNode<AudioStreamPlayer>("AudioStreamPlayer2");

	private readonly Dictionary<string, AudioStream> _cacheMusica = new();

	private readonly Dictionary<string, AudioStream> _cacheSonidos = new();

	private float _posicionPausa = 0f;

	private bool _crossfadeEnProceso = false;

	public float VolumenGeneral
	{
		get => Ajustes.VolumenGeneral;
		set
		{
			Ajustes.VolumenGeneral = Mathf.Clamp(value, 0f, 1f);
			ActualizarVolumenGlobal();
		}
	}

	public float VolumenMusica
	{
		get => Ajustes.VolumenMusica;
		set
		{
			Ajustes.VolumenMusica = Mathf.Clamp(value, 0f, 1f);
			ActualizarVolumenMusica();
		}
	}

	public float VolumenSonidos
	{
		get => Ajustes.VolumenSonidos;
		set
		{
			Ajustes.VolumenSonidos = Mathf.Clamp(value, 0f, 1f);
			ActualizarVolumenSfx();
		}
	}

	private Queue<string> _colaCrossfade = new();

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		InicializarSfxPool();
		CargarRecursosAudio();

		// Inicializar volúmenes según ajustes
		ActualizarVolumenGlobal();
	}

	private void InicializarSfxPool()
	{
		_sfxPlayers.Clear();

		for (int i = 0; i < _numSfxPlayers; i++)
		{
			var player = new AudioStreamPlayer();
			EffectsContainer.AddChild(player);

			// Conectamos la señal Finished...
			player.Finished += () => LiberarSfxPlayer(player);

			_sfxPlayers.Add(player);
		}
	}

	private void LiberarSfxPlayer(AudioStreamPlayer player)
	{
		// ...para marcarlo como libre.
		player.Stream = null;
		LoggerJuego.Trace($"Player '{player.Name}' terminado.");
	}

	private void CargarRecursosAudio()
	{
		// Cargar música
		CargarRecursosMusica();

		// Cargar efectos
		CargarRecursosSonidos();
	}

	private void CargarRecursosMusica()
	{
		var archivosMusica = DirAccess.Open(RUTA_MUSICA).GetFiles();
		if (archivosMusica == null)
		{
			LoggerJuego.Error($"No se pudo abrir la carpeta: {RUTA_MUSICA}");
		}
		else
		{
			archivosMusica = FiltrarExtensionesAceptadas(archivosMusica);
			foreach (string nombreArchivoMusica in archivosMusica)
			{
				var musica = ObtenerMusica(nombreArchivoMusica);
				if (musica != null)
					_cacheMusica[nombreArchivoMusica] = musica;
			}
		}
	}

	private void CargarRecursosSonidos()
	{
		var archivosSonidos = DirAccess.Open(RUTA_SONIDOS).GetFiles();
		if (archivosSonidos == null)
		{
			LoggerJuego.Error($"No se pudo abrir la carpeta: {RUTA_SONIDOS}");
		}
		else
		{
			archivosSonidos = FiltrarExtensionesAceptadas(archivosSonidos);
			foreach (string nombreArchivoSonido in archivosSonidos)
			{
				var sonido = ObtenerSonido(nombreArchivoSonido);
				if (sonido != null)
					_cacheSonidos[nombreArchivoSonido] = sonido;
			}
		}
	}

	private void ActualizarVolumenGlobal()
	{
		// Esto puede afectar música y SFX simultáneamente si quieres un volumen maestro
		ActualizarVolumenMusica();
		ActualizarVolumenSfx();
	}

	private void ActualizarVolumenMusica()
	{
		if (AudioStreamPlayer != null)
			AudioStreamPlayer.VolumeDb = LinearToDb(VolumenMusica * VolumenGeneral);

		if (AudioStreamPlayer2 != null)
			AudioStreamPlayer2.VolumeDb = LinearToDb(VolumenMusica * VolumenGeneral);
	}

	private void ActualizarVolumenSfx()
	{
		float volumen = LinearToDb(VolumenSonidos * VolumenGeneral);
		foreach (var player in _sfxPlayers)
		{
			player.VolumeDb = volumen;
		}
	}

	// Convierte volumen lineal [0,1] a dB [-80,0]
	private static float LinearToDb(float linear)
	{
		linear = Mathf.Clamp(linear, 0.0001f, 1f); // evitar log(0)
		return (float)(20.0 * Math.Log10(linear));
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
		return ResourceLoader.Load<AudioStream>(rutaArchivoAudio, cacheMode: ResourceLoader.CacheMode.Reuse);
	}

	private AudioStreamPlayer BuscarPlayerLibre()
	{
		// 🔍 Buscamos un player que no esté reproduciendo nada
		foreach (var player in _sfxPlayers)
		{
			if (!player.Playing && player.Stream == null)
				return player;
		}

		// 🔄 Si no hay libres, uso circular como fallback
		var p2 = _sfxPlayers[_sfxIndex];
		_sfxIndex = (_sfxIndex + 1) % _sfxPlayers.Count;
		return p2;
	}

	public void ReproducirSonido(string nombreSonido)
	{
		if (!_cacheSonidos.TryGetValue(nombreSonido, out var sonido))
		{
			LoggerJuego.Error($"Efecto no encontrado: '{nombreSonido}'");
			return;
		}

		var player = BuscarPlayerLibre();
		player.Stream = sonido;
		player.Play();

		LoggerJuego.Trace($"Reproduciendo sonido '{nombreSonido}'.");
	}

	public void ReproducirMusica(string nombreMusica)
	{
		if (!_cacheMusica.TryGetValue(nombreMusica, out var cancion))
		{
			LoggerJuego.Error($"Música no encontrada: '{nombreMusica}'");
			return;
		}

		if (AudioStreamPlayer.Stream == cancion && AudioStreamPlayer.Playing)
			return;

		AudioStreamPlayer.Stream = cancion;
		AudioStreamPlayer.Play(_posicionPausa);
		_posicionPausa = 0f;

		LoggerJuego.Trace($"Reproduciendo música '{nombreMusica}'.");
	}

	public void PauseMusic()
	{
		_posicionPausa = AudioStreamPlayer.GetPlaybackPosition();
		AudioStreamPlayer.Stop();
	}

	public void StopMusic()
	{
		_posicionPausa = 0f;
		AudioStreamPlayer.Stop();
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
		if (_crossfadeEnProceso)
			return;

		_crossfadeEnProceso = true;

		while (_colaCrossfade.Count > 0)
		{
			string cancionNombre = _colaCrossfade.Dequeue();
			if (!_cacheMusica.TryGetValue(cancionNombre, out var cancion))
			{
				LoggerJuego.Error($"Música no encontrada: {cancionNombre}");
				continue;
			}

			AudioStreamPlayer2.Stream = cancion;
			AudioStreamPlayer2.VolumeDb = -80;
			AudioStreamPlayer2.Play();

			float tiempo = 0f;
			while (tiempo < duracion)
			{
				float t = tiempo / duracion;
				AudioStreamPlayer.VolumeDb = Mathf.Lerp(0, -80, t);
				AudioStreamPlayer2.VolumeDb = Mathf.Lerp(-80, 0, t);

				await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
				float delta = (float)GetPhysicsProcessDeltaTime();
				tiempo += delta;
			}

			AudioStreamPlayer.Stop();
			AudioStreamPlayer.Stream = cancion;
			AudioStreamPlayer.VolumeDb = 0;

			(_AudioStreamPlayer2, _AudioStreamPlayer) = (_AudioStreamPlayer, _AudioStreamPlayer2);
		}

		_crossfadeEnProceso = false;
	}
}

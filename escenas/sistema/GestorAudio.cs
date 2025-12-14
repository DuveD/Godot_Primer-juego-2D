using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

	private bool _fadeEnProceso = false;

	private float _VolumenGeneral;
	public float VolumenGeneral
	{
		get => _VolumenGeneral;
		set
		{
			_VolumenGeneral = Mathf.Clamp(value, 0f, 1f);
			ActualizarVolumenGlobal();
		}
	}

	public float _VolumenMusica;
	public float VolumenMusica
	{
		get => _VolumenMusica;
		set
		{
			_VolumenMusica = Mathf.Clamp(value, 0f, 1f);
			ActualizarVolumenMusica();
		}
	}

	public float _VolumenSonidos;
	public float VolumenSonidos
	{
		get => _VolumenSonidos;
		set
		{
			_VolumenSonidos = Mathf.Clamp(value, 0f, 1f);
			ActualizarVolumenSfx();
		}
	}

	private Queue<string> _colaCrossfade = new();

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		InicializarSfxPool();
		CargarRecursosAudio();

		this._VolumenGeneral = Ajustes.VolumenGeneral;
		this._VolumenMusica = Ajustes.VolumenMusica;
		this._VolumenSonidos = Ajustes.VolumenSonidos;

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

	private string ObtenerNombreCancionActual()
	{
		if (AudioStreamPlayer.Stream == null) return "<ninguna>";
		return _cacheMusica.FirstOrDefault(kv => kv.Value == AudioStreamPlayer.Stream).Key ?? "<desconocida>";
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

	public async void ReproducirMusica(string nombreCancion, float duracionFade = 0f)
	{
		// Comprobamos que la canción exista.
		if (!_cacheMusica.TryGetValue(nombreCancion, out var cancion))
		{
			LoggerJuego.Error($"Música no encontrada: '{nombreCancion}'");
			return;
		}

		// Esperamos a que termine un crossfade o un fade en proceso.
		while (_fadeEnProceso || _crossfadeEnProceso)
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

		bool reanudando = false;

		// Comprobamos si es la misma canción.
		if (AudioStreamPlayer.Stream == cancion)
		{
			if (AudioStreamPlayer.Playing)
			{
				LoggerJuego.Trace($"La canción '{nombreCancion}' ya se está reproduciendo.");
				return;
			}

			reanudando = _posicionPausa > 0f;
		}

		float posicionInicial = reanudando ? _posicionPausa : 0f;

		// 4Asignamos la canción al player principal.
		AudioStreamPlayer.Stream = cancion;

		// Reproducidmos desde la posición adecuada.
		AudioStreamPlayer.Play(posicionInicial);

		float volumenObjetivo = LinearToDb(VolumenMusica * VolumenGeneral);
		_posicionPausa = 0f;

		// Hacemos fade-in opcional si se ha informado.
		if (!reanudando || duracionFade <= 0f)
		{
			AudioStreamPlayer.VolumeDb = volumenObjetivo;

			LoggerJuego.Trace($"Reproduciendo música '{nombreCancion}' sin fade.");
		}
		else
		{
			LoggerJuego.Trace($"Reanudando música '{nombreCancion}' con fade-in.");

			AudioStreamPlayer.VolumeDb = -80f;

			float tiempo = 0f;
			while (tiempo < duracionFade)
			{
				float t = tiempo / duracionFade;
				AudioStreamPlayer.VolumeDb = Mathf.Lerp(-80f, volumenObjetivo, t);

				await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
				tiempo += (float)GetPhysicsProcessDeltaTime();
			}

			// Dejamos el volumen final normalizado.
			AudioStreamPlayer.VolumeDb = volumenObjetivo;

			LoggerJuego.Trace($"Música '{nombreCancion}' renaudada con fade-in.");
		}
	}

	public bool MusicaReproduciendo()
	{
		return AudioStreamPlayer.Playing;
	}


	public async void PausarMusica(float duracionFade = 0f)
	{
		if (_fadeEnProceso) return; // Evitamos solapamiento

		_fadeEnProceso = true;

		if (duracionFade <= 0f)
		{
			_posicionPausa = AudioStreamPlayer.GetPlaybackPosition();
			AudioStreamPlayer.Stop();
			_fadeEnProceso = false;
		}
		else
		{
			LoggerJuego.Trace($"Iniciando fade-out para la canción '{ObtenerNombreCancionActual()}'.");

			float inicioVolDb = AudioStreamPlayer.VolumeDb;
			float tiempo = 0f;

			while (tiempo < duracionFade)
			{
				float t = tiempo / duracionFade;
				AudioStreamPlayer.VolumeDb = Mathf.Lerp(inicioVolDb, -80f, t);

				await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
				tiempo += (float)GetPhysicsProcessDeltaTime();
			}

			AudioStreamPlayer.VolumeDb = -80f;
			_posicionPausa = AudioStreamPlayer.GetPlaybackPosition();
			AudioStreamPlayer.Stop();
			_fadeEnProceso = false;

			LoggerJuego.Trace($"Fade-out terminado para la canción '{ObtenerNombreCancionActual()}'.");
		}
	}

	public void PararMusica()
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

	private async Task EjecutarCrossfade(float duracion)
	{
		if (_crossfadeEnProceso)
		{
			LoggerJuego.Warn($"Ya hay un cross-fade en proceso.");
			return;
		}

		_crossfadeEnProceso = true;

		while (_colaCrossfade.Count > 0)
		{
			string cancionNombre = _colaCrossfade.Dequeue();
			if (!_cacheMusica.TryGetValue(cancionNombre, out var cancion))
			{
				LoggerJuego.Error($"Música no encontrada: {cancionNombre}.");
				continue;
			}

			LoggerJuego.Trace($"Iniciando cross-ffade para: {cancionNombre}.");

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

			// Dejamos el volumen final normalizado.
			AudioStreamPlayer2.VolumeDb = LinearToDb(VolumenMusica * VolumenGeneral);

			AudioStreamPlayer.Stop();
			AudioStreamPlayer.Stream = cancion;
			AudioStreamPlayer.VolumeDb = 0;

			(_AudioStreamPlayer2, _AudioStreamPlayer) = (_AudioStreamPlayer, _AudioStreamPlayer2);

			LoggerJuego.Trace($"Cross-ffade terminado para: {cancionNombre}.");
		}

		_crossfadeEnProceso = false;
	}
}

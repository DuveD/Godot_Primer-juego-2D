# 🕹️ Primer Juego 2D (Godot)

Un juego simple 2D hecho con Godot 4.x, basado en el tutorial oficial "[Your First 2D Game](https://docs.godotengine.org/en/4.4/getting_started/first_2d_game/index.html)".
He ampliado el proyecto con características extra para aprender más sobre Godot y C#.

---

## 📂 Estructura del proyecto

- escenas/ → Todas las escenas del juego: Batalla (BatallaHUD, BatallaControlador), Jugador, Enemigo, etc.

- nucleo/ → Archivos de configuración, archivos de constantes, localización, utilidades, etc.

- recursos/ → Imágenes, audio, fuentes, archivos de traduccion, etc.

## ⚡ Características añadidas

### v1.4.0

- Cambio de feedback de uso de los botones del menú principal según se use teclado o ratón.
- Añadido control para no spawnear monedas cerca del jugador.

### v1.3.0

- Añadido spawn de monedas con animación vinculadas a la puntuación.
- Gestor de audio avanzado con pool de reproductores para sonidos.
- Diferentes niveles de audio para música y efectos.
- Gestión de ajustes persistido en fichero 'ajustes.ini'.

### v1.2.0

- Sistema de Logger con niveles Trace, Info, Warning y Error.
- Menú principal.
- Fondo con partículas.
- Proyecto actualizado a Godot 4.5.1.
- Efecto Shake al ser golpeado por un enemigo.

### v1.1.0

- Gestión de la localización e idiomas Español e Inglés.
- Pausa de la partida.

### v1.0.0

- Movimiento en 8 direcciones con "animación" correspondiente.
- Juego base implementado.

## 📖 Referencias

- Tutorial oficial de Godot 4: [Your First 2D Game](https://docs.godotengine.org/en/4.4/getting_started/first_2d_game/index.html)
- Video tutorial de Alva Majo: [Godot para retrasados [Tutorial]](https://www.youtube.com/watch?v=eQ_HBvtdoiU&t=663s)
- Andrew Vickerman Godot Audio Manager: [Godot Audio Manager](https://github.com/insideout-andrew/godot-audio-manager/tree/main)
- Video tutorial de Rayuse: [Start Menu Keyboard Selection and Shortcuts in Godot](https://www.youtube.com/watch?v=hXXSWhsjp6M)

## Créditos

- Recursos del tutorial de Godot: © 2014-present Juan Linietsky, Ariel Manzur y la comunidad de Godot (CC BY 3.0)
- _digital_click.mp3_ by CreatorsHome ([Digital Click](https://pixabay.com/es/sound-effects/digital-click-357350/))
- _game-over-arcade.mp3_ by freesound_community ([Game Over Arcade](https://pixabay.com/es/sound-effects/game-over-arcade-6435/))
- _retro_song.mp3_ by H-Beats ([Retro game effects](https://pixabay.com/es/sound-effects/retro-game-effects-252988/))
- _retro_coin.mp3_ by Driken5482 ([Retro coin 4](https://pixabay.com/es/sound-effects/retro-coin-4-236671/))
- _retro_wave.mp3_ by van_Wiese ([Retro wave loop 125 BPM](https://pixabay.com/es/sound-effects/retro-wave-loop-125-bpm-8963/))
- _kick.mp3_ by u_9ikddrpcfz ([Kick](https://pixabay.com/es/sound-effects/kick-182227/))

## ⚖️ Aviso de uso

Este proyecto se ha realizado **únicamente con fines educativos** y de aprendizaje.  
No pretende comercializar ni redistribuir los contenidos originales de Godot ni de los autores de los recursos utilizados.

El contenido original de este proyecto está bajo el copyright de los autores de los diferentes recursos.

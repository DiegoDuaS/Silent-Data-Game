# Silent-Data-Game
Desarrollo de Juego para el curso de Progamación de Videojuegos

## Mecánicas Generales de los enemigos
Todos los enemigos comparten un núcleo lógico de detección:

- Visión: Cono de visión dinámico que detecta al jugador tras un tiempo de exposición (timeToDetect).

- Oído: Radio de proximidad que detecta al jugador si este corre (Shift no presionado).

- Investigación: Si el enemigo pierde al jugador o escucha un ruido, se desplazará a la Last Known Position para buscar antes de retomar su patrulla.

## Tipos de Enemigos
### 1. Guardia Armado (`ArmedGuardBehaviour`)
Unidad de presión a distancia diseñada para mantener al jugador bajo fuego.
* **Combate:** Al detectar al jugador, corre hacia él hasta alcanzar la `shootingDistance`. En rango, se detiene y rota para apuntar con precisión.
* **Velocidad:** Cuenta con un modo de persecución agresivo (`chaseSpeed`) y una caminata lenta cuando sospecha (`suspiciousSpeed`).
* **Especialidad:** Control de áreas abiertas y ataque a distancia.

### 2. Guardia Melee (`GuardBehaviour`)
Unidad persistente diseñada para la persecución cercana y el combate cuerpo a cuerpo.
* **Combate:** No se detiene; persigue al jugador hasta estar a la `attackDistance` (aprox. 1 metro).
* **Mecánica de Ataque:** Posee un sistema de **Cooldown** para regular los golpes y un `ResetTrigger` que cancela la animación si el jugador escapa del rango justo a tiempo.
* **Especialidad:** Posee una **vista más lejana** que el guardia armado, ideal para pasillos largos, aunque su velocidad de carrera es menor para permitir la huida.

### 3. Dron de Vigilancia (`DroneScoutBehaviour`)
Unidad de apoyo aérea que actúa como multiplicador de fuerza y sensor de proximidad.
* **Detección Esférica:** Detecta en un radio de 360° (burbuja de proximidad) desde una altura de vuelo configurada.
* **Sistema de Alerta:** No ataca directamente. Al confirmar un objetivo, entra en modo `Tracking` y alerta a **todos los guardias cercanos** (armados y melee) en un radio de 30 metros.
* **Visuales:** Incluye una luz de patrulla que cambia de color dinámicamente según el nivel de alerta.

## Demo y Referencias Visuales

Puedes ver el comportamiento de estos enemigos y el sistema de alerta en funcionamiento en el siguiente video:

* **Video de Demostración:**
  
[![Mira el video aquí](https://img.youtube.com/vi/9gBn15mZ-0E/0.jpg)](https://www.youtube.com/watch?v=9gBn15mZ-0E)

> *Haz clic en la imagen para ver la demostración en YouTube.*

## Arquitectura Avanzada: Event & Data Driven System
En la última fase del desarrollo, hemos desacoplado las mecánicas principales utilizando un sistema de Eventos Globales y Scriptable Objects. Esto permite que los sistemas (UI, Jugador, Puertas) se comuniquen sin conocerse entre sí, optimizando el rendimiento y la escalabilidad.

### Características del Sistema de Eventos
- UIManager Reactivo: La interfaz ya no utiliza el método Update para consultar la salud o los documentos. Ahora, solo se actualiza cuando el EventManager dispara una notificación, ahorrando ciclos de CPU.
- Gestión de Inventario Dinámica: Se implementó una lógica de filtrado donde los objetos de misión (Files, Keycards) actualizan contadores de texto, mientras que los gadgets (USB, Phone, Medikit) se instancian visualmente en el panel de inventario.
- Puertas de Seguridad Inteligentes: Las puertas se suscriben al evento OnSecurityLevelChanged. Al detectar al jugador, validan automáticamente si su nivel de acceso es suficiente para bajar la compuerta.
- Habilidades Desbloqueables: El sistema de hackeo del jugador se activa mediante un evento al recoger el teléfono, desacoplando la lógica de recolección de la lógica de habilidades.

### Demo: Sistema de Inventario y Tercera Persona
Puedes ver la implementación del sistema de eventos, la nueva cámara en tercera persona y el HUD reactivo en el siguiente video:

Video de Arquitectura y Gameplay:

[![Mira el video aquí](https://img.youtube.com/vi/0Sxx0rLU_Bw/0.jpg)](https://youtu.be/0Sxx0rLU_Bw)
> *Haz clic en la imagen para ver la demostración en YouTube.*

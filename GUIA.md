# Guía: Instructor Station (C# / WPF)

Una estación de instructor para un avión simulado. Un motor de simulación calcula el estado del avión (altitud, velocidad, rumbo...). Desde una aplicación de escritorio, el instructor lo observa en vivo, mueve los mandos e inyecta fallos.

## Reglas del juego

1. **Primero el concepto y después el código.** Cada fase lista lo que tienes que entender antes de escribir. Léelo en Microsoft Learn (learn.microsoft.com/dotnet/csharp).
2. **Todo el código lo escribes tú.** Si te atascas, pide una *pista*, no la solución.
3. **Al terminar cada fase, haz un commit** (`feat: ...`) y pide una revisión del código.
4. **Si no sabes responder las preguntas de autoevaluación**, la fase no está terminada.
5. **Plan de recorte:** con las fases 0–5 terminadas ya tienes un proyecto presentable. Las fases 6 y 7 son extra.

## Arquitectura

```
InstructorStation.sln
├── src/
│   ├── FlightSim.Core/           # Dominio + simulación. Sin dependencias de UI.
│   ├── FlightSim.ConsoleRunner/  # Para probar el motor antes de tener UI
│   └── InstructorStation.Wpf/    # UI (MVVM). Depende de Core, nunca al revés.
└── tests/
    └── FlightSim.Core.Tests/     # xUnit
```

La regla de oro es que **las dependencias apuntan hacia el dominio**. `Core` no sabe que existe WPF. Esto es lo que te va a permitir testearlo, y además es una muy buena respuesta para la entrevista.

---

## Fase 0: Entorno (jueves noche, ~1 h)

**Instalar:** `winget install Microsoft.DotNet.SDK.10` y Visual Studio 2022 Community o Rider (para tener el diseñador de XAML).

**Tareas**
- Crea la solución y los 4 proyectos **con la CLI de `dotnet`**, no desde el IDE: `dotnet new sln`, `dotnet new classlib`, `dotnet new xunit`, `dotnet new console`, `dotnet new wpf`.
- Añádelos a la solución (`dotnet sln add`) y crea las referencias (`dotnet add reference`).
- `git init` y un `.gitignore` (`dotnet new gitignore`).

**Terminado cuando:** `dotnet build` y `dotnet test` pasan.

**Autoevaluación**
- ¿Qué diferencia hay entre `.sln` y `.csproj`?
- ¿Qué es el CLR? ¿Y el IL?
- ¿Qué significa que .NET 10 sea una versión LTS?

---

## Fase 1: Modelo de dominio (viernes)

**Conceptos:** `class` vs `struct` vs `record`, tipos por valor vs por referencia, propiedades (`get`/`init`), inmutabilidad, expresiones `with`, `enum`, nullable reference types.

**Tareas** (en `FlightSim.Core`)
- `AircraftState`: un `record` inmutable con altitud (ft), velocidad (kt), rumbo (grados), velocidad vertical (ft/min) y pitch y bank (grados).
- `ControlInputs`: throttle (0–1), pitch deseado y bank deseado.
- `SimulationStatus`: un enum con `Stopped`, `Running` y `Paused`.

**Terminado cuando:** puedes crear un estado nuevo a partir de otro con `with`, sin mutar el original.

**Autoevaluación**
- ¿Por qué el estado es inmutable? ¿Qué problema evita cuando lo leen dos hilos?
- ¿Qué pasa al comparar dos `record` con `==`? ¿Y dos `class`?
- ¿Cuándo usarías `record struct`?

---

## Fase 2: Modelo de vuelo + tests (viernes)

**Conceptos:** interfaces, funciones puras, `Math`, xUnit (`[Fact]`, `[Theory]`, `[InlineData]`), Arrange-Act-Assert.

**Tareas**
- `IFlightModel` con `AircraftState Step(AircraftState state, ControlInputs controls, TimeSpan dt)`.
- `SimpleFlightModel`, con física deliberadamente simple:
  - La velocidad tiende a `throttle * velocidadMáxima` y no cambia de golpe (interpolación).
  - Velocidad vertical ≈ velocidad × sin(pitch).
  - La altitud se integra con la velocidad vertical y nunca baja de 0.
  - La tasa de giro depende del bank, y el rumbo siempre queda en el rango [0, 360).
- **Escribe primero los tests** y después la implementación (TDD):
  - Con el rumbo en 359 y un giro a la derecha, el resultado queda cerca de 0–1, no en 360+.
  - Con throttle 0 la velocidad baja.
  - Con pitch negativo en el suelo, la altitud no baja de 0.
  - Un `[Theory]` con varios `dt`.

**Autoevaluación**
- ¿Para qué sirve programar contra `IFlightModel` y no contra la clase concreta?
- ¿Por qué es fácil testear una función pura?
- ¿Qué diferencia hay entre `[Fact]` y `[Theory]`?

---

## Fase 3: Motor de simulación (viernes noche / sábado mañana)

**Conceptos:** delegates, `event` y `EventHandler<T>`, `Task`, `async/await`, `PeriodicTimer`, `CancellationToken`, `lock`, `IDisposable`.

**Tareas**
- `SimulationEngine`:
  - Recibe un `IFlightModel` por constructor (esto ya es inyección de dependencias).
  - `Start()`, `Pause()`, `Resume()`, `Stop()` y `Reset()`.
  - Un loop de **paso fijo** a 20 Hz en una `Task` aparte, con `PeriodicTimer`.
  - `SetControls(ControlInputs)`, que se puede llamar desde otro hilo (protégelo).
  - Un evento `StateUpdated` que lanza cada tick con el nuevo estado.
  - Implementa `IDisposable` y cancela el loop al hacer dispose.
- En `ConsoleRunner`: arranca el motor, suscríbete al evento, imprime el estado y sube el throttle a los 3 s.

**Terminado cuando:** la consola muestra el avión acelerando y subiendo, y `Stop()` termina limpio, sin excepciones.

**Autoevaluación**
- ¿Qué diferencia hay entre un `delegate` y un `event`?
- ¿Qué hace realmente `await`? ¿Se crea un hilo nuevo?
- ¿Por qué `.Result` o `.Wait()` pueden causar un deadlock en una app con UI?
- ¿Por qué un paso fijo y no "lo que tarde el frame"?
- ¿Qué pasa si no te desuscribes de un evento? (pista: memory leak)

---

## Fase 4: Fallos (sábado mañana)

**Conceptos:** polimorfismo, clases abstractas vs interfaces, principio Open/Closed, `List<T>`, LINQ (`Where`, `Select`, `Any`, `Aggregate`).

**Tareas**
- `IFailure` con `Name` y un método que transforma el estado o los controles.
- Implementa al menos tres:
  - `EngineFailure`: el throttle efectivo pasa a 0.
  - `PitotFailure`: la velocidad *indicada* se congela.
  - `StuckRudder` u otro que se te ocurra.
- Distingue el **estado real** del **estado indicado**: los fallos de sensor afectan solo a lo que ve el piloto. Pensar en esto te va a llevar un rato, y es justamente lo interesante del proyecto.
- El motor permite `InjectFailure` y `ClearFailure`, y aplica los fallos activos en cada tick.
- Escribe tests para cada fallo.

**Autoevaluación**
- Si mañana te piden un fallo nuevo, ¿qué clases tocas? (lo ideal es que la respuesta sea "ninguna, solo creo una nueva")
- ¿Interfaz o clase abstracta? ¿Cuándo elegirías cada una?
- ¿Qué es la ejecución diferida en LINQ? ¿Cuándo se ejecuta realmente un `Where`?

---

## Fase 5: UI WPF con MVVM (sábado)

**Conceptos:** XAML, `DataContext`, data binding, `INotifyPropertyChanged`, `ICommand`, el hilo de UI y `Dispatcher`, `ObservableCollection<T>`.

**Tareas**
- Escribe a mano una `ViewModelBase` (con `INotifyPropertyChanged`) y un `RelayCommand` (con `ICommand`). Nada de librerías MVVM: la idea es entender qué hay debajo.
- `MainViewModel` con estos paneles:
  - **Telemetría:** altitud, velocidad, rumbo y velocidad vertical, actualizados en vivo.
  - **Mandos:** sliders de throttle, pitch y bank enlazados a `SetControls`.
  - **Fallos:** checkboxes o botones para inyectar y limpiar fallos.
  - **Simulación:** Start, Pause y Reset, deshabilitados según el estado (`CanExecute`).
  - **Log de eventos:** una lista con hora y evento ("14:02:11 – Engine failure injected"), en una `ObservableCollection`.
- El evento `StateUpdated` llega desde el hilo de la simulación. Pásalo al hilo de UI con `Dispatcher`. **Primero hazlo mal a propósito** para ver la excepción.
- El code-behind (`MainWindow.xaml.cs`) tiene que quedar prácticamente vacío.

**Terminado cuando:** mueves el throttle, ves subir la velocidad, inyectas un fallo de motor y ves al avión perder velocidad, todo con la UI fluida.

**Autoevaluación**
- ¿Qué problema resuelve MVVM? ¿Qué testearías del ViewModel?
- ¿Por qué no puedes tocar la UI desde otro hilo?
- ¿Cómo se entera WPF de que una propiedad cambió?

---

## Fase 6: DI + escenarios (sábado noche, extra)

**Conceptos:** inyección de dependencias, contenedor, lifetimes (Singleton, Scoped, Transient), `System.Text.Json`.

**Tareas**
- Configura `Microsoft.Extensions.DependencyInjection` en `App.xaml.cs` y registra el motor, el modelo de vuelo y el ViewModel.
- Guarda y carga **escenarios** en JSON (estado inicial más fallos programados a los X segundos), igual que prepara una clase un instructor real.

**Autoevaluación**
- ¿Qué lifetime le das al `SimulationEngine` y por qué?
- ¿Qué pasa si un Singleton depende de un Scoped?

---

## Fase 7: Portfolio (domingo mañana)

- Un README con la descripción, un diagrama de arquitectura, capturas o un GIF, cómo ejecutarlo, **las decisiones técnicas y por qué las tomaste**, y qué mejorarías.
- Súbelo a GitHub con un historial de commits limpio.
- Prepara un pitch de 2 minutos: qué es, la arquitectura y la decisión técnica más interesante (estado real vs indicado, paso fijo, hilos).

## Domingo tarde: repaso

Vuelve a las preguntas de autoevaluación de todas las fases y respóndelas en voz alta, sin mirar.

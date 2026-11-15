# Instructor Station

A small flight simulator **instructor station** built with C# and WPF. A simplified flight model runs at a fixed 20 Hz step, and an instructor watches live telemetry, moves the controls and injects an engine failure.

> Built as a learning project to practise C#/.NET fundamentals, test-driven development and MVVM.

## Features

- Live telemetry: altitude, airspeed, heading and vertical speed
- Instructor controls: throttle, pitch and bank
- Simulation control: Start / Pause / Reset (buttons enable/disable automatically)
- Failure injection: engine failure

## Architecture

```
InstructorStation.slnx
├── src/
│   ├── FlightSim.Core/           # Domain + flight model. No UI dependencies.
│   ├── FlightSim.ConsoleRunner/  # Console sandbox for the domain model
│   └── InstructorStation.Wpf/    # WPF UI (MVVM)
└── tests/
    └── FlightSim.Core.Tests/     # xUnit tests for the flight model
```

Dependencies point **towards the domain**: the UI and tests reference `FlightSim.Core`, and the core references nothing. The flight model can therefore be tested in isolation and replaced without touching the UI.

## Design decisions

- **Immutable state (`record`)**: `AircraftState` is never mutated. Each simulation step returns a new state using `with` expressions. A reader always sees a complete, consistent snapshot (never half of one tick and half of the next), and the UI only needs one change notification per tick.
- **Programming against an interface**: the ViewModel depends on `IFlightModel`, not on `SimpleFlightModel`. A more realistic model can be plugged in without changes elsewhere.
- **Pure, testable flight model**: `Step(state, controls, dt)` has no side effects, so it is easy to test.
- **Named constants** instead of magic numbers (`TurnRatePerBankDeg`, `AirspeedResponseRate`, `FpmPerKnot`…).
- **Hand-written MVVM infrastructure** (`ViewModelBase`, `RelayCommand`) instead of a library, to understand what `INotifyPropertyChanged` and `ICommand` do underneath.

## Flight model (simplified)

| Quantity | Rule |
|---|---|
| Heading | Turn rate = bank × 0.1 °/s, normalized to [0, 360) |
| Airspeed | Converges to `throttle × 300 kt` at 10 % of the remaining difference per second |
| Vertical speed | `airspeed × 101.27 × sin(pitch)` (knots → ft/min) |
| Altitude | Integrated from vertical speed, never below 0 |
| Engine failure | Effective throttle forced to 0 |

## Tests

Written test-first (red → green → refactor). Examples of bugs caught by the tests:

- **Negative modulo**: in C#, `-1 % 360 == -1`, so turning left past north produced a heading of -1° instead of 359°.
- **Aircraft below ground**: once altitude was integrated, the "stays on the ground" edge-case test caught the aircraft going to -29 ft.

```powershell
dotnet test
```

## Running

Requires the .NET SDK (11) on Windows.

```powershell
dotnet run --project src/InstructorStation.Wpf
```

## Known simplifications and next steps

- **Simulation loop on the UI thread**: a `DispatcherTimer` drives the simulation, which is fine for a lightweight model. A heavier model should run on a background loop (`PeriodicTimer` + `CancellationToken`) and marshal states to the UI through the `Dispatcher`.
- **Attitude is set directly**: `ControlInputs` has pitch and bank fields that the model does not use yet. The instructor sets the attitude on the state directly.
- **Failures**: a polymorphic `IFailure` design (sensor failures that affect *indicated* values but not *real* ones, such as a frozen pitot tube).
- **Dependency injection** with `Microsoft.Extensions.DependencyInjection`.
- **Scenarios**: save and load initial states and scheduled failures as JSON.
- **ViewModel tests**.

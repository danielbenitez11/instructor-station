using FlightSim.Core;

AircraftState state = new AircraftState
{
    AltitudeFt = 10000,
    AirspeedKt = 250,
    HeadingDeg = 90,
    VerticalSpeedFpm = 500,
    PitchDeg = 5,
    BankDeg = 10
};

AircraftState newState = state with { AltitudeFt = 12000};

Console.WriteLine($"Aircraft state - Old Altitude: {state.AltitudeFt} ft, New Altitude: {newState.AltitudeFt} ft, Airspeed: {newState.AirspeedKt} kt, " +
    $"Heading: {newState.HeadingDeg} deg, Vertical Speed: {newState.VerticalSpeedFpm} fpm, Pitch: {newState.PitchDeg} deg, Bank: {newState.BankDeg} deg");

Console.WriteLine(state);
Console.WriteLine(newState);


ControlInputs inputs = new ControlInputs
{
    Throttle = 0.8,
    PitchDeg = 2,
    BankDeg = 5
};

Console.WriteLine(inputs);

namespace FlightSim.Core
{
    public record AircraftState
    {
        public double AltitudeFt { get; init; }
        public double AirspeedKt { get; init; }
        public double HeadingDeg { get; init; }
        public double VerticalSpeedFpm { get; init; }
        public double PitchDeg { get; init; }
        public double BankDeg { get; init; }
        public bool EngineFailed { get; init; }
    }
}

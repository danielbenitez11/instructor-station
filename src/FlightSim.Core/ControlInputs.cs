namespace FlightSim.Core
{
    public record ControlInputs
    {
        /// <summary> Engine power, from 0 (idle) to 1 (full) </summary>
        public double Throttle { get; init; }
        public double PitchDeg { get; init; }
        public double BankDeg { get; init; }
    }
}

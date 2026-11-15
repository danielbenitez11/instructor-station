using System;
using System.Collections.Generic;
using System.Text;

namespace FlightSim.Core
{
    public class SimpleFlightModel : IFlightModel
    {
        public AircraftState Step(AircraftState state, ControlInputs controls, TimeSpan dt)
        {
            double seconds = dt.TotalSeconds;

            double turnRate = state.BankDeg * 0.1;

            double rawHeading = state.HeadingDeg + turnRate * seconds;

            double targetAirspeedKt = controls.Throttle * 300;

            double newAirspeedKt = state.AirspeedKt + (targetAirspeedKt - state.AirspeedKt) * 0.1 * seconds;

            return state with { HeadingDeg = NormalizeHeading(rawHeading), AirspeedKt = newAirspeedKt };
        }
        

        private static double NormalizeHeading(double heading)
        {
            return (heading + 360) % 360;
        }
    
    
    
    
    }

}

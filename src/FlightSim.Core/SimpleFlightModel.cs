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

            double newHeading = (state.HeadingDeg + turnRate * seconds) % 360;

            return state with { HeadingDeg = newHeading };
        }
    }
}

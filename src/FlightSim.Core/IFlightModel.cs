using System;
using System.Collections.Generic;
using System.Text;

namespace FlightSim.Core
{
    public interface IFlightModel
    {
        AircraftState Step (AircraftState state, ControlInputs controls, TimeSpan dt);


    }
}

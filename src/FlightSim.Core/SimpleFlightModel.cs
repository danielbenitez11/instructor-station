using System;
using System.Collections.Generic;
using System.Text;

namespace FlightSim.Core
{

    public class SimpleFlightModel : IFlightModel
    {

        private const double MaxAirspeedKt = 300;
        private const double TurnRatePerBankDeg = 0.1;

        private const double AirspeedResponseRate = 0.1;

        public AircraftState Step(AircraftState state, ControlInputs controls, TimeSpan dt)
        {
            double seconds = dt.TotalSeconds;

            double turnRate = state.BankDeg * TurnRatePerBankDeg;

            double rawHeading = state.HeadingDeg + turnRate * seconds;

            double targetAirspeedKt = controls.Throttle * MaxAirspeedKt;

            double newAirspeedKt = state.AirspeedKt + (targetAirspeedKt - state.AirspeedKt) * AirspeedResponseRate * seconds;

            return state with { HeadingDeg = NormalizeHeading(rawHeading), AirspeedKt = newAirspeedKt };
        }
        

        private static double NormalizeHeading(double heading)
        {
            return (heading + 360) % 360;
        }
    
    
    
    
    }

}

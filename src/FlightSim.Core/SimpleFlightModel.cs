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

        private const double FpmPerKnot = 101.27;

        public AircraftState Step(AircraftState state, ControlInputs controls, TimeSpan dt)
        {
            double seconds = dt.TotalSeconds;

            double turnRate = state.BankDeg * TurnRatePerBankDeg;

            double rawHeading = state.HeadingDeg + turnRate * seconds;

            double effectiveThrottle = state.EngineFailed ? 0 : controls.Throttle;

            double targetAirspeedKt = effectiveThrottle * MaxAirspeedKt;

            double newAirspeedKt = state.AirspeedKt + (targetAirspeedKt - state.AirspeedKt) * AirspeedResponseRate * seconds;

            double newVerticalSpeedFpm = state.AirspeedKt * FpmPerKnot * Math.Sin(DegreesToRadians(state.PitchDeg));

            double newAltitudeFt = Math.Max(0, state.AltitudeFt + newVerticalSpeedFpm * seconds / 60);

            


            return state with 
            {
                HeadingDeg = NormalizeHeading(rawHeading), 
                AirspeedKt = newAirspeedKt, 
                AltitudeFt = newAltitudeFt, 
                VerticalSpeedFpm = newVerticalSpeedFpm
            };
        }
        

        private static double NormalizeHeading(double heading)
        {
            return (heading + 360) % 360;
        }


        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }




    }

}

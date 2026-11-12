using System;
using System.Collections.Generic;
using System.Text;

namespace FlightSim.Core.Tests
{
    public class SimpleFlightModelTests
    {
        [Fact]
        public void Step_WhenHeadingCrosses360_WrapsAroundToZero()
        {
            // Arrange
            var model = new SimpleFlightModel();
            var initialState = new AircraftState
            {
                AltitudeFt = 10000,
                AirspeedKt = 250,
                HeadingDeg = 359,
                VerticalSpeedFpm = 0,
                PitchDeg = 0,
                BankDeg = 20
            };

            var controls = new ControlInputs
            {
                Throttle = 0,
                PitchDeg = 0,
                BankDeg = 0
            };

            var dt = TimeSpan.FromSeconds(1);

            // Act
            var newState = model.Step(initialState, controls, dt);


            // Assert
            Assert.Equal(1, newState.HeadingDeg, precision:3);
        }

    }
}

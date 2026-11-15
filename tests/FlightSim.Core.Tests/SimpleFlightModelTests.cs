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

        [Fact]
        public void Step_WhenHeadingCrosses0_WrapsAroundTo360()
        {
            // Arrange
            var model = new SimpleFlightModel();
            var initialState = new AircraftState
            {
                AltitudeFt = 10000,
                AirspeedKt = 250,
                HeadingDeg = 1,
                VerticalSpeedFpm = 0,
                PitchDeg = 0,
                BankDeg = -20
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
            Assert.Equal(359, newState.HeadingDeg, precision: 3);
        }

        [Fact]
        public void Step_WhenThrottleIsZero_AirspeedDecreases()
        {
            // Arrange
            var model = new SimpleFlightModel();
            var initialState = new AircraftState
            {
                AltitudeFt = 10000,
                AirspeedKt = 250,
                HeadingDeg = 90,
                VerticalSpeedFpm = 0,
                PitchDeg = 0,
                BankDeg = 0
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
            Assert.Equal(225, newState.AirspeedKt, precision: 3);
        }

        [Fact]
        public void Step_WhenOnGroundAndPitchDown_AltitudeStaysAtZero()
        {
            // Arrange
            var model = new SimpleFlightModel();
            var initialState = new AircraftState
            {
                AltitudeFt = 0,
                AirspeedKt = 100,
                HeadingDeg = 90,
                VerticalSpeedFpm = 0,
                PitchDeg = -10,
                BankDeg = 0
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
            Assert.Equal(0, newState.AltitudeFt, precision: 3);
        }

        [Fact]
        public void Step_WhenPitchUp_AltitudeIncreases()
        {
            // Arrange
            var model = new SimpleFlightModel();
            var initialState = new AircraftState
            {
                AltitudeFt = 1000,
                AirspeedKt = 100,
                HeadingDeg = 90,
                VerticalSpeedFpm = 0,
                PitchDeg = 10,
                BankDeg = 0

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
            Assert.True(newState.AltitudeFt > 1000);
        }

        [Fact]
        public void Step_WhenEngineFailed_AirspeedDecreases()
        {
            // Arrange
            var model = new SimpleFlightModel();
            var initialState = new AircraftState
            {
                AltitudeFt = 10000,
                AirspeedKt = 250,
                HeadingDeg = 90,
                VerticalSpeedFpm = 0,
                PitchDeg = 0,
                BankDeg = 0,
                EngineFailed = true
            };
            var controls = new ControlInputs
            {
                Throttle = 1, 
                PitchDeg = 0,
                BankDeg = 0
            };
            var dt = TimeSpan.FromSeconds(1);
            // Act
            var newState = model.Step(initialState, controls, dt);
            // Assert
            Assert.Equal(225, newState.AirspeedKt, precision: 3);
        }

    }
}

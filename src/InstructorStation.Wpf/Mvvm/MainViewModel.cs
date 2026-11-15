using FlightSim.Core;
using System.Windows.Threading;

namespace InstructorStation.Wpf.Mvvm
{
    internal class MainViewModel : ViewModelBase
    {
        private AircraftState _state = new AircraftState { AltitudeFt = 5000, AirspeedKt = 200 };
        private readonly IFlightModel _flightModel = new SimpleFlightModel();
        private static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(50);
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public MainViewModel()
        {
            _timer.Interval = TickInterval;
            _timer.Tick += OnTick;
            _timer.Start();
        }


        public AircraftState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;

                    OnPropertyChanged();
                }
            }
        }

        private void OnTick(object? sender, EventArgs e)
        {
            State = _flightModel.Step(State, new ControlInputs(), TickInterval);
        }
    }
}

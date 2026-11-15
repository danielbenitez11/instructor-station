using FlightSim.Core;
using System.Windows.Threading;

namespace InstructorStation.Wpf.Mvvm
{
    internal class MainViewModel : ViewModelBase
    {
        private static readonly AircraftState InitialState = new AircraftState { AltitudeFt = 5000, AirspeedKt = 200 };

        private AircraftState _state = InitialState;

        private double _throttle = 0;
        private readonly IFlightModel _flightModel = new SimpleFlightModel();
        private static readonly TimeSpan TickInterval = TimeSpan.FromMilliseconds(50);
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public RelayCommand StartCommand { get; }
        public RelayCommand PauseCommand { get; }
        public RelayCommand ResetCommand { get; }


        public MainViewModel()
        {
            _timer.Interval = TickInterval;
            _timer.Tick += OnTick;
            StartCommand = new RelayCommand(Start, () => !_timer.IsEnabled);
            PauseCommand = new RelayCommand(Pause, () => _timer.IsEnabled);
            ResetCommand = new RelayCommand(Reset);
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
                    OnPropertyChanged(nameof(PitchDeg));
                    OnPropertyChanged(nameof(BankDeg));
                    OnPropertyChanged(nameof(EngineFailed));
                }
            }
        }

        public double Throttle
        {
            get => _throttle;
            set
            {
                if (_throttle != value)
                {
                    _throttle = value;
                    OnPropertyChanged();
                }
            }
        }



        public double PitchDeg
        {
            get => State.PitchDeg;
            set => State = State with { PitchDeg = value };
        }

        public double BankDeg
        {
            get =>  State.BankDeg;
            set => State = State with { BankDeg = value };
        }

        public bool EngineFailed
        {
            get => State.EngineFailed;
            set => State = State with { EngineFailed = value };
        }

        private void OnTick(object? sender, EventArgs e)
        {
            State = _flightModel.Step(State, new ControlInputs { Throttle = _throttle }, TickInterval);
        }

        private void Start()
        {
            _timer.Start();
            StartCommand.RaiseCanExecuteChanged();
            PauseCommand.RaiseCanExecuteChanged();
        }

        private void Pause()
        {
            _timer.Stop();
            StartCommand.RaiseCanExecuteChanged();
            PauseCommand.RaiseCanExecuteChanged();
        }

        private void Reset()
        {
            State = InitialState;
            Throttle = 0;
        }
    }
}

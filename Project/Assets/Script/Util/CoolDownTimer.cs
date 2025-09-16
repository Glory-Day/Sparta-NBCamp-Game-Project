using System;
using System.Threading;
using System.Threading.Tasks;
using Backend.Util.Debug;

namespace Backend.Util
{
    public class CoolDownTimer
    {
        private CancellationTokenSource _source;

        private readonly TimeSpan _time;

        public CoolDownTimer(float time)
        {
            _time = TimeSpan.FromSeconds(time);
        }

        public void Start()
        {
            if (IsCoolingDown)
            {
                return;
            }

            IsCoolingDown = true;

            _source = new CancellationTokenSource();

            Task.Run(
                async () =>
                {
                    try
                    {
                        await Task.Delay(_time, _source.Token);
                    }
                    catch (TaskCanceledException exception)
                    {
                        Debugger.LogError(exception.Message);
                    }
                });

            IsCoolingDown = false;
        }

        public void Stop()
        {
            if (_source != null && _source.IsCancellationRequested)
            {
                _source.Cancel();

                IsCoolingDown = false;
            }
        }

        public bool IsCoolingDown { get; private set; }
    }
}

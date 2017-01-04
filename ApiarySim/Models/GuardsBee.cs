using System;
using ApiarySim.Helpers;
using ApiarySim.ViewModels;

namespace ApiarySim.Models
{
    public class GuardsBee : Bee
    {
        private readonly Random _random = new Random();
        private readonly int _timerCheck;
        private WorkerBee _hiveWorker;

        public GuardsBee()
        {
            _timerCheck = App.Random.Next(DataForCreate.Instance.MinGuardCheckTime, DataForCreate.Instance.MaxGuardCheckTime);
        }

        public CheckState Check()
        {
            if (TimerLive < _timerCheck)
                return CheckState.Wait;

            if (_hiveWorker.CountTry > _hiveWorker.Max)
            {
                _hiveWorker.CountTry = 0;
                return CheckState.Done;
            }
            _hiveWorker.CountTry++;
            var result = _random.Next(0, 1000) >500;
            if (result)
                return CheckState.Done;
            return CheckState.None;
        }

        public void Done(Hive hive)
        {
            hive.Honey++;
            hive.InsidesCount++;
            hive.WorkersCount--;
            hive.BusyGuards.Remove(this);
            hive.AvaliableGuards.Add(this);
            _hiveWorker.BeeState = BeeState.In;
            _hiveWorker = null;
            TimerLive = 0;
        }

        public void None(Hive hive)
        {
            hive.BusyGuards.Remove(this);
            hive.AvaliableGuards.Add(this);
            _hiveWorker.BeeState = BeeState.Out;
            _hiveWorker = null;
            TimerLive = 0;
        }

        public void WaitWorker(WorkerBee hiveWorker)
        {
            _hiveWorker = hiveWorker;
            hiveWorker.BeeState = BeeState.Checking;
        }
    }
}
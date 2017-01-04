using System;
using ApiarySim.Helpers;
using ApiarySim.ViewModels;

namespace ApiarySim.Models
{
    public class WorkerBee : Bee
    {
        public WorkerBee()
        {
            TimeInside = App.Random.Next(DataForCreate.Instance.MinWorkerInsideTime, DataForCreate.Instance.MaxWorkerInsideTime);
            TimeHoney = App.Random.Next(DataForCreate.Instance.MinWorkerHoneyTime, DataForCreate.Instance.MaxWorkerHoneyTime);
            Max = DataForCreate.Instance.MaxWorkerTry;
        }

        public int CountTry { get; set; }
        public int Max { get; set; }
        public int TimeHoney { get; set; }

        public int TimeInside { get; set; }

        public void DoWork(Hive hive, ref int newWorker, ref int newInside)
        {
            switch (BeeState)
            {
                //надо вылететь
                case BeeState.In:
                    if (TryFly())
                    {
                        hive.WorkerOut(this);
                        newInside--;
                        newWorker++;
                    }
                    break;
                //залетаем на проверку или собираем мед
                case BeeState.Out:
                    if (TryGotoCheck())
                    {
                        BeeState = BeeState.NeedCheck;
                    }
                    break;
                //ждем проверку
                case BeeState.NeedCheck:
                    hive.TryInside(this);
                    break;

                case BeeState.Checking:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool TryFly()
        {
            if (TimerLive >= TimeInside)
            {
                TimerLive = 0;
                return true;
            }
            return false;
        }

        public bool TryGotoCheck()
        {
            if (TimerLive >= TimeHoney)
            {
                TimerLive = 0;
                return true;
            }

            return false;
        }

        public void WaitCheck()
        {
        }
    }
}
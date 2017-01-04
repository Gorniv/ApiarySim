using ApiarySim.Helpers;
using ApiarySim.Models.Queen;
using ApiarySim.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Newtonsoft.Json;

namespace ApiarySim.Models
{
    public class Hive : BaseViewModel
    {
        private readonly ReaderWriterLockSlim _rw = new ReaderWriterLockSlim();

        public Hive()
        {
            Number = KeyProvader.GetHive();
            Workers = new List<WorkerBee>(GenerateBee<WorkerBee>.GetBees(DataForCreate.Instance.MinWorkers, DataForCreate.Instance.MaxWorkers));
            WorkersCount = Workers.Count(c => c.BeeState == BeeState.Out);
            InsidesCount = Workers.Count(c => c.BeeState == BeeState.In);
            Guards = new ObservableCollection<GuardsBee>(GenerateBee<GuardsBee>.GetBees(DataForCreate.Instance.MinGuards, DataForCreate.Instance.MaxGuards));
            AvaliableGuards = Guards.ToList();
            BusyGuards = new List<GuardsBee>();
            Queens = new ObservableCollection<QueenBee>(new[] { new QueenBee() });
        }

        public Hive(SimpleHive simpleHive)
        {
            Number = simpleHive.Number;
            Workers = new List<WorkerBee>(GenerateBee<WorkerBee>.GetBees(simpleHive.Workers, simpleHive.Workers));
            var insides = new List<WorkerBee>(GenerateBee<WorkerBee>.GetBees(simpleHive.Insides, simpleHive.Insides));
            insides.ForEach(c => c.BeeState = BeeState.Out);
            Workers.AddRange(insides);
            WorkersCount = Workers.Count(c => c.BeeState == BeeState.Out);
            InsidesCount = Workers.Count(c => c.BeeState == BeeState.In);
            Guards = new ObservableCollection<GuardsBee>(GenerateBee<GuardsBee>.GetBees(simpleHive.Guards, simpleHive.Guards));
            AvaliableGuards = Guards.ToList();
            BusyGuards = new List<GuardsBee>();
            Queens = new ObservableCollection<QueenBee>(new[] { new QueenBee() });
            Honey = simpleHive.Honey;
        }

        [JsonIgnore]
        public List<GuardsBee> AvaliableGuards { get; set; }
        [JsonIgnore]
        public List<GuardsBee> BusyGuards { get; set; }
        public ObservableCollection<GuardsBee> Guards { get; set; }
        public int Honey { get; set; }
        [JsonIgnore]
        public int InsidesCount { get; set; }
        public int Number { get; set; }
        public ObservableCollection<QueenBee> Queens { get; set; }
        [JsonIgnore]
        public int TotalBees => WorkersCount + InsidesCount;
        public List<WorkerBee> Workers { get; set; }
        [JsonIgnore]
        public int WorkersCount { get; set; }

        public void AddWorker(WorkerBee workerBee)
        {
            _rw.EnterWriteLock();
            Workers.Add(workerBee);
            InsidesCount++;
            _rw.ExitWriteLock();
        }

        public IEnumerable<WorkerBee> GetWorker()
        {
            _rw.EnterReadLock();
            var workers = Workers.ToList();
            _rw.ExitReadLock();
            return workers;
        }

        public void OnPropertyHoney()
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low,
                () =>
                {
                    OnPropertyChanged(nameof(Honey));
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public void TickSim()
        {
            var needRefresh = false;
            foreach (var queen in Queens.ToList())
            {
                queen.Live();
                needRefresh = needRefresh || queen.QueenLive.DoLive(queen, this);
            }

            var newWorker = 0;
            var newInside = 0;
            var busyGuard = BusyGuards.ToList();
            foreach (var guard in busyGuard)
            {
                guard.Live();
                var result = guard.Check();
                if (result == CheckState.Done)
                {
                    newInside++;
                    newWorker--;
                    guard.Done(this);
                }
                if (result == CheckState.None)
                {
                    guard.None(this);
                }
            }

            var workers = GetWorker();
            foreach (var bee in workers)
            {
                bee.Live();
                bee.DoWork(this, ref newWorker, ref newInside);
            }
            if (newWorker != newInside && (newInside != 0 || newWorker != 0) || needRefresh)
            {
                UpdateProperty();
            }
        }

        public bool TryInside(WorkerBee hiveWorker)
        {
            var guard = AvaliableGuards.FirstOrDefault();
            if (guard != null)
            {
                AvaliableGuards.Remove(guard);
                BusyGuards.Add(guard);
                guard.WaitWorker(hiveWorker);
                return true;
            }
            hiveWorker.WaitCheck();
            return false;
        }

        public void UpdateProperty()
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low,
                () =>
                {
                    OnPropertyChanged(nameof(TotalBees));
                    OnPropertyChanged(nameof(WorkersCount));
                    OnPropertyChanged(nameof(InsidesCount));
                    OnPropertyChanged(nameof(Honey));
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public void WorkerOut(WorkerBee hiveWorker)
        {
            _rw.EnterWriteLock();
            InsidesCount--;
            WorkersCount++;
            hiveWorker.BeeState = BeeState.Out;
            _rw.ExitWriteLock();
        }
    }
}
using ApiarySim.Helpers;
using ApiarySim.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Core;
using static Windows.ApplicationModel.Core.CoreApplication;

namespace ApiarySim.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly List<Hive> _forRemove;
        private readonly StorageFolder _localCacheFolder;
        private int _collectedHoney;
        private ObservableCollection<Hive> _hives;
        private bool _isInit;
        private bool _isSim;
        private StorageFolder _localConfigFolder;
        private bool _needAdd;
        private bool _needCollect;
        private bool _needRemove;
        private Hive _selectedHive;
        private CancellationTokenSource _token;
        private int _totalBees;
        private int _totalHiveHoney;
        private int _totalHives;

        public MainViewModel()
        {
            _localCacheFolder = ApplicationData.Current.LocalFolder;
            _localConfigFolder = ApplicationData.Current.LocalFolder;
            LoadConfig();
            StartCommand = new DelegateCommand(ExecuteStart, CanExecuteStart);
            StopCommand = new DelegateCommand(ExecuteStop, CanExecuteStop);
            CollectCommand = new DelegateCommand(ExecuteCollect, CanExecuteCollect);
            AddCommand = new DelegateCommand(ExecuteAdd, CanExecuteAdd);
            RemoveCommand = new DelegateCommand(ExecuteRemove, CanExecuteRemove);
            Init();
            _forRemove = new List<Hive>();
        }

        public ICommand AddCommand { get; set; }

        public ICommand CollectCommand { get; set; }

        public int CollectedHoney
        {
            get { return _collectedHoney; }
            set
            {
                _collectedHoney = value;
                OnPropertyChanged(nameof(CollectedHoney));
            }
        }

        public ObservableCollection<Hive> Hives
        {
            get { return _hives; }
            set
            {
                _hives = value;
                TotalHives = value?.Count ?? 0;
                OnPropertyChanged(nameof(Hives));
            }
        }

        public int LiveTime { get; set; }

        public ICommand RemoveCommand { get; set; }

        public Hive SelectedHive
        {
            get { return _selectedHive; }
            set
            {
                _selectedHive = value;
                OnPropertyChanged(nameof(SelectedHive));
                OnPropertyChanged(nameof(RemoveCommand));
            }
        }

        public ICommand StartCommand { get; set; }

        public ICommand StopCommand { get; set; }

        public int TotalBees
        {
            get { return _totalBees; }
            set
            {
                _totalBees = value;
                OnPropertyChanged(nameof(TotalBees));
            }
        }

        public int TotalHiveHoney
        {
            get { return _totalHiveHoney; }
            set
            {
                _totalHiveHoney = value;
                OnPropertyChanged(nameof(TotalHiveHoney));
                OnPropertyChanged(nameof(CollectCommand));
            }
        }

        public int TotalHives
        {
            get { return _totalHives; }
            set
            {
                _totalHives = value;
                OnPropertyChanged(nameof(TotalHives));
            }
        }

        private void AddOpertaion()
        {
            Hives.Add(new Hive());
            var temp = Hives?.Count ?? 0;
            TotalHives = temp;
            _needAdd = false;
        }

        private bool CanExecuteAdd(object arg)
        {
            return _isInit;
        }

        private bool CanExecuteCollect(object arg)
        {
            return TotalHiveHoney > 0;
        }

        private bool CanExecuteRemove(object arg)
        {
            return SelectedHive != null;
        }

        private bool CanExecuteStart(object arg)
        {
            return !_isSim;
        }

        private bool CanExecuteStop(object arg)
        {
            return _isSim;
        }

        private void CollectOpertaion()
        {
            var temp = TotalHiveHoney;
            foreach (var hive in Hives)
            {
                hive.Honey = 0;
                hive.OnPropertyHoney();
            }
            TotalHiveHoney = 0;
            CollectedHoney += temp;
            _needCollect = false;
        }

        private void ExecuteAdd(object obj)
        {
            if (_isSim)
            {
                _needAdd = true;
            }
            else
            {
                AddOpertaion();
                UpdateMainWindow();
            }
        }

        private void ExecuteCollect(object arg)
        {
            if (_isSim)
            {
                _needCollect = true;
            }
            else
            {
                CollectOpertaion();
            }
        }

        private void ExecuteRemove(object obj)
        {
            if (_forRemove.Contains(SelectedHive))
                return;

            _forRemove.Add(SelectedHive);
            if (_isSim)
            {
                _needRemove = true;
            }
            else
            {
                RemoveOpertaion();
                UpdateMainWindow();
            }
        }

        private void ExecuteStart(object obj)
        {
            _isSim = true;
            Task.Factory.StartNew(StartOperation);
            OnPropertyChanged(nameof(StartCommand));
            OnPropertyChanged(nameof(StopCommand));
        }

        private void ExecuteStop(object obj)
        {
            _token.Cancel();
            _isSim = false;
            OnPropertyChanged(nameof(StartCommand));
            OnPropertyChanged(nameof(StopCommand));
            Task.Factory.StartNew(Save);
        }

        private void Init()
        {
            if (!DataForCreate.Instance.LoadFromCache || !LoadFromCache())
            {
                var hives = new List<Hive>();
                for (var i = 0; i < DataForCreate.Instance.HivesCount; i++)
                {
                    hives.Add(new Hive());
                }
                Hives = new ObservableCollection<Hive>(hives);
            }

            UpdateMainWindow();
            _isInit = true;
        }

        private bool LoadConfig()
        {
            try
            {
                //Read the first line of dataFile.txt in LocalCacheFolder and store it in a String
                var dataFile = StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{DataForCreate.Instance.ConfigFileName}")).GetAwaiter().GetResult();
                var serializationData = FileIO.ReadTextAsync(dataFile).GetAwaiter().GetResult();
                var config = JsonConvert.DeserializeObject<DataForCreate>(serializationData);
                DataForCreate.Instance = config;
                return true;
            }
            catch (Exception)
            {
                // ignored
            }
            return false;
        }

        private bool LoadFromCache()
        {
            try
            {
                //Read the first line of dataFile.txt in LocalCacheFolder and store it in a String
                var dataFile = _localCacheFolder.GetFileAsync(DataForCreate.Instance.DataFileName).GetAwaiter().GetResult();
                var serializationData = FileIO.ReadTextAsync(dataFile).GetAwaiter().GetResult();
                var cacheApiary = JsonConvert.DeserializeObject<CacheApiary>(serializationData);
                var hives = new List<Hive>();
                foreach (var listHivesHive in cacheApiary.Hives)
                {
                    hives.Add(new Hive(listHivesHive));
                }
                Hives = new ObservableCollection<Hive>(hives);
                CollectedHoney = cacheApiary.CollectedHoney;
                LiveTime = cacheApiary.LiveTime;
                return true;
            }
            catch (Exception)
            {
                // ignored
            }
            return false;
        }

        private void RemoveOpertaion()
        {
            foreach (var hive in _forRemove.ToList())
            {
                Hives.Remove(hive);
                if (SelectedHive == hive)
                    SelectedHive = null;
            }
            var temp = Hives?.Count ?? 0;
            TotalHives = temp;
            _needRemove = false;
        }

        private void Save()
        {
            try
            {
                var sampleFile = _localCacheFolder.CreateFileAsync(DataForCreate.Instance.DataFileName,
                        CreationCollisionOption.ReplaceExisting).GetAwaiter().GetResult();
                var result = JsonConvert.SerializeObject(new CacheApiary(Hives, CollectedHoney, LiveTime));
                FileIO.WriteTextAsync(sampleFile, result).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async Task StartOperation()
        {
            _token = new CancellationTokenSource();
#pragma warning disable 4014
            Task.Factory.StartNew(WorkerUi);
#pragma warning restore 4014
            while (true)
            {
                if (_token.IsCancellationRequested)
                {
                    return;
                }
                if (_needAdd)
                {
                    await MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, AddOpertaion);
                    _needAdd = false;
                }
                if (_needRemove)
                {
                    await MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, RemoveOpertaion);
                    _needRemove = false;
                }
                if (Hives.Count == 0)
                    await MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => { StopCommand.Execute(null); });

                Parallel.ForEach(Hives, hive => hive.TickSim());
                // слишком часто...
                // MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low,UpdateMainWindow);
                LiveTime++;
            }
        }

        private void UpdateMainWindow()
        {
            if (_needCollect)
                CollectOpertaion();
            OnPropertyChanged(nameof(LiveTime));
            TotalHiveHoney = Hives.Sum(c => c.Honey);
            TotalBees = Hives.Sum(c => c.TotalBees);
        }

        private void WorkerUi()
        {
            while (true)
            {
                if (_token.IsCancellationRequested)
                    return;
                if (_needAdd || _needRemove)
                {
                    Task.Delay(300);
                }
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, UpdateMainWindow);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Delay(300);
            }
        }

        public void Suspending(object sender, SuspendingEventArgs e)
        {
            Save();
        }
    }
}
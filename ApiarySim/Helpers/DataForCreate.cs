using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiarySim.Helpers
{
    public sealed class DataForCreate
    {
        private static DataForCreate _instance;
        private static readonly object SyncRoot = new object();

        internal DataForCreate() { }

        public static DataForCreate Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DataForCreate();
                    }
                }
                return _instance;
            }
            set
            {
                lock (SyncRoot)
                {
                    _instance = value;
                }
            }
        }

        public string DataFileName = "dataFile.txt";
        public string ConfigFileName = "config.txt";
        public int HivesCount = 1290;

        public int MinWorkerInsideTime = 10;
        public int MaxWorkerInsideTime = 500;
        public int MinWorkerHoneyTime = 10;
        public int MaxWorkerHoneyTime = 500;
        public int MaxWorkerTry = 2;
        public int MinGuardCheckTime = 10;
        public int MaxGuardCheckTime = 500;
        public int MinWorkers = 100;
        public int MaxWorkers = 700;
        public int MinGuards = 10;
        public int MaxGuards = 70;
        public int MinQueen = 100;
        public int MaxQueen = 1000;
        public bool LoadFromCache = false;
    }
}

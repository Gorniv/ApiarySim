using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ApiarySim.Models;

namespace ApiarySim.ViewModels
{
    public class CacheApiary
    {
        public CacheApiary()
        {
            
        }
        public CacheApiary(ObservableCollection<Hive> hives, int collectedHoney, int liveTime)
        {
            CollectedHoney = collectedHoney;
            var temHives = hives.ToList();
            var saveHives = new List<SimpleHive>();
            foreach (var hive in temHives)
            {
                saveHives.Add(new SimpleHive(hive));
            }
            Hives = saveHives;
            LiveTime = liveTime;
        }

        public int LiveTime { get; set; }

        public List<SimpleHive> Hives { get; set; }

        public int CollectedHoney { get; set; }
    }
}
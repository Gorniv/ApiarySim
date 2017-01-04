using ApiarySim.Models;

namespace ApiarySim.ViewModels
{
    public class SimpleHive
    {
        public SimpleHive()
        {
            
        }
        public SimpleHive(Hive hive)
        {
            Guards = hive.Guards.Count;
            Workers = hive.WorkersCount;
            Honey = hive.Honey;
            Insides = hive.InsidesCount;
            Number = hive.Number;
        }

        public int Guards { get; set; }

        public int Number { get; set; }

        public int Insides { get; set; }

        public int Honey { get; set; }

        public int Workers { get; set; }


    }
}
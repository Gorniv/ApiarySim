using System.Collections.Generic;
using ApiarySim.Models;

namespace ApiarySim.Helpers
{
    public class GenerateBee<T> where T : class, new()
    {
        public static List<T> GetBees(int min,int max)
        {
            var bees = new List<T>();
            var lengh = App.Random.Next(min, max);
            for (int i = 0; i < lengh; i++)
            {
                bees.Add(new T());
            }
            return bees;
        }

    }
}
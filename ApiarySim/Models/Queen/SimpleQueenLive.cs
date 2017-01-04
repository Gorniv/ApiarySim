namespace ApiarySim.Models.Queen
{
    public class SimpleQueenLive : IQueenLive
    {
        public void AddNewBee(Hive hive)
        {
            hive.AddWorker(new WorkerBee());
        }

        public bool DoLive(QueenBee bee, Hive hive)
        {
            if (bee.TimerLive >= bee.ChildTime)
            {
                bee.TimerLive = 0;
                AddNewBee(hive);
                return true;
            }
            return false;
        }
    }
}
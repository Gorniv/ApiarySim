using ApiarySim.Helpers;

namespace ApiarySim.Models.Queen
{
    public class QueenBee : Bee
    {
        public QueenBee()
        {
            QueenLive = new SimpleQueenLive();
            ChildTime = App.Random.Next(DataForCreate.Instance.MinQueen, DataForCreate.Instance.MaxQueen);
        }

        public int ChildTime { get; set; }
        public IQueenLive QueenLive { get; set; }
    }
}
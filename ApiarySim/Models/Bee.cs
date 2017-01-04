using ApiarySim.Helpers;

namespace ApiarySim.Models
{
    public abstract class Bee
    {
        private BeeState _beeState;

        protected Bee()
        {
            Id = KeyProvader.GetBee();
        }

        public BeeState BeeState
        {
            get { return _beeState; }
            set
            {
                _beeState = value;
                TimerLive = 0;
            }
        }

        public int Id { get; set; }
        public int TimerLive { get; set; }

        public void Live()
        {
            if (int.MaxValue == TimerLive)
                TimerLive = 0;
            TimerLive++;
        }
    }
}
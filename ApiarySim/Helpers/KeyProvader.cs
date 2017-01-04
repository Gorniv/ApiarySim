namespace ApiarySim.Helpers
{
    public static class KeyProvader
    {
        private static readonly object LockBee = new object();
        private static readonly object LockHive = new object();
        private static int _nuberBees;
        private static int _nuberHives;

        public static int GetBee()
        {
            lock (LockBee)
            {
                _nuberBees++;
                return _nuberBees;
            }
        }

        public static int GetHive()
        {
            lock (LockHive)
            {
                _nuberHives++;
                return _nuberHives;
            }
        }
    }
}
namespace DefaultNamespace
{
    public class Status
    {
        private int statValue = 20;
        private int min = 0;
        public static int max = 100;
        public event System.Action changed;

        public Status()
        {
            
        }

        public int Value()
        {
            return statValue;
        }

        public int Add(int num)
            {
                statValue += num;
                if (statValue > max)
                    statValue = max;
                else if (statValue < min)
                    statValue = min;
                changed?.Invoke();
                return statValue;
            }
        
    }
}
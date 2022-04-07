namespace DefaultNamespace
{
    public class Status
    {
        private int statValue;
        private int min = 0;
        public static int max = 50;
        public event System.Action changed;

        public Status()
        {
            statValue = GameManager.instance.statusBarsStart;
            max = GameManager.instance.statusBarsMax;
        }

        public int Value()
        {
            return statValue;
        }

        public int Add(int num)
        {
            if (num == 0)
                return statValue;
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
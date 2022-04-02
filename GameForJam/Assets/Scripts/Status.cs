namespace DefaultNamespace
{
    public class Status
    {
        private int statValue;
        private int min = 0;
        private int max = 100;
        
        public int Value {
            get => statValue;
            set
            {
                statValue += value;
                if (statValue > max)
                    statValue = max;
                else if (statValue < min)
                    statValue = min;
            }
        }
    }
}
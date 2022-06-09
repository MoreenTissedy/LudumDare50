using UnityEngine;

namespace CauldronCodebase
{
    public class Status
    {
        private int statValue;
        private int min = 0;
        public static int max = 50;
        public Statustype type;
        public event System.Action changed;

        public Status(Statustype type)
        {
            this.type = type;
            if (type == Statustype.Money)
            {
                statValue = 0;
            }
            else
            {
                statValue = GameManager.instance.statusBarsStart;
            }
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
            if (type == Statustype.Money && num < 0)
                return statValue;
            statValue += num;
            if (statValue > max)
                statValue = max;
            else if (statValue < min)
                statValue = min;
            changed?.Invoke();
            Debug.Log(type+" "+statValue);
            return statValue;
        }
        
    }
}
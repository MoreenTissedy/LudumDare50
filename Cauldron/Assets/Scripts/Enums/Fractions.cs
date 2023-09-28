using System;

namespace CauldronCodebase
{
    public enum Fractions
    {
        None,
        King,
        Bishop,
        Rogue
    }

    [Serializable]
    public struct FractionData
    {
        public Fractions Fraction;
        public int Status;
    }

    public class FractionStatus
    {
        private FractionData[] data;

        public FractionStatus()
        {
            data = new FractionData[3]
            {
                new FractionData()
                {
                    Fraction = Fractions.King,
                    Status = 0
                },
                new FractionData()
                {
                    Fraction = Fractions.Bishop,
                    Status = 0
                },
                new FractionData()
                {
                    Fraction = Fractions.Rogue,
                    Status = 0
                },
            };
        }

        public void Load(int[] statuses)
        {
            if (statuses is null || statuses.Length < 3)
            {
                return;
            }
            for (int i = 0; i < 3; i++)
            {
                data[i].Status = statuses[i];
            }
        }

        public int[] Save()
        {
            var save = new int[3];
            for (int i = 0; i < 3; i++)
            {
                save[i] = data[i].Status;
            }
            return save;
        }

        public void ChangeStatus(Fractions fraction, int shift)
        {
            if (fraction is Fractions.None)
            {
                return;
            }
            data[(int) fraction - 1].Status += shift;
        }

        public override string ToString()
        {
            return $"King: {data[0].Status}, Bishop: {data[1].Status}, Rogue: {data[2].Status}";
        }
    }
}
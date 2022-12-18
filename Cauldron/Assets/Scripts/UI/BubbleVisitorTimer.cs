using UnityEngine;

namespace CauldronCodebase
{
    public class BubbleVisitorTimer: VisitorTimer
    {
        public Transform prefab;
        public float angleSpan = 15f;

        private Animator[] items;
        private int currentAttempts;
        private static readonly int Use = Animator.StringToHash("Use");

        private void Start()
        {
            prefab.gameObject.SetActive(false);
        }

        public override void ReduceTimer()
        {
            currentAttempts--;
            items[currentAttempts].SetTrigger(Use);
        }

        public override void ResetTimer(int attempts)
        {
            Clear();

            items = new Animator[attempts];
            Vector3 position = prefab.localPosition;
            for (int i = 0; i < attempts; i++)
            {
                items[i] = Instantiate(prefab, transform).GetComponent<Animator>();
                items[i].transform.localPosition =
                    Quaternion.Euler(0, 0, angleSpan * i) * position;
                items[i].gameObject.SetActive(true);
            }
            currentAttempts = attempts;
        }

        private void Clear()
        {
            if (items != null)
            {
                foreach (var animator in items)
                {
                    Destroy(animator.gameObject);
                }
            }
        }
    }
}
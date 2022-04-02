using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SimpleNumber : MonoBehaviour
    {
        public Statustype type;
        private Text text;
        private void Start()
        {
            text = GetComponent<Text>();
            switch (type)
            {
                case Statustype.Money:
                    GameManager.instance.money.changed += () => SetValue(GameManager.instance.money.Value);
                    SetValue(GameManager.instance.money.Value);
                    break;
                case Statustype.Fear:
                    GameManager.instance.fear.changed += () => SetValue(GameManager.instance.fear.Value);
                    SetValue(GameManager.instance.money.Value);
                    break;
                case Statustype.Fame:
                    GameManager.instance.fame.changed += () => SetValue(GameManager.instance.fame.Value);
                    SetValue(GameManager.instance.money.Value);
                    break;
            }
            
        }

        void SetValue(int value)
        {
            text.text = value.ToString();
        }
    }
}
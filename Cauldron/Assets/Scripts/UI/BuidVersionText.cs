using TMPro;
using UnityEngine;

namespace CauldronCodebase
{
    public class BuidVersionText : MonoBehaviour
    {
        private TMP_Text text;

        public void Awake()
        {
            text = gameObject.GetComponent<TMP_Text>();
        }

        public void Start()
        {
            text.text = $"v{Application.version}";
        }
    }
}


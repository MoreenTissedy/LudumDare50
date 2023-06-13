using TMPro;
using UnityEngine;

namespace EasyLoc
{
    [RequireComponent(typeof(TMP_Text))]
    public class EasyLocTMPTool : MonoBehaviour, ILocTextTool
    {
        [Localize] [TextArea(5, 8)]
        public string text;

        [SerializeField] protected string id;

        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                text = GetComponent<TMP_Text>().text;
                return;
            }
            //replace with the opposite
            GetComponent<TMP_Text>().text = text;
        }

        public string GetId() => id;

        public virtual void SetText(string text)
        {
            this.text = text;
            GetComponent<TMP_Text>().text = text;
        }
    }
}
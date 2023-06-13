using System;
using UnityEngine;
using UnityEngine.UI;

namespace EasyLoc
{
    [RequireComponent(typeof(Text))]
    public class EasyLocTextTool : MonoBehaviour, ILocTextTool
    {
        [Localize] [TextArea(5, 8)]
        public string text;

        [SerializeField] protected string id;

        protected virtual void OnValidate()
        {
            //replace with the opposite
            GetComponent<Text>().text = text;
        }

        public string GetId() => id;

        public virtual void SetText(string text)
        {
            this.text = text;
            GetComponent<Text>().text = text;
        }
    }
}
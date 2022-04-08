using System;
using UnityEngine;
using UnityEngine.UI;

namespace EasyLoc
{
    [RequireComponent(typeof(Text))]
    public class EasyLocTextTool : MonoBehaviour
    {
        [Localize] [TextArea(5, 8)]
        public string text;

        public string id;

        protected virtual void OnValidate()
        {
            //replace with the opposite
            GetComponent<Text>().text = text;
        }

        public virtual void SetText(string text)
        {
            this.text = text;
            GetComponent<Text>().text = text;
        }
    }
}
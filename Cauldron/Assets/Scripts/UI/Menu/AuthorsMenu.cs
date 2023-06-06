using System;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class AuthorsMenu : MonoBehaviour
    {
        [SerializeField] private Button closePanelButton;

        private void Start()
        {
            closePanelButton.onClick.AddListener(Close);
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }
        
        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}

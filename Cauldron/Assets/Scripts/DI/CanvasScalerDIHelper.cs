using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasScalerDIHelper : MonoBehaviour
    {
        [Inject]
        private void SetCamera(Camera main)
        {
            GetComponent<Canvas>().worldCamera = main;
        }
    }
}
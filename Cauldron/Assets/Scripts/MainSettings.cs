using System;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Main settings", menuName = "Main settings", order = 0)]
    public class MainSettings : ScriptableObject
    {
        public Gameplay gameplay;
        [Serializable]
        public class Gameplay
        {
             public int statusBarsMax = 100;
             public int statusBarsStart = 20;
             public int cardsPerDay = 3;
             public int cardsDealtAtNight = 3;
             public float nightDelay = 4f;
             public float villagerDelay = 2f;
        }
    }
}
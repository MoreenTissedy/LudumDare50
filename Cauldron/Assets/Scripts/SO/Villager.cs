using System;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "New_Villager", menuName = "Villager", order = 0)]
    public class Villager : ScriptableObjectWithId
    {
        public Sprite image;
        public int fameBonus;
        public int moneyBonus;
        public int fearBonus;
        public int patience = 3;
    }
}
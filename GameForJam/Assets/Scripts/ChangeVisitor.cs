using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
public class ChangeVisitor : MonoBehaviour
   {
        public static ChangeVisitor instance;

            private void Awake()
            {
                if (instance is null)
                    instance = this;
                else   
                {
                    Debug.LogError("double singleton:"+this.GetType().Name);
                }
            }    
            public void Enter(Villager villager)
            {
            
            }
    }
}
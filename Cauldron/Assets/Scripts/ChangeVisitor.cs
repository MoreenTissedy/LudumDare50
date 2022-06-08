using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace DefaultNamespace
{
public class ChangeVisitor : MonoBehaviour
   {
        public static ChangeVisitor instance;

        private SpriteRenderer rend;
        private SkeletonAnimation anim;

            private void Awake()
            {
                // if (instance is null)
                //     instance = this;
                // else
                // {
                //     Debug.LogError("double singleton:"+this.GetType().Name);
                // }
                instance = this;

                rend = GetComponent<SpriteRenderer>();
            }    
            public void Enter(Villager villager)
            {
                rend.sprite = villager.image;
            }

            public void Exit()
            {
                rend.sprite = null;
            }
    }
}
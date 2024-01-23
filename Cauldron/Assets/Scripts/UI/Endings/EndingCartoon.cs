using System;
using Spine.Unity;
using UnityEngine;

namespace CauldronCodebase
{
    public class EndingCartoon: MonoBehaviour
    {
        private SkeletonAnimation anim;

        [SpineAnimation]
        public string full = "Full";

        [SpineAnimation]
        public string idle = "Full2";

        private void Start()
        {
            anim = GetComponent<SkeletonAnimation>();
            try
            {
                anim.AnimationState.SetAnimation(0, full, false);
            }
            catch (Exception e)
            {
            }

            try
            {
                anim.AnimationState.AddAnimation(0, idle, true, 0);
            }
            catch (Exception e)
            {
            }
        }
    }
}
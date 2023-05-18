using System;
using Spine;
using UnityEngine;
using Spine.Unity;

namespace CauldronCodebase
{
    [Serializable]
    public class Visitor : MonoBehaviour
    {
        private SkeletonAnimation anim;
        private MeshRenderer rend;
        [SpineAnimation]
        public string idle, enter, exit;


        protected virtual void Awake()
        {
            rend = GetComponent<MeshRenderer>();
            rend.enabled = false;
        }

        public void Enter()
        {
            if (!anim) anim = GetComponent<SkeletonAnimation>();
            if (!rend) rend = GetComponent<MeshRenderer>();
            rend.enabled = true;
            if (!string.IsNullOrEmpty(enter))
            {
                anim.AnimationState.SetAnimation(0, enter, false);
            }
            anim.AnimationState.AddAnimation(0, idle, true, 0f);
        }

        public virtual void Exit()
        {
            if (!string.IsNullOrEmpty(exit))
            {
                anim.AnimationState.SetAnimation(0, exit, false);
                anim.AnimationState.Complete += Hide;
            }
            else
            {
                Hide(null);
            }
        }

        void Hide(TrackEntry trackEntry)
        {
            anim.AnimationState.Complete -= Hide;
            rend.enabled = false;
        }
    }
}
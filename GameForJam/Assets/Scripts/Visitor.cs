using System;
using JetBrains.Annotations;
using Spine;
using UnityEngine;
using Spine.Unity;

namespace DefaultNamespace
{
    public class Visitor : MonoBehaviour
    {
        private SkeletonAnimation anim;
        private MeshRenderer rend;
        [SpineAnimation]
        public string idle, enter, exit;

        private void Awake()
        {
            anim = GetComponent<SkeletonAnimation>();
            rend = GetComponent<MeshRenderer>();
            rend.enabled = false;
        }

        public void Enter()
        {
            rend.enabled = true;
            anim.AnimationState.SetAnimation(0, enter, false);
            anim.AnimationState.AddAnimation(0, idle, true, 0f);
        }

        public void Exit()
        {
            anim.AnimationState.SetAnimation(0, exit, false);
            anim.AnimationState.Complete += Hide;
        }

        void Hide(TrackEntry trackEntry)
        {
            rend.enabled = false;
        }
    }
}
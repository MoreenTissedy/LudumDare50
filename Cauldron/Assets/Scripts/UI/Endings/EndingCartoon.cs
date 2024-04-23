using Spine.Unity;
using UnityEngine;

namespace CauldronCodebase
{
    public class EndingCartoon: MonoBehaviour
    {
        public bool cycleFirstAnim;
        private SkeletonAnimation anim;
        const string full = "Full";
        const string idle = "Full2";

        private void Start()
        {
            anim = GetComponent<SkeletonAnimation>();
            var fullAnimation = anim.SkeletonDataAsset.GetSkeletonData(false).FindAnimation(full);
            var idleAnimation = anim.SkeletonDataAsset.GetSkeletonData(false).FindAnimation(idle);
            if (fullAnimation!=null)
            {
                anim.AnimationState.SetAnimation(0, fullAnimation, cycleFirstAnim);
            }
            if (idleAnimation!=null)
            {
                anim.AnimationState.AddAnimation(0, idleAnimation, true, 0);
            }
        }
    }
}
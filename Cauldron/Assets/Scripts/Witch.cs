using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CauldronCodebase
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class Witch : MonoBehaviour, IPointerClickHandler
    {
        private SkeletonAnimation anim;

        [SpineAnimation]
        public string active, active2, active3;
        [SpineAnimation]
        public string hide;
        [SpineAnimation]
        public string unhide;
        
        public bool Hidden { get; private set; }

        
        private void Awake()
        {
            //Respond to Brew, Nightfall, NewDay events
            anim = GetComponent<SkeletonAnimation>();
        }

        public void Activate()
        {
            int rnd = Random.Range(0, 2);
            switch (rnd)
            {
                case 0:
                    anim.AnimationState.SetAnimation(1, active, false);
                    break;
                case 1:
                    anim.AnimationState.SetAnimation(1, active2, false);
                    break;
            }

            anim.AnimationState.AddEmptyAnimation(1, 0.2f, 0);
        }

        public void Hide()
        {
            anim.AnimationState.SetAnimation(1, hide, false);
            Hidden = true;
        }

        public void Wake()
        {
            anim.AnimationState.SetAnimation(1, unhide, false);
            Hidden = false;
            anim.AnimationState.AddEmptyAnimation(1, 0.2f, 0);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Fly();
        }

        public void Fly()
        {
            anim.AnimationState.SetAnimation(1, active3, false);
            anim.AnimationState.AddEmptyAnimation(1, 0.2f, 0);
        }
    }
}
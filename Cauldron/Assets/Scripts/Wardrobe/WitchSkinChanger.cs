using Spine.Unity;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class WitchSkinChanger : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation witchSkeleton;
        [SerializeField] private ParticleSystem skinChangeVFX;

        [SpineAnimation, SerializeField] private string skinChangeBeginAnimation;
        
        [SpineAnimation, SerializeField] private string activeAnimation;
        
        [SpineAnimation, SerializeField] private string idleAnimation;

        private SkinSO currentSkin;
        public SkinSO CurrentSkin => currentSkin;

        private bool skinChangeAvailable = true;
        public bool SkinChangeAvailable => skinChangeAvailable;

        [Inject] private GameDataHandler gameDataHandler;

        private void Start()
        {
            currentSkin = gameDataHandler.currentSkin;
            witchSkeleton.Skeleton.SetSkin(currentSkin.SpineName);
        }

        public void ChangeSkin(SkinSO newSkin)
        {
            if (newSkin == currentSkin) return;

            currentSkin = newSkin;
            gameDataHandler.currentSkin = newSkin;
            
            skinChangeVFX.Play();
            witchSkeleton.AnimationState.SetAnimation(0, activeAnimation, false);
            witchSkeleton.Skeleton.SetSkin(currentSkin.SpineName);
            witchSkeleton.AnimationState.AddAnimation(0, idleAnimation, true, 0);
        }
    }
}
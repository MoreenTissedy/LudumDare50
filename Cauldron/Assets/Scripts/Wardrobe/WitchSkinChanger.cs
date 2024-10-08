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
        public bool SkinChangeAvailable { get; } = true;

        [Inject] private SkinsProvider skinsProvider;

        private void Start()
        {
            currentSkin = skinsProvider.GetLastUnlocked();
            witchSkeleton.Skeleton.SetSkin(currentSkin.SpineName);
        }

        public void ChangeSkin(SkinSO newSkin)
        {
            if (newSkin == currentSkin) return;

            currentSkin = newSkin;
            
            skinChangeVFX.Play();
            witchSkeleton.AnimationState.SetAnimation(0, activeAnimation, false);
            witchSkeleton.Skeleton.SetSkin(currentSkin.SpineName);
            witchSkeleton.AnimationState.AddAnimation(0, idleAnimation, true, 0);
        }
    }
}
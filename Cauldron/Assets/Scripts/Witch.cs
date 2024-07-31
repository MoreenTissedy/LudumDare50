using System;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase.GameStates;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    [Serializable]
    public struct WitchSkinSet
    {
        public string LastUnlockedEnding;
        [SpineSkin()] public string WitchSkin;
    }
    
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

        [SpineAnimation()] public string angry;

        [SerializeField] private WitchSkinSet[] skinSets;

        [Inject] private Cauldron cauldron;
        [Inject] private GameDataHandler gameDataHandler;
        [Inject] private GameStateMachine gameStateMachine;
        [Inject] private EndingsProvider endingsProvider;
        
        public bool Hidden { get; private set; }

        private List<string> unlockedSkins;
        private IReadOnlyList<string> unlockedEndings;

        
        private void Awake()
        {
            anim = GetComponent<SkeletonAnimation>();
        }

        private void Start()
        {
            unlockedEndings = endingsProvider.UnlockedEndings;
            SetWitchSkin();
            cauldron.PotionBrewed += CauldronOnPotionBrewed;
            gameStateMachine.OnChangeState += OnDayNightChange;
        }

        private void OnDayNightChange(GameStateMachine.GamePhase phase)
        {
            if (phase != GameStateMachine.GamePhase.Night && Hidden)
            {
                anim.AnimationState.SetAnimation(1, unhide, false);
                anim.AnimationState.AddEmptyAnimation(1, 0.2f, 0);
                Hidden = false;
            }
            else if (phase == GameStateMachine.GamePhase.Night)
            {
                anim.AnimationState.SetAnimation(1, hide, false);
                Hidden = true;
            }
        }

        private void SetWitchSkin()
        {
            if (unlockedEndings.Count > 0)
            {
                var skinSet = skinSets.FirstOrDefault(x => x.LastUnlockedEnding == unlockedEndings[unlockedEndings.Count - 1]);
                if (!string.IsNullOrWhiteSpace(skinSet.WitchSkin))
                {
                    anim.Skeleton.SetSkin(skinSet.WitchSkin);
                }
            }
        }

        private void CauldronOnPotionBrewed(Potions potion)
        {
            if (potion == Potions.Placebo && gameDataHandler.wrongExperiments > 3)
            {
                anim.AnimationState.SetAnimation(1, angry, false);
                anim.AnimationState.AddEmptyAnimation(1, 0.2f, 0);
            }
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

        public void OnPointerClick(PointerEventData eventData)
        {
            Fly();
            
            if (unlockedEndings.Count == 0)
            {
                return;
            }

            if (unlockedSkins is null)
            {
                unlockedSkins = GetUnlockedSkins();
            }
            if (unlockedSkins.Count == 0)
            {
                return;
            }
            
            int currentSkinIndex = unlockedSkins.IndexOf(anim.Skeleton.Skin.Name);
            string nextSkin = unlockedSkins[(currentSkinIndex + 1) % unlockedSkins.Count];
            if (!string.IsNullOrWhiteSpace(nextSkin))
            {
                anim.Skeleton.SetSkin(nextSkin);
            }
        }

        private List<string> GetUnlockedSkins()
        {
            List<string> skinList = new List<string>() {"main"};
            foreach (var set in skinSets)
            {
                if (unlockedEndings.Contains(set.LastUnlockedEnding) && !skinList.Contains(set.WitchSkin))
                {
                    skinList.Add(set.WitchSkin);
                }
            }

            return skinList;
        }

        private void Fly()
        {
            anim.AnimationState.SetAnimation(1, active3, false);
            anim.AnimationState.AddEmptyAnimation(1, 0.2f, 0);
        }
    }
}
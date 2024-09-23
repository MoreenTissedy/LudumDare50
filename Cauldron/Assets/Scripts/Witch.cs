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

        [Inject] private Cauldron cauldron;
        [Inject] private GameDataHandler gameDataHandler;
        [Inject] private GameStateMachine gameStateMachine;
        
        public bool Hidden { get; private set; }
        
        private void Awake()
        {
            anim = GetComponent<SkeletonAnimation>();
        }

        private void Start()
        {
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
        }

        private void Fly()
        {
            anim.AnimationState.SetAnimation(1, active3, false);
            anim.AnimationState.AddEmptyAnimation(1, 0.2f, 0);
        }
    }
}
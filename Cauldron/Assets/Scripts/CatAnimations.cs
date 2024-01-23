using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    public class CatAnimations : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
    {
        [SerializeField] private SkeletonAnimation catSkeleton;
        [FormerlySerializedAs("idleAnimations")] [SpineAnimation] public string[] randomActions;
        [SpineAnimation] public string strokeAnimation;
        [SerializeField] private float strokeAnimationSpeedMultiplier;
        [SpineAnimation] public string dragAnimation;
        [SpineAnimation] public string moveAnimation;
        [SpineAnimation] public string idleAnimation = "idle";

        [SpineAnimation] public string hideEffect;
        [SpineAnimation] public string showEffect;
        
        [Tooltip("Interval in minutes")]
        public float minIdleInterval = 0.5f;
        public float maxIdleInterval = 1f;

        [Header("Drag")]
        [SerializeField] private Vector2 dragAnimationOffset;
        [SerializeField] private float dragThreshold;
        public bool IsDragged { get; private set; }
        public bool IsInCauldron { get; private set; }
        private bool dragAnimated;
        private Vector2 startDragPosition;

        [Header("Movement")]
        [SerializeField] private CatPath catPath;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float catDropSpeed;

        [Header("Eating")] 
        [SpineAnimation] public string eatingAnimation;
        [SpineSlot()] public string eatingSlot;

        public event Action MouseOverCat;

        private Vector3 startPosition;

        private Collider2D col;

        private Tween moveTween;
        private Tween dropTween;
        
        private Coroutine randomAction;

        [Inject] private SoundManager soundManager;
        [Inject] private Cauldron cauldron;
        
        // convert ingredient to eating slot name
        private Dictionary<Ingredients, string> EatingSlots = new Dictionary<Ingredients, string>() 
        {
            { Ingredients.Snake, "Snake" },
            { Ingredients.Root1, "root1" },
            { Ingredients.Root2, "root2" },
            { Ingredients.Leaf1, "leaf1" },
            { Ingredients.Leaf2, "leaf2" },
            { Ingredients.Agaricus, "agaricus" },
            { Ingredients.Amanita, "amanita1" },
            { Ingredients.Bat, "bat" },
            { Ingredients.Rat, "rat" },
            { Ingredients.Toadstool, "toadstool" },
        };
        
        

        private void Start()
        {
            col = GetComponent<Collider2D>();
            startPosition = transform.position;
            
            randomAction = StartCoroutine(RandomActionsRoutine());
        }

        // the method was chosen for correct behavior when the player released the cat directly inside the cauldron
        private async void OnTriggerStay2D(Collider2D other)
        {
            if (IsDragged || IsInCauldron) return;

            IsInCauldron = true;
            transform.DOKill();
            cauldron.splash.Play();
            soundManager.Play(Sounds.Splash);
            gameObject.SetActive(false);
            catSkeleton.AnimationState.SetEmptyAnimation(0, 0);

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            
            gameObject.SetActive(true);
            soundManager.PlayCat(CatSound.Appear);
            catSkeleton.skeleton.ScaleX = 1;
            transform.position = startPosition;
            PlayAnimationOneShot(showEffect);
            IsInCauldron = false;
        }

        public void SetInteractable(bool interact)
        {
            col.enabled = interact;
        }
        
        public void Move(bool toStartSpot)
        {
            StopAllCoroutines();
            
            var path = catPath.GetPath(transform, toStartSpot);
            
            catSkeleton.AnimationState.SetAnimation(0, moveAnimation, true);

            moveTween = transform.DOPath(path, movementSpeed, PathType.CatmullRom)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    catSkeleton.skeleton.ScaleX = toStartSpot ? 1 : -1;
                    catSkeleton.AnimationState.SetAnimation(0, idleAnimation, true);
                    randomAction = StartCoroutine(RandomActionsRoutine());
                });
        }
        
        private void DropAnimation()
        {
            bool rightWay = transform.position.x < startPosition.x;
            var closestWaypoint = catPath.FindClosestWaypoint(transform.position.x, rightWay).position;
            
            var moveDirection = (closestWaypoint - transform.position).normalized;
            if (Vector2.Dot(Vector2.left, moveDirection) < 0)
            {
                catSkeleton.skeleton.ScaleX = -1;
            }
            else
            {
                catSkeleton.skeleton.ScaleX = 1;
            }

            dropTween = transform.DOMoveY(closestWaypoint.y, catDropSpeed)
                                .SetSpeedBased(true)
                                .SetEase(Ease.InSine)
                                .OnComplete(CatLanding);
        }
        
        private void CatLanding()
        {
            TrackEntry strokeEntry = catSkeleton.AnimationState.SetAnimation(0, strokeAnimation, false);
            soundManager.PlayCat(CatSound.Annoyed);
            catSkeleton.timeScale = strokeAnimationSpeedMultiplier;
            
            strokeEntry.Complete += (_) =>
            {
                catSkeleton.timeScale = 1;
                Move(true);
            };
        }

        private IEnumerator RandomActionsRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minIdleInterval, maxIdleInterval) * 60);
                PlayRandomAction();
            }
        }

        private void PlayRandomAction()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            soundManager.PlayCat(CatSound.Annoyed);
            PlayAnimationOneShot(randomActions[Random.Range(0, randomActions.Length)]);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(randomAction != null)
                StopCoroutine(randomAction);
            
            dropTween?.Kill();
            moveTween?.Kill();

            startDragPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            CheckDragBegin(eventData.position);
            
            if (IsDragged)
            {
                var targetPosition = Camera.main.ScreenToWorldPoint((Vector3)eventData.position + Vector3.forward * Camera.main.nearClipPlane) 
                                            + (Vector3)dragAnimationOffset;
                transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
                EnableDragAnimation();
            }
        }

        private void CheckDragBegin(Vector2 pos)
        {
            if(IsDragged) return;
            
            IsDragged = (startDragPosition - pos).magnitude > dragThreshold;
            if (IsDragged)
            {
                soundManager.PlayCat(CatSound.Annoyed);
            }
        }

        private void EnableDragAnimation()
        {
            if(dragAnimated) return;

            dragAnimated = true;
            catSkeleton.AnimationState.SetAnimation(0, dragAnimation, true);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (IsDragged)
            {
                IsDragged = false;
                dragAnimated = false;
                DropAnimation();
            }
            else
            {
                soundManager.PlayCat(CatSound.Purr);
                PlayAnimationOneShot(strokeAnimation);
            }
        }

        private void PlayAnimationOneShot(string animation)
        {
            catSkeleton.AnimationState.SetAnimation(0, animation, false);
            catSkeleton.AnimationState.AddAnimation(0, idleAnimation, true, 0f);
        }

        public void SetEatingAnimation(Ingredients ingredient)
        {
            if (!EatingSlots.TryGetValue(ingredient, out string slot)) return;
            
            catSkeleton.Skeleton.SetAttachment(eatingSlot, slot);
            catSkeleton.AnimationState.SetAnimation(0, eatingAnimation, false);
            catSkeleton.AnimationState.AddAnimation(0, idleAnimation, true, 0f);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseOverCat?.Invoke();
        }
    }
}
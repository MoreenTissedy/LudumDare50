using System.Collections;
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
    public class CatAnimations : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        private bool dragAnimated;
        private Vector2 startDragPosition;

        [Header("Movement")]
        [SerializeField] private CatPath catPath;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float catDropSpeed;

        private Vector3 startPosition;

        private Collider2D col;

        private Tween moveTween;
        private Tween dropTween;
        
        private Coroutine randomAction;

        [Inject] private SoundManager soundManager;
        [Inject] private Cauldron cauldron;

        private void Start()
        {
            col = GetComponent<Collider2D>();
            startPosition = transform.position;
            
            randomAction = StartCoroutine(RandomActionsRoutine());
        }

        // the method was chosen for correct behavior when the player released the cat directly inside the cauldron
        private void OnTriggerStay2D(Collider2D other)
        {
            if (IsDragged) return;
            
            transform.DOKill();
            cauldron.splash.Play();
            
            PlayAnimationOneShot(showEffect);
            catSkeleton.skeleton.ScaleX = 1;
            transform.position = startPosition;
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
                soundManager.PlayCat(CatSound.Annoyed);
                PlayAnimationOneShot(strokeAnimation);
            }
        }

        private void PlayAnimationOneShot(string animation)
        {
            catSkeleton.AnimationState.SetAnimation(0, animation, false);
            catSkeleton.AnimationState.AddAnimation(0, idleAnimation, true, 0f);
        }
    }
}
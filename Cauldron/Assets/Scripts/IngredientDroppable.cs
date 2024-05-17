using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Universal;
using Zenject;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    public class IngredientDroppable: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, 
        IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        private const float doubleClickTime = 0.5f;
        
        public Ingredients ingredient;
        public float rotateAngle = 10f;
        public float rotateSpeed = 0.3f;
        public float returntime = 0.5f;
        public IngredientsData dataList;
        public Collider2D collider;
        
        [SerializeField, HideInInspector]
        private ScrollTooltip tooltip;
        [SerializeField, HideInInspector]
        private SpriteRenderer image;

        [SerializeField] GameObject ingredientParticle;
        [SerializeField] private GameObject dragTrail;
        [SerializeField] private bool useDoubleClick;
        bool isHighlighted = false;
        private Vector3 initialPosition;
        private bool dragging;
        private float lastClickTime = -1000f;

        private CatAnimations catAnimations;
        private Cauldron cauldron;
        private TooltipManager ingredientManager;
        private float initialRotation;
        private CancellationTokenSource cancellationTokenSource;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            tooltip = GetComponentInChildren<ScrollTooltip>();
            ChangeText();

            image = GetComponentInChildren<SpriteRenderer>();
            image.sprite = dataList?.Get(ingredient)?.image;
        }
#endif

        private void ChangeText()
        {
            tooltip.SetText(dataList?.Get(ingredient)?.friendlyName ?? "not specified").Forget();
        }
        
        [Inject]
        public void Construct(Cauldron cauldron, CatAnimations catAnimations)
        {
            this.catAnimations = catAnimations;
            this.cauldron = cauldron;
            ingredientManager = cauldron.tooltipManager;
            ingredientManager.AddIngredient(this);
            dataList.Changed += ChangeText;
        }

        private void OnEnable()
        {
            initialRotation = image.gameObject.transform.rotation.eulerAngles.z;
            transform.DOScale(transform.localScale, rotateSpeed).From(Vector3.zero);
        }

        private void OnDisable()
        {
            DisableHighlight();
        }

        private void Start()
        {
            if (tooltip != null)
            {
                ChangeText();
            }
            
            initialPosition = transform.position;
            ingredientParticle?.SetActive(false);
            dragTrail?.SetActive(false);
            useDoubleClick = true;
        }

        private void OnDestroy()
        {
            ingredientManager.RemoveIngredient(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (catAnimations.IsDragged) return;
            
            if (!cauldron.Mix.Contains(ingredient))
            {
                image.gameObject.transform.DORotate(new Vector3(0, 0, initialRotation + rotateAngle), rotateSpeed)
                    .SetLoops(-1, LoopType.Yoyo).From(new Vector3(0, 0, initialRotation - rotateAngle))
                    .SetEase(Ease.InOutSine);
            }
            OpenTooltipWithDelay().Forget();
        }

        private async UniTask OpenTooltipWithDelay()
        {
            cancellationTokenSource = new CancellationTokenSource();
            await UniTask.Delay(600, DelayType.Realtime, PlayerLoopTiming.FixedUpdate, cancellationTokenSource.Token);
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                tooltip?.Open();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            cancellationTokenSource?.Cancel();
            tooltip?.Close();
            
            image.transform.DOKill();
            image.transform.DORotate(new Vector3(0, 0, initialRotation), rotateSpeed);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            bool doubleClick = Time.timeSinceLevelLoad - lastClickTime < doubleClickTime;
            lastClickTime = Time.timeSinceLevelLoad;
            if (doubleClick || Gamepad.current != null)
            {
                ThrowInCauldron().Forget();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (cauldron.Mix.Contains(ingredient))
                return;
            SetMovementVisuals();
            cauldron.MouseEnterCauldronZone += OverCauldron;
            catAnimations.MouseOverCat += OverCat;
        }

        private void SetMovementVisuals()
        {
            image.sortingLayerName = "Hints";
            collider.enabled = false;
            tooltip?.Close();
            dragging = true;
            image.transform.DOKill(true);
            dragTrail?.SetActive(true);
            ingredientParticle?.SetActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!dragging)
                return;
            transform.position =
                Camera.main.ScreenToWorldPoint((Vector3)eventData.position + Vector3.forward * Camera.main.nearClipPlane);
        }

        private void OverCauldron()
        {
            ReturnToStartSpot();
            cauldron.AddToMix(ingredient);
            Unsubscribe();
        }


        private void OverCat()
        {
            ReturnToStartSpot();
            catAnimations.SetEatingAnimation(ingredient);
            Unsubscribe();
        }

        private void ReturnToStartSpot()
        {
            ResetVisuals();
            transform.position = initialPosition;
            transform.DOScale(Vector3.one, rotateSpeed).From(Vector3.zero).ToUniTask();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!dragging)
                return;
            transform.DOMove(initialPosition, returntime);
            ResetVisuals();
            if (isHighlighted)
            {
                ingredientParticle?.SetActive(true);
            }
            Unsubscribe();
        }

        private void ResetVisuals()
        {
            image.sortingLayerName = "Interactables";
            collider.enabled = true;
            dragging = false;
            dragTrail?.SetActive(false);
        }

        private void Unsubscribe()
        {
            cauldron.MouseEnterCauldronZone -= OverCauldron;
            catAnimations.MouseOverCat -= OverCat;
        }

        public void EnableHighlight()
        {
            if(!isHighlighted)
            {
                ingredientParticle?.SetActive(true);
                isHighlighted = true;
            }
        }

        public void DisableHighlight()
        {
            if(isHighlighted)
            {
                ingredientParticle?.SetActive(false);
                isHighlighted = false;
            }
        }

        public void ChangeHighlight(bool state)
        {
            if(isHighlighted != state)
            {
                ingredientParticle?.SetActive(state);
                isHighlighted = state;
            }
        }

        public async UniTaskVoid ThrowInCauldron()
        {
            if(!useDoubleClick)
                return;
            
            if (cauldron.Mix.Contains(ingredient))
                 return;
            
            const float timeMoveDoubleClick = 0.5f;

            SetMovementVisuals();
            await transform.DOPath(GetRandomBezierPath(), timeMoveDoubleClick, PathType.CubicBezier).SetEase(Ease.Flash).ToUniTask();
            ReturnToStartSpot();
            cauldron.AddToMix(ingredient);
        }

        private Vector3[] GetRandomBezierPath()
        {
            Vector3 cauldronPosition = cauldron.transform.position;
            Vector3 targetPosition = cauldronPosition + Vector3.up;
            
            Vector3 randomDirection = Random.insideUnitCircle.normalized;
            float desiredXDirection = Mathf.Sign(cauldronPosition.x - transform.position.x);
            if (Mathf.Sign(randomDirection.x) != desiredXDirection)
            {
                randomDirection.x *= -1;
            }
            randomDirection *= Random.Range(1f, 3f);
            randomDirection += transform.position;
            return new[]
            {
                targetPosition,
                randomDirection,
                targetPosition + Vector3.up * 5, 
            };
        }
    }
}
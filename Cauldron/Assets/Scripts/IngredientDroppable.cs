using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class IngredientDroppable: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, 
        IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public Ingredients ingredient;
        public float rotateAngle = 10f;
        public float rotateSpeed = 0.3f;
        public float returntime = 0.5f;
        public IngredientsData dataList;
        
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

        private Cauldron cauldron;
        private TooltipManager ingredientManager;
        private float initialRotation;
        private CancellationTokenSource cancellationTokenSource;
        private Vector3[] pathDoubleClickAnimation;
        
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
            tooltip.SetText(dataList?.Get(ingredient)?.friendlyName ?? "not specified");
        }
        
        [Inject]
        public void Construct(Cauldron cauldron)
        {
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

        private void OnDestroy()
        {
            ingredientManager.RemoveIngredient(this);
        }

        private void Start()
        {
            if (tooltip != null)
            {
                ChangeText();
            }
            
            pathDoubleClickAnimation = new Vector3[]
            {
                new Vector3(transform.position.x, transform.position.y + 1, transform.position.z),
                new Vector3(transform.position.x, transform.position.y - 2, transform.position.z),
                new Vector3(cauldron.transform.position.x, cauldron.transform.position.y + 5, cauldron.transform.position.z),
                new Vector3(cauldron.transform.position.x, cauldron.transform.position.y + 1, cauldron.transform.position.z),
            };

            initialPosition = transform.position;
            ingredientParticle?.SetActive(false);
            dragTrail?.SetActive(false);
            useDoubleClick = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
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
            if (eventData.clickCount >= 2) 
                TrowInCauldron();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (cauldron.Mix.Contains(ingredient))
                return;
            EnableDrag();
            cauldron.MouseEnterCauldronZone += OverCauldron;
        }

        private void OverCauldron()
        {
            ReturnIngredient();
            cauldron.MouseEnterCauldronZone -= OverCauldron;
        }

        private void EnableDrag()
        {
            dragging = true;
            image.transform.DOKill(true);
            dragTrail?.SetActive(true);
            ingredientParticle?.SetActive(false);
        }

        private void ReturnIngredient()
        {
            cauldron.AddToMix(ingredient);
            dragging = false;
            dragTrail?.SetActive(false);
            transform.position = initialPosition;
            transform.DOScale(transform.localScale, rotateSpeed).From(Vector3.zero);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!dragging)
                return;
            transform.position =
                Camera.main.ScreenToWorldPoint((Vector3)eventData.position + Vector3.forward * Camera.main.nearClipPlane);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!dragging)
                return;
            transform.DOMove(initialPosition, returntime);
            dragging = false;
            dragTrail?.SetActive(false);
            if (isHighlighted)
            {
                ingredientParticle?.SetActive(true);
            }
            cauldron.MouseEnterCauldronZone -= OverCauldron;
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

        private async void TrowInCauldron()
        {
            if(!useDoubleClick)
                return;
            
            if (cauldron.Mix.Contains(ingredient))
                 return;
            
            const float timeMoveDoubleClick = 1.3f;

            EnableDrag();
            transform.DOPath(pathDoubleClickAnimation, timeMoveDoubleClick, PathType.CatmullRom);
            await Task.Delay(1300);
            ReturnIngredient();
        }
    }
}
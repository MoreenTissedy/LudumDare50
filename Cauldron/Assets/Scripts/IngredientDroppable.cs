using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class IngredientDroppable: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, 
        IBeginDragHandler, IDragHandler, IEndDragHandler
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
        bool isHighlighted = false;
        private Vector3 initialPosition;
        private bool dragging;

        private Cauldron cauldron;
        private TooltipManager ingredientManager;
        private float initialRotation;

        
#if UNITY_EDITOR
        private void OnValidate()
        {
            tooltip = GetComponentInChildren<ScrollTooltip>();
            ChangeText();

            image = GetComponentInChildren<SpriteRenderer>();
            image.sprite = dataList?.Get(ingredient)?.image;
        }

        private void ChangeText()
        {
            tooltip.SetText(dataList?.Get(ingredient)?.friendlyName ?? "not specified");
        }

#endif
        
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

            initialPosition = transform.position;
            ingredientParticle?.SetActive(false);
            dragTrail?.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltip?.Open();
            if (!cauldron.Mix.Contains(ingredient))
                image.gameObject.transform.
                    DORotate(new Vector3(0,0, initialRotation+rotateAngle), rotateSpeed).
                    SetLoops(-1, LoopType.Yoyo).
                    From(new Vector3(0, 0, initialRotation-rotateAngle)).
                    SetEase(Ease.InOutSine);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltip?.Close();
            
            image.transform.DOKill();
            image.transform.DORotate(new Vector3(0, 0, initialRotation), rotateSpeed);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (cauldron.Mix.Contains(ingredient))
                return;
            dragging = true;
            image.transform.DOKill(true);
            cauldron.MouseEnterCauldronZone += OverCauldron;
            dragTrail?.SetActive(true);
            ingredientParticle?.SetActive(false);
        }

        void OverCauldron()
        {
            cauldron.AddToMix(ingredient);
            dragging = false;
            dragTrail?.SetActive(false);
            transform.position = initialPosition;
            cauldron.MouseEnterCauldronZone -= OverCauldron;
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
    }
}
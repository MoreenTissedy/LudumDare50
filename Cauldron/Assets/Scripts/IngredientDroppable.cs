using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
        private Text tooltip;
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

        [Inject]
        public void Construct(Cauldron cauldron, TooltipManager ingredientManager)
        {
            this.cauldron = cauldron;
            this.ingredientManager = ingredientManager;
        }

        private void OnEnable()
        {
            initialRotation = image.gameObject.transform.rotation.eulerAngles.z;
            ingredientManager.AddIngredient(this);
            transform.DOScale(transform.localScale, rotateSpeed).From(Vector3.zero);
        }

        private void OnDisable()
        {
            DisableHighlight();
            ingredientManager.RemoveIngredient(this);
        }

        private void OnValidate()
        {
            tooltip = GetComponentInChildren<Text>();
            if (tooltip != null && tooltip.text == String.Empty)
            {
                tooltip.text = dataList?.Get(ingredient)?.friendlyName ?? "not specified";
                //Debug.Log("override ingredient text!");
            }

            image = GetComponentInChildren<SpriteRenderer>();
            image.sprite = dataList?.Get(ingredient)?.image;
        }

        private void Start()
        {
            if (tooltip != null)
            {
                tooltip.text = dataList?.Get(ingredient)?.friendlyName ?? "not specified";
                tooltip.gameObject.SetActive(false);
            }

            initialPosition = transform.position;
            ingredientParticle?.SetActive(false);
            dragTrail?.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltip is null)
                return;
            tooltip.gameObject.SetActive(true);
            if (!cauldron.mix.Contains(ingredient))
                image.gameObject.transform.
                    DORotate(new Vector3(0,0, initialRotation+rotateAngle), rotateSpeed).
                    SetLoops(-1, LoopType.Yoyo).
                    From(new Vector3(0, 0, initialRotation-rotateAngle)).
                    SetEase(Ease.InOutSine);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltip is null)
                return;
            tooltip.gameObject.SetActive(false);
            image.transform.DOKill();
            image.transform.DORotate(new Vector3(0, 0, initialRotation), rotateSpeed);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (cauldron.mix.Contains(ingredient))
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
        
    }
}
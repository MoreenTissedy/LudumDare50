using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CauldronCodebase
{
    /// <summary>
    /// child to cauldron, add box collider
    /// </summary>
    public class Fire : MonoBehaviour, IPointerClickHandler
    {
        public ParticleSystem bubbles;
        public AudioSource bubblesSound;
        public GameObject steam;
        public AudioClip fireSound;
        public float boilingTemp = 100;
        public float maxTemp = 150;
        [Tooltip("How much the temperature increases each time player swipes the fire")]
        public float tempIncrement = 10;
        [Tooltip("How much the temperature drops with every ingredient")]
        public float tempDecrement = 20;
        public float initialTemp = 20;
        [Tooltip("Temperature reduction in seconds")]
        public float tempReduction = 1f;
        [Tooltip("Additional temperature reduction when mixing, coef of mixing speed")]
        public float reductionBonus = 1f;

        Animator animator;
        bool animatorFound;

        [Header("DEBUG")]
        public float temperature;
        public float Temperature => temperature;
        public bool boiling = true;
        public bool Boiling => boiling;

        private Mix mixing;
        private bool mixingFound;
        private static readonly int TempAnimParam = Animator.StringToHash("Temperature");

        private void Awake()
        {
            GetComponentInParent<Cauldron>().IngredientAdded += JoltTemperature;
            mixing = GetComponentInParent<Mix>();
            if (mixing is null)
            {
                mixingFound = false;
            }
            else
            {
                mixingFound = true;
            }

            animator = GetComponent<Animator>();
            if (!(animator is null))
            {
                animatorFound = true;
            }

            temperature = initialTemp;
            bool initialBoiling = initialTemp > boilingTemp;
            SetBoiling(initialBoiling);
            StartCoroutine(ControlTemperature());
        }

        void SetBoiling(bool active)
        {
            if (boiling == active)
                return;
            boiling = active;
            if (active)
            {
                bubbles.Play();
                bubblesSound.Play();
                steam.SetActive(true);
                steam.transform.DOScaleX(1, 0.2f).From(0);
            }
            else
            {
                bubbles.Stop();
                bubblesSound.Stop();
                steam.transform.DOScaleX(0, 0.2f).From(1)
                    .OnComplete(() => steam.SetActive(false));
            }
        }

        private void JoltTemperature(Ingredients ingredients)
        {
            temperature -= tempDecrement;
        }

        IEnumerator ControlTemperature()
        {
            var waitForSeconds = new WaitForSeconds(1);
            while (true)
            {
                yield return waitForSeconds;
                if (temperature<=initialTemp)
                    continue;
                temperature -= tempReduction;
                if (mixingFound)
                {
                    temperature -= Mathf.Abs(mixing.Speed) * reductionBonus;
                }

                if (animatorFound)
                {
                    animator.SetFloat(TempAnimParam, temperature);
                }
                SetBoiling(temperature > boilingTemp);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            bubblesSound.PlayOneShot(fireSound, 0.5f);
            temperature += tempIncrement;
        }

        
    }
}
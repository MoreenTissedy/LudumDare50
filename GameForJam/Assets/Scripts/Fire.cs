using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    /// <summary>
    /// child to cauldron, add box collider
    /// </summary>
    public class Fire : MonoBehaviour, IPointerClickHandler
    {
        public ParticleSystem bubbles;
        public AudioSource bubblesSound;
        public AudioClip fireSound;
        public float boilingTemp = 100;
        [Tooltip("How much the temperature increases each time player swipes the fire")]
        public float tempIncrement = 10;
        [Tooltip("How much the temperature drops with every ingredient")]
        public float tempDecrement = 20;
        public float initialTemp = 20;
        [Tooltip("Temperature reduction in seconds")]
        public float tempReduction = 1f;
        [Tooltip("Additional temperature reduction when mixing, coef of mixing speed")]
        public float reductionBonus = 1f;

        public float temperature;
        public float Temperature => temperature;
        public bool boiling = true;
        public bool Boiling => boiling;

        private Mix mixing;
        private bool mixingFound;

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
            }
            else
            {
                bubbles.Stop();
                bubblesSound.Stop();
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

                SetBoiling(temperature > boilingTemp);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            bubblesSound.PlayOneShot(fireSound, 0.5f);
            temperature += tempIncrement;
        }

        public void OnDrag(PointerEventData eventData)
        {
        }
    }
}
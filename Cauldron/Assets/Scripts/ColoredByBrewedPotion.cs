using System.Collections;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    /// <summary>
    /// Класс используется для покраски объектов в зависимости от зелья в котле
    /// </summary>
    public class ColoredByBrewedPotion : MonoBehaviour
    {
        [Range(-1f, 1f)] [SerializeField] private float vibranceModifier = 0f; //Модификатор цвета
        [SerializeField] private float delay = 0f; //Задержка смены цвета в секундах
        [SerializeField] private bool tween = false; //true - менять цвет постепенно, false - мгновенно, 
        [SerializeField] private float tweenTime = 1f; //Длительность(плавность) смены цвета в секундах

        private RecipeProvider recipeProvider;
        private Cauldron cauldron;


        [Inject]
        private void Construct(Cauldron cauldron, RecipeProvider recipeProvider)
        {
            this.recipeProvider = recipeProvider;
            this.cauldron = cauldron;
        }

        private void Start()
        {
            cauldron.PotionBrewed += DefineColor;
            cauldron.PotionAccepted += ResetColor;
        }

        private void OnDisable()
        {
            cauldron.PotionBrewed -= DefineColor;
            cauldron.PotionAccepted -= ResetColor;
        }

        /// <summary>
        /// Сброс цвета до нейтрального
        /// </summary>
        /// <param name="potion"></param>
        private void ResetColor(Potions potion)
        {
            Color color = Color.grey;
            float currentAlpha = GetComponent<SpriteRenderer>().color.a;
            color.a = currentAlpha;
            GetComponent<SpriteRenderer>().color = color;
        }

        /// <summary>
        /// Определяем цвет в зависимости от наличия рецепта зелья
        /// </summary>
        /// <param name="potion"></param>
        private void DefineColor(Potions potion)
        {
            Recipe recipe = recipeProvider.GetRecipeForPotion(potion);
            Color color;
            if (recipe != null)
            {
                color = recipe.color;
            }
            else
            {
                color = Color.HSVToRGB(0.97f, 0.68f, 0.27f);
            }

            ChangeColor(color);
        }

        /// <summary>
        /// Меняем цвет плавно или резко
        /// </summary>
        /// <param name="color"></param>
        private void ChangeColor(Color color)
        {
            color = ModifyColor(color);
            if (tween)
            {
                StartCoroutine(DoColor(color));
            }
            else
            {
                GetComponent<SpriteRenderer>().color = color;
            }
        }

        /// <summary>
        /// Смешиваем цвет
        /// </summary>
        /// <param name="color"></param>
        private Color ModifyColor(Color color)
        {
            float currentAlpha = GetComponent<SpriteRenderer>().color.a;
            Color.RGBToHSV(color, out float h, out float s, out float v);
            s += vibranceModifier;
            s = Mathf.Clamp(s, 0f, 1f);
            Color newColor = Color.HSVToRGB(h, s, v);
            if (currentAlpha < color.a)
            {
                newColor.a = currentAlpha;
            }

            return newColor;
        }

        /// <summary>
        /// Применяем цвет к объекту
        /// </summary>
        /// <param name="color"></param>
        private IEnumerator DoColor(Color color)
        {
            float timer = 0f;
            Color startColor = GetComponent<SpriteRenderer>().color;
            yield return new WaitForSeconds(delay);
            while (timer < tweenTime)
            {
                timer += Time.deltaTime;
                float blend = Mathf.Clamp01(timer / tweenTime);
                Color blendedColor = Color.Lerp(startColor, color, blend);
                GetComponent<SpriteRenderer>().color = blendedColor;
                yield return null;
            }
        }
    }
}
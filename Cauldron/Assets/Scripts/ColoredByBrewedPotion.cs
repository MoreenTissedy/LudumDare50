using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    /// <summary>
    /// Класс используется для покраски объектов в зависимости от зелья в котле
    /// </summary>
    /// 
    public class ColoredByBrewedPotion : MonoBehaviour
    {
        [Range(-1f, 1f)] [SerializeField] private float vibranceModifier = 0f; //Модификатор цвета
        [SerializeField] private float delay = 0f; //Задержка смены цвета в секундах
        [SerializeField] private bool tween = false; //true - менять цвет постепенно, false - мгновенно, 
        [SerializeField] private float tweenTime = 1f; //Длительность(плавность) смены цвета в секундах

        private RecipeProvider recipeProvider;
        private Cauldron cauldron;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ParticleSystem particleSystem;


        [Inject]
        private void Construct(Cauldron cauldron, RecipeProvider recipeProvider)
        {
            this.recipeProvider = recipeProvider;
            this.cauldron = cauldron;
        }

        private void OnValidate()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            particleSystem = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            cauldron.PotionBrewed += DefineColor;
            cauldron.PotionAccepted += ResetColor;
            cauldron.PotionDeclined += PotionDeclined;
        }

        private void OnDisable()
        {
            cauldron.PotionBrewed -= DefineColor;
            cauldron.PotionAccepted -= ResetColor;
        }

        private void PotionDeclined()
        {
            ResetColor(Potions.Placebo);
        }

        /// <summary>
        /// Сброс цвета до нейтрального
        /// </summary>
        /// <param name="potion"></param>
        private void ResetColor(Potions potion)
        {
            Color color = Color.grey;
            if (spriteRenderer)
            {
                color.a = spriteRenderer.color.a;
                StartCoroutine(DoColor(color, spriteRenderer));
            }

            if (particleSystem)
            {
                var particles = particleSystem.main;
                color.a = particles.startColor.color.a;
                StartCoroutine(DoColor(color, particleSystem));
            }
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

            if (spriteRenderer)
            {
                ChangeColor(color, spriteRenderer);
            }

            if (particleSystem)
            {
                ChangeColor(color, particleSystem);
            }
        }

        /// <summary>
        /// Меняем цвет плавно или резко
        /// </summary>
        /// <param name="color"></param>
        /// <param name="component"></param>
        private void ChangeColor(Color color, Component component)
        {
            color = ModifyColor(color);
            if (tween)
            {
                StartCoroutine(DoColor(color, component));
            }
            else
            {
                StartCoroutine(SetColor(color, component));
            }
        }

        /// <summary>
        /// Смешиваем цвет
        /// </summary>
        /// <param name="color"></param>
        private Color ModifyColor(Color color)
        {
            float currentAlpha = 0.5f;
            if (spriteRenderer)
            {
                currentAlpha = spriteRenderer.color.a;
            }

            if (particleSystem)
            {
                currentAlpha = particleSystem.main.startColor.color.a;
            }
            
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
        /// <param name="component"></param>
        private IEnumerator DoColor(Color color, Component component)
        {
            float timer = 0f;
            yield return new WaitForSeconds(delay);

            if (component == spriteRenderer)
            {
                Color startColor = spriteRenderer.color;
                while (timer < tweenTime)
                {
                    timer += Time.deltaTime;
                    float blend = Mathf.Clamp01(timer / tweenTime);
                    Color blendedColor = Color.Lerp(startColor, color, blend);
                    spriteRenderer.color = blendedColor;
                    yield return null;
                }
            }

            if (component == particleSystem)
            {
                Color startColor = particleSystem.main.startColor.color;
                while (timer < tweenTime)
                {
                    timer += Time.deltaTime;
                    float blend = Mathf.Clamp01(timer / tweenTime);
                    Color blendedColor = Color.Lerp(startColor, color, blend);
                    var main = particleSystem.main;
                    main.startColor = blendedColor;
                    yield return null;
                }
            }
        }

        private IEnumerator SetColor(Color color, Component component)
        {
            if (particleSystem)
            {
                var main = particleSystem.main;
                yield return new WaitForSeconds(delay);
                main.startColor = color;
            }

            if (spriteRenderer)
            {
                yield return new WaitForSeconds(delay);
                spriteRenderer.color = color;
            }
        }
    }
}
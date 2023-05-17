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

        private SpriteRenderer spriteRenderer;
        private ParticleSystem particleSystem;
        [SerializeField] private float currentAlpha;


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
            if (spriteRenderer)
            {
                currentAlpha = spriteRenderer.color.a;
            }
            if (particleSystem)
            {
                currentAlpha = particleSystem.main.startColor.color.a;
            }
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
            if (spriteRenderer)
            {
                color.a = spriteRenderer.color.a;
                spriteRenderer.color = color;
            }

            if (particleSystem)
            {
                var particles = particleSystem.main;
                color.a = particles.startColor.color.a;
                particles.startColor = color;
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
            if (component is SpriteRenderer)
            {
                color = ModifyColor(color, spriteRenderer);
                if (tween)
                {
                    StartCoroutine(DoColor(color, component));
                }
                else
                {
                    spriteRenderer.color = color;
                }
            }
            else if (component is ParticleSystem)
            {
                var mainModule = particleSystem.main;
                color = ModifyColor(color, particleSystem);
                if (tween)
                {
                    StartCoroutine(DoColor(color, component));
                }
                else
                {
                    mainModule.startColor = color;
                }
            }
        }


        /// <summary>
        /// Смешиваем цвет
        /// </summary>
        /// <param name="color"></param>
        /// <param name="component"></param>
        private Color ModifyColor(Color color, Component component)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            s += vibranceModifier;
            s = Mathf.Clamp(s, 0f, 1f);
            Color newColor = Color.HSVToRGB(h, s, v);
            if (currentAlpha < color.a)
            {
                newColor.a = currentAlpha;
            }

            if (component is SpriteRenderer)
            {
                spriteRenderer.color = newColor;
            }
            else if (component is ParticleSystem)
            {
                var mainModule = particleSystem.main;
                mainModule.startColor = newColor;
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
            Color startColor = GetStartColor(component);
            yield return new WaitForSeconds(delay);
            while (timer < tweenTime)
            {
                timer += Time.deltaTime;
                float blend = Mathf.Clamp01(timer / tweenTime);
                Color blendedColor = Color.Lerp(startColor, color, blend);
                SetColor(component, blendedColor);
                yield return null;
            }
        }

        private Color GetStartColor(Component component)
        {
            if (component is SpriteRenderer)
            {
                return spriteRenderer.color;
            }

            if (component is ParticleSystem)
            {
                var mainModule = particleSystem.main;
                return mainModule.startColor.color;
            }
            else
            {
                throw new ArgumentException("Component must be a SpriteRenderer or ParticleSystem");
            }
        }

        private void SetColor(Component component, Color color)
        {
            if (component is SpriteRenderer)
            {
                spriteRenderer.color = color;
            }
            else if (component is ParticleSystem)
            {
                var mainModule = particleSystem.main;
                mainModule.startColor = color;
            }
            else
            {
                throw new ArgumentException("Component must be a SpriteRenderer or ParticleSystem");
            }
        }
    }
}
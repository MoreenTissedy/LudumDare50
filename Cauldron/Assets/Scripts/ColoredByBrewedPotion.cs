using System.Collections;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class ColoredByBrewedPotion : MonoBehaviour
    {
        [Tooltip("Color after accepted or declined potion")]
        [SerializeField] private Color neutralColor = Color.gray;
        
        [Tooltip("Color for Placebo potion brewed")]
        [SerializeField] private Color placeboColor = Color.HSVToRGB(0.97f, 0.68f, 0.27f);
        
        [Range(-1f, 1f)] [SerializeField] private float saturation = 0f;
        [Range(-1f, 1f)] [SerializeField] private float brightness = 1f;
        
        [Tooltip("Delay in seconds")]
        [SerializeField] private float delay = 0f; 
        
        [SerializeField] private bool tween = false;  
        
        [Tooltip("Tween time in seconds")]
        [SerializeField] private float tweenTime = 1f; 

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
            if (!tween)
            {
                tweenTime = 0;
            }
        }

        private void Start()
        {
            cauldron.PotionBrewed += DefineColor;
            cauldron.PotionAccepted += ResetColor;
            cauldron.PotionDeclined += PotionDeclined;
        }

        private void OnDestroy()
        {
            cauldron.PotionBrewed -= DefineColor;
            cauldron.PotionAccepted -= ResetColor;
            cauldron.PotionDeclined -= PotionDeclined;
        }

        private void PotionDeclined()
        {
            ResetColor(Potions.Placebo);
        }
        
        private void ResetColor(Potions potion)
        {
            if (spriteRenderer) 
            { 
                StartCoroutine(tween ? DoTweenColor(neutralColor, tweenTime) : SetColor(neutralColor)); 
            } 
            if (particleSystem) 
            { 
                StartCoroutine(tween ? DoTweenColor(neutralColor, tweenTime) : SetColor(neutralColor)); 
            } 
        }
        
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
                color = placeboColor;
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
        
        private void ChangeColor(Color color, Component component)
        {
            color = ModifyColor(color);
            if (tween)
            {
                StartCoroutine(DoTweenColor(color, tweenTime));
            }
            else
            {
                StartCoroutine(SetColor(color));
            }
        }
        
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
            s += saturation;
            s = Mathf.Clamp01(s);
            v += brightness;
            v = Mathf.Clamp01(v);
            Color newColor = Color.HSVToRGB(h, s, v);
            if (currentAlpha < newColor.a)
            {
                newColor.a = currentAlpha;
            }

            return newColor;
        }
        
        private IEnumerator DoTweenColor(Color color, float time)
        {
            float timer = 0f;
            yield return new WaitForSeconds(delay);

            if (spriteRenderer)
            {
                Color startColor = spriteRenderer.color;
                while (timer < time)
                {
                    timer += Time.deltaTime;
                    float blend = Mathf.Clamp01(timer / time);
                    Color blendedColor = Color.Lerp(startColor, color, blend);
                    spriteRenderer.color = blendedColor;
                    yield return null;
                }
            }

            if (particleSystem)
            {
                Color startColor = particleSystem.main.startColor.color;
                while (timer < time)
                {
                    timer += Time.deltaTime;
                    float blend = Mathf.Clamp01(timer / time);
                    Color blendedColor = Color.Lerp(startColor, color, blend);
                    var main = particleSystem.main;
                    main.startColor = blendedColor;
                    yield return null;
                }
            }
        }

        private IEnumerator SetColor(Color color)
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
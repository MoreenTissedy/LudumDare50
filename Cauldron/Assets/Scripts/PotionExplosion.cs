using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class PotionExplosion : MonoBehaviour
    {
        public const float EFFECT_DURATION = 1.5f;
        public GameObject defaultEffect;

        [Serializable]
        public class ExplosionItem
        {
            [HideInInspector] [UsedImplicitly]
            public string label;
            public Potions potion;
            public GameObject effect;
        }

        public ExplosionItem[] effects;
        [SerializeField] RecipeProvider recipes;
        
        [Inject] private Cauldron cauldron;

        private void OnValidate()
        {
            if (effects.Length == 0 && defaultEffect && recipes)
            {
                List<ExplosionItem> list = new List<ExplosionItem>(10);
                foreach (var recipe in recipes.allRecipes)
                {
                    if (recipe.magical)
                    {
                        list.Add(new ExplosionItem()
                        {
                            label = recipe.potion.ToString(),
                                potion = recipe.potion,
                                effect = defaultEffect
                        });
                    }
                }
                effects = list.ToArray();
            }
            foreach (var item in effects)
            {
                item.label = item.potion.ToString();
            }
        }

        private void Start()
        {
            cauldron.PotionBrewed += PlayEffect;
            foreach (var item in effects)
            {
                item.effect.SetActive(false);
            }
        }

        private async void PlayEffect(Potions potion)
        {
            var effect = effects.FirstOrDefault(x => x.potion == potion)?.effect;
            if (effect is null)
            {
                return;
            }
            effect.SetActive(true);
            await Task.Delay(TimeSpan.FromSeconds(EFFECT_DURATION));
            effect.SetActive(false);
        }

        private void OnDestroy()
        {
            cauldron.PotionBrewed -= PlayEffect;
        }
    }
}
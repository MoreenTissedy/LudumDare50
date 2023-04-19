using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class PotionExplosion : MonoBehaviour
    {
        public GameObject defaultEffect;
        
        private GameObject currentEffect;

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

        private void PlayEffect(Potions potion)
        {
            if (currentEffect)
            {
                currentEffect.SetActive(false);
            }
            currentEffect = effects.FirstOrDefault(x => x.potion == potion)?.effect;
            if (currentEffect is null)
            {
                return;
            }
            currentEffect.SetActive(true);
        }

        private void OnDestroy()
        {
            cauldron.PotionBrewed -= PlayEffect;
        }
    }
}
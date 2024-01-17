using System;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace CauldronCodebase
{
    public static class RecipeGenerator
    {
        public static Ingredients[] GenerateRandomRecipe(RecipeBook book)
        {
            Ingredients[] randomRecipe;
            var generatedRecipe = new List<Ingredients[]>(RecipeBook.MAX_COMBINATIONS_COUNT);
            int tryCount = 0;

            do
            {
                randomRecipe = GenerateIngredients();

                if (generatedRecipe.Any(recipe => recipe.All(randomRecipe.Contains)))
                {
                    continue;
                }

                tryCount++;
                generatedRecipe.Add(randomRecipe);

                if (tryCount >= RecipeBook.MAX_COMBINATIONS_COUNT)
                {
                    return null;
                }
            
            } while (book.IsIngredientSetKnown(randomRecipe));
            
            return randomRecipe;
        }

        public static Recipe GenerateLockedRecipe(RecipeBook book)
        {
            return book.LockedRecipes.Count == 0 ? null : book.LockedRecipes[UnityEngine.Random.Range(0, book.LockedRecipes.Count)];
        }

        public static Ingredients[] GenerateLastIngredientRecipe(Ingredients[] mix, RecipeBook book)
        {
            Ingredients[] recipeToTips;
            Ingredients randomIngredient;

            List<Ingredients> allIngredients = Enum.GetValues(typeof(Ingredients)).Cast<Ingredients>().ToList();
            foreach (var ingredientInMix in mix)
            {
                allIngredients.Remove(ingredientInMix);
            }

            int tryCount = allIngredients.Count;
            do
            {
                randomIngredient = allIngredients[UnityEngine.Random.Range(0, allIngredients.Count)];
                recipeToTips = new[] {mix[0], mix[1], randomIngredient};

                tryCount -= 1;
                if (tryCount <= 0) break;
            } while (book.IsIngredientSetKnown(recipeToTips));

            return tryCount > 0 ? recipeToTips : null;
        }

        public static Ingredients[] GenerateCorrectLastIngredientRecipe(Ingredients[] mix, RecipeBook book)
        {
            List<Recipe> potentialRecipes = new List<Recipe>();

            foreach (var recipe in book.LockedRecipes)
            {
                if(mix.All(recipe.RecipeIngredients.Contains))
                {
                    potentialRecipes.Add(recipe);
                }
            }

            if (potentialRecipes.Count > 0)
            {
                return potentialRecipes[UnityEngine.Random.Range(0, potentialRecipes.Count)].RecipeIngredients.ToArray();
            }
            else
            {
                return null;
            }
        }

        private static Ingredients[] GenerateIngredients()
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());
        
            var ingredients = Enum.GetValues(typeof(Ingredients)).Cast<Ingredients>().ToList();

            return ingredients.OrderBy(x => rnd.Next()).Take(3).ToArray();
        }
    }
}
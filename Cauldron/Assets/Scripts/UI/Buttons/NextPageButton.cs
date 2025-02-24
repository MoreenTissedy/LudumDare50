using UnityEngine;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class NextPageButton : MonoBehaviour
    {
        [SerializeField] private FlexibleButton button;
        [SerializeField] private bool isNext = true;
        
        [Inject]
        private RecipeBook book;

        private void Awake()
        {
            if (isNext)
            {
                button.OnClick += book.NextPage;
            }
            else
            {
                button.OnClick += book.PrevPage;
            }
        }

        private void OnDestroy()
        {
            if (isNext)
            {
                button.OnClick -= book.NextPage;
            }
            else
            {
                button.OnClick -= book.PrevPage;
            }
        }
    }
}
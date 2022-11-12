using UnityEngine;
using UnityEngine.EventSystems;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class CloseBookButton : GrowOnMouseEnter
    {
        [SerializeField] private Book book;
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            book.CloseBook();
        }
    }
}
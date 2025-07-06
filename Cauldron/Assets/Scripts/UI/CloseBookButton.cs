using UnityEngine;
using UnityEngine.EventSystems;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    //TODO: adapt for gamepad (auto click rn)
    public class CloseBookButton : GrowOnMouseEnter
    {
        [SerializeField] private Book book;
        //public override void OnPointerClick(PointerEventData eventData)
        //{
        //    base.OnPointerClick(eventData);
        //    book.CloseBook();
        //}
    }
}
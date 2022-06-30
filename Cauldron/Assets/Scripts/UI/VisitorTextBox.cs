using System;
using System.Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class VisitorTextBox : MonoBehaviour
    {
        public float offScreen = -1000;
        public float animTime = 0.5f;
        public TMP_Text text;
        public VisitorTextIcon[] iconObjects = new VisitorTextIcon[3];

        [ContextMenu("Find Icon Objects")]
        private void FindIconObjects()
        {
            iconObjects = GetComponentsInChildren<VisitorTextIcon>();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        } 
        
        public void Display(Encounter card)
        {
            gameObject.SetActive(true);
            gameObject.transform.DOLocalMoveX(gameObject.transform.localPosition.x, animTime)
                .From(offScreen);
            text.text = card.text;
            
            iconObjects[0]?.Display(card.primaryInfluence, card.hidden);
            iconObjects[1]?.Display(card.secondaryInfluence, card.hidden);
            if (card.quest)
            {
                iconObjects[2]?.DisplayItem();
            }
            else
            {
                iconObjects[2]?.Hide();
            }
        }
    }
}
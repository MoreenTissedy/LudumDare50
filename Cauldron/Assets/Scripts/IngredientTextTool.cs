using System;
using EasyLoc;
using UnityEngine;

namespace CauldronCodebase
{
    public class IngredientTextTool : EasyLocTextTool
    {
        private IngredientDroppable script;
        protected override void OnValidate()
        {
            base.OnValidate();
            if (id != String.Empty)
                return;
            script = GetComponentInParent<IngredientDroppable>();
            if (script is null)
                id = "no ingredient found";
            else
                id = script.gameObject.name;
        }

        public override void SetText(string text)
        {
            script = GetComponentInParent<IngredientDroppable>();
            base.SetText(text);
            script.dataList.Get(script.ingredient).friendlyName = text;
        }
    }
}
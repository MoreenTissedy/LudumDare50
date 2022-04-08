using System;
using EasyLoc;
using UnityEngine;

namespace DefaultNamespace
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
            script.dataList.Get(script.type).friendlyName = text;
        }
    }
}
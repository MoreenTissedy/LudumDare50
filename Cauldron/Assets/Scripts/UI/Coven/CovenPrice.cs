using TMPro;
using UnityEngine;

namespace CauldronCodebase
{
    public class CovenPrice: MonoBehaviour
    {
        [SerializeField] private TMP_Text textField;
        [SerializeField] private MainSettings settings;
        
        private void OnEnable()
        {
            textField.text = $"-{settings.statusBars.CovenCost}";
        }
    }
}
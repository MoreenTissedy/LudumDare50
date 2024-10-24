using UnityEngine;
using Zenject;

public class NewGameButton : MonoBehaviour
{
    [SerializeField] private GameObject startGameText;
    [SerializeField] private GameObject newRoundText;

    [Inject] private VillagerFamiliarityChecker villagerChecker;
    
    public void Start()
    {
        UpdateButton();
    }

    public void UpdateButton()
    {
        if(villagerChecker.GetFamiliarityCount == 0)
        {
            startGameText.SetActive(true);
            newRoundText.SetActive(false);
        }
        else
        {
            startGameText.SetActive(false);
            newRoundText.SetActive(true);
        }
    }
}

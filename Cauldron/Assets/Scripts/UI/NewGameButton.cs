using System;
using CauldronCodebase;
using UnityEngine;

public class NewGameButton : MonoBehaviour
{
    [SerializeField] private GameObject startGameText;
    [SerializeField] private GameObject newRoundText;
    
    //Fix me:/
    public void UpdateButton()
    {
        var fileDataHandler = new FileDataHandler<ListToSave<string>>("UnlockedVillager");
        if(!fileDataHandler.IsFileValid())
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

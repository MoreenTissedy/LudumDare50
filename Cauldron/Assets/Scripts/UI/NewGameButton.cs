using System;
using CauldronCodebase;
using UnityEngine;

public class NewGameButton : MonoBehaviour
{
    [SerializeField] private GameObject startGameText;
    [SerializeField] private GameObject newRoundText;
    
    //Fix me:/
    private readonly string fileName = "UnlockedVillager";
    private FileDataHandler<ListToSave<string>> fileDataHandler;

    private void Start()
    {
        fileDataHandler = new FileDataHandler<ListToSave<string>>(fileName);
    }

    public void UpdateButton()
    {
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

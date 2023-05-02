using System;
using Save;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    //This class is only to display interface in Unity inspector
    [Serializable]
    public abstract class EncounterDeckBase : ScriptableObject, IEncounterDeck, IDataPersistence
    { 
        public abstract void Init(GameDataHandler game, DataPersistenceManager dataPersistenceManager, SODictionary dictionary);

        public abstract void NewDayPool(int day);

        public abstract void DealCards(int num);

        public abstract void AddStoryCards();

        public abstract void AddCardToPool(Encounter card);

        public abstract void AddToDeck(Encounter card, bool asFirst = false);

        public abstract Encounter GetTopCard();

        public abstract void LoadData(GameData data, bool newGame);
        public abstract void SaveData(ref GameData data);

    }
}
namespace CauldronCodebase
{
    public interface IDataPersistence
    {
        void LoadData(GameData data, bool newGame);
        void SaveData(ref GameData data);
    }
}
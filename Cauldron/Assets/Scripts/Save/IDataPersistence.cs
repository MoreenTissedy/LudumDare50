namespace Save
{
    public interface IDataPersistence
    {
        void LoadData(GameData data, bool newGame);
        void SaveData(ref GameData data);
    }
}
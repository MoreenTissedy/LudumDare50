namespace CauldronCodebase
{
    public interface IOverlayElement
    {
        void Lock(bool on);

        bool IsLocked();
    }
}
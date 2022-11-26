namespace TandC.RunIfYouWantToLive
{
    public interface IServiceLocator
    {
        T GetService<T>();
        void Update();
    }
}
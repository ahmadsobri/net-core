namespace Net.Core.Authentication.Exceptions.Base
{
    public interface IBaseException
    {
        string GetStatus();
        string GetMessage();

    }
}

namespace MemeOfTheYear.Providers
{
    public interface ILocalStorageProvider
    {
        string ImagePath { get; }

        void MoveFile(string orig, string save);

        Task<string> GetFileContent(string filename);

        Stream OpenRead(string orig);

        Task<T> GetConfigAsync<T>(string configName);

        T GetConfig<T>(string configName);
    }
}
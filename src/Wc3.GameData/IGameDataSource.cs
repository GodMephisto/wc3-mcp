namespace Wc3.GameData;
public interface IGameDataSource : IDisposable
{
    byte[]? ReadFile(string name);
}

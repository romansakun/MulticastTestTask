using Cysharp.Threading.Tasks;

namespace Infrastructure
{
    public interface IAsyncOperation 
    {
        UniTask ProcessAsync();
    }
}
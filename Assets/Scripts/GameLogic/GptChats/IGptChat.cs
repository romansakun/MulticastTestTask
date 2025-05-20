using Cysharp.Threading.Tasks;

namespace GameLogic.GptChats
{
    public interface IGptChat
    {
        UniTask<string> Ask(string question);
    }
}
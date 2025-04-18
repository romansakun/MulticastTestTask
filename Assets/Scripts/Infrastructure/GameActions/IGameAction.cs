using Cysharp.Threading.Tasks;

namespace Infrastructure.GameActions
{
    public interface IGameAction
    {
        public UniTask ExecuteAsync();

        public IValidator GetValidator();
    }
}
using Infrastructure;

namespace GameLogic.UI.CenterMessage
{
    public class CenterMessageViewModel : ViewModel
    {
        public IReactiveProperty<string> Text => _text;
        private readonly ReactiveProperty<string> _text = new ReactiveProperty<string>();

        public override void Initialize()
        {
        }

        public void SetText(string text)
        {
            _text.SetValueAndForceNotify(text);
        }

        public override void Dispose()
        {
            _text.Dispose();
        }
    }
}
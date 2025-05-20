using System;

namespace Infrastructure
{
    public class ReactiveProperty<T>: IReactiveProperty<T>
    {
        private Action<T> _onChanged;
        private T _value;
        private bool _isDisposed;

        public T Value
        {
            get => _value;
            set
            {
                if (value.Equals(_value))
                    return;

                _value = value;
                _onChanged?.Invoke(_value);
            }
        }

        public bool IsDisposed => _isDisposed;

        public ReactiveProperty()
        {
            
        }

        public ReactiveProperty(T value)
        {
            _value = value;
        }

        public void Subscribe(Action<T> action, bool withForceAction = true)
        {
            _onChanged += action;
            if (withForceAction)
                action(_value);
        }

        public void Unsubscribe(Action<T> action)
        {
            _onChanged -= action;
        }

        public void SetValueAndForceNotify(T value)
        {
            _value = value;
            _onChanged?.Invoke(_value);
        }

        public void ForceNotify()
        {
            _onChanged?.Invoke(_value);
        }

        public void Dispose()
        {
            _onChanged = null;
            _value = default;
            _isDisposed = true;
        }

    }
}
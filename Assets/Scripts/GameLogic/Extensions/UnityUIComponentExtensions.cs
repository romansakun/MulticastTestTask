// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System;
using TMPro;

namespace UniRx
{
    public static partial class UnityUIComponentExtensions
    {
        public static IDisposable SubscribeToText(this IObservable<string> source, TextMeshProUGUI text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x);
        }

        public static IDisposable SubscribeToText<T>(this IObservable<T> source, TextMeshProUGUI text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x.ToString());
        }

        public static IDisposable SubscribeToText<T>(this IObservable<T> source, TextMeshProUGUI text, Func<T, string> selector)
        {
            return source.SubscribeWithState2(text, selector, (x, t, s) => t.text = s(x));
        }
    }
}

#endif
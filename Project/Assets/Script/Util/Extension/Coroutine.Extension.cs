using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Script.Util.Extension
{
    public static class CoroutineExtension
    {
        public static Task AsTask(this IEnumerator enumerator, MonoBehaviour owner)
        {
            var source = new TaskCompletionSource<bool>();
            owner.StartCoroutine(Wrap(enumerator, source));

            return source.Task;
        }

        private static IEnumerator Wrap(this IEnumerator enumerator, TaskCompletionSource<bool> source)
        {
            yield return enumerator;

            source.TrySetResult(true);
        }
    }
}

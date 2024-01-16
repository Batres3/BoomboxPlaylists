using UnityEngine;

namespace BoomboxPlaylists.Utils
{
    internal class SharedCoroutineStarter : MonoBehaviour
    {
        private static SharedCoroutineStarter _instance;

        public new static Coroutine StartCoroutine(System.Collections.IEnumerator routine)
        {
            if (_instance == null)
            {
                _instance = new GameObject("Shared Coroutine Starter").AddComponent<SharedCoroutineStarter>();
                DontDestroyOnLoad(_instance);
            }
            return ((MonoBehaviour)_instance).StartCoroutine(routine);
        }
    }
}

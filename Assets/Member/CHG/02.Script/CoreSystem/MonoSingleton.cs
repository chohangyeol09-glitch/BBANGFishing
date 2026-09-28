using UnityEngine;

namespace CHG._02.Script.CoreSystem
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        [SerializeField] private bool isDonDestroy = false;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (isDonDestroy)
                    DontDestroyOnLoad(this.gameObject);
            }
            else if (_instance != this)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
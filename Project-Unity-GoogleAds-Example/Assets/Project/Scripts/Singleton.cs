using UnityEngine;

namespace CTJ
{
    public class Singleton<T> : MonoBehaviour where T : class, new()
    {
        private static T _Instance;

        private static readonly object _Locker = new object();

        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_Locker)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new T();
                            Logger.LogFormat("創建單利對象：{0}", typeof(T).Name);
                        }
                    }
                }
                return _Instance;
            }
        }
    }
}

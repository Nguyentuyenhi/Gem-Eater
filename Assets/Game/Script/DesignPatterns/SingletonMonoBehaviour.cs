using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
                // if (instance == null)
                // {
                //     GameObject singletonObject = new GameObject(typeof(T).Name);
                //     instance = singletonObject.AddComponent<T>();
                // }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        CreateInstance();
    }
    protected virtual void CreateInstance(bool destroyInload = true)
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as T;
            if (!destroyInload)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
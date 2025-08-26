using UnityEngine;

public abstract class SingletonSO<T> : ScriptableObject where T : SingletonSO<T>
{
    private static T _instance;
    
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<T>(typeof(T).Name);
                
                if (_instance == null)
                {
                    Debug.LogError($"No instance of {typeof(T).Name} found in Resources folder!");
                }
            }
            return _instance;
        }
    }
    
}
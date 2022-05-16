using UnityEngine;
/// <summary>
/// class realizing a pattern Singlton for MonoBehaviour classes
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            return instance;
        }

        protected set
        {
            if (value == null)
            {
                instance = null;
            }
            else
            {
                if (instance == null)
                {
                    instance = value;
                }
                else if (value != instance)
                {
                    Destroy(value);
                }
            }
        }
    }

    protected virtual void Awake()
    {
        Instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        Instance = null;
    }
}
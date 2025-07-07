using UnityEngine;

public class Singleton<T>: MonoBehaviour where T : MonoBehaviour
{

    [SerializeField] private bool _presistThroughScenes = false;
    public static T Instance { get; private set; }



    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;

            if (_presistThroughScenes)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);

            return;
        }
    }


}

using UnityEngine;

public class AmbianceObject : MonoBehaviour
{



    private static AmbianceObject _instance;

    public static AmbianceObject Instance { get { return _instance; } }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}

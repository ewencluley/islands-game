using UnityEngine;

public class Sea : MonoBehaviour
{
    public static Sea Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
    }

    public float GetSeaLevel()
    {
        return transform.position.y;
    }
}
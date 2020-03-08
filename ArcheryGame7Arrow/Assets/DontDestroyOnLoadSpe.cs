using UnityEngine;

public class DontDestroyOnLoadSpe : MonoBehaviour
{
    
    private void Awake()
    {
        DontDestroyOnLoad( this.gameObject );
    }
}
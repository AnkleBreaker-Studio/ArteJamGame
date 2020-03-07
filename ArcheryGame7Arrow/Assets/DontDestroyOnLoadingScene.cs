using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadingScene : MonoBehaviour
{
    private static DontDestroyOnLoadingScene instance = null;
     
    // Game Instance Singleton
    public static DontDestroyOnLoadingScene Instance
    {
        get
        { 
            return instance; 
        }
    }
     
    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this) 
        {
            Destroy(this.gameObject);
        }
 
        instance = this;
        DontDestroyOnLoad( this.gameObject );
    }
}

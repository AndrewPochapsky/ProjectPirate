using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour {

    public static World worldInstance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if(worldInstance == null)
        {
            worldInstance = this;
        }
        else
        {
            DestroyObject(gameObject);
        }
    }
}

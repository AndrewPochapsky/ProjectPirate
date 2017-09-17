using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour {

    private static World worldInstance;

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
    
    public void ToggleWorld(bool value)
    {
        this.gameObject.SetActive(value);
    }




}

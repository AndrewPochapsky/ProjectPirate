using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour {

    public static World Instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if(Instance != null && Instance != this){
			Destroy(gameObject);
		}else{
			Instance = this;
		}
    }
}

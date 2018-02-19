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

    /// <summary>
    /// Sets the world container's transform in such a way that makes (0,0) be the middle of it.
    /// </summary>
    public void SetPositionRelativeToOrigin()
    {
        int position = (WorldController.numberOfChunks/2) * WorldController.chunkSize * WorldController.Instance.newSize;

        transform.position = new Vector3(-position, 0, -position);
    }
}

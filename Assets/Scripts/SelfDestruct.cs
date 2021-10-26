using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{

    public  float timeToDestroy;
    public float destructionThreshold;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeToDestroy+=Time.deltaTime;
        if(timeToDestroy>destructionThreshold){
            GameObject.Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfNoChild : MonoBehaviour
{
    
    private float floatTimer;
    // Start is called before the first frame update
    void Start()
    {
        floatTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if( transform.childCount > 0 )
        {
            floatTimer = 0f;
        }
        else
        {
            floatTimer += Time.deltaTime;
        }

        if(floatTimer > 3f )
        {
            Destroy(gameObject);
        }
    }
}

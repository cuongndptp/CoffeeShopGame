using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAnimationStartMenu : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 50f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateInYAxis();
    }

    private void RotateInYAxis()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}

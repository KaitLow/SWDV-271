using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    
    [SerializeField] Vector3 movementVector = new Vector3(20, -15, 0);
    [SerializeField] float period = 2f; // default period id 2 seconds
    [Range(0, 1)] // you can have more than one attribute on a variable
    float movementFactor;
    Vector3 destinationVector;

    Vector3 startingPos;
    float movementStepsize = 0.2f;
    bool stepUp = true;


    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // set movement factor
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2f;
        float rawSineWave = Mathf.Sin(cycles * tau);
        movementFactor = rawSineWave / 2f + 0.5f;
        destinationVector = startingPos + movementVector * movementFactor;
        transform.position = destinationVector;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;
    public bool isStop = false;


    // todo remove from inspector later
    [Range(0, 1)]
    [SerializeField]
    float movementFactor; // 0 for not moved, 1 for fully moved.

    Vector3 startingPos; 

    void Start()
    {
        startingPos = transform.position;
    }

    void Update()
    {

        if (period <= Mathf.Epsilon || isStop) { return; }

        float cycles = Time.time / period; // grows from 0

        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave; // from -1 to 1
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }

    public void Stop()
    {
        isStop = true;
    }
}

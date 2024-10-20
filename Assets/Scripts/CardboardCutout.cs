using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardboardCutout : MonoBehaviour
{
    Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        transform.forward = -mainCamera.transform.forward;
    }
}

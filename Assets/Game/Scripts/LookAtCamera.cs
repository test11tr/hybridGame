using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public bool inverted;

    Camera _camera;

    void EnsureCamera()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }
    }

    protected void Update()
    {
        EnsureCamera();
        if (inverted)
        {
            transform.forward = _camera.transform.forward;
            return;
        }

        transform.forward = -_camera.transform.forward;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class DiscShapeVisualization : MonoBehaviour {
    public float dashSpeed = 10;
    public Disc outLineDisc;

    public Vector3 offset = new Vector3(0, 0.2f, 0);

    private void Update() {
        outLineDisc.DashOffset += Time.deltaTime * dashSpeed;
    }
}

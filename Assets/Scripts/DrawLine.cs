using UnityEngine;
using System.Collections;


public class DrawLine : MonoBehaviour {
    LineRenderer line;

	// Use this for initialization
	void Awake () {
        if (GetComponent<LineRenderer>() == null)
            line = gameObject.AddComponent<LineRenderer>();
        else
            line = GetComponent<LineRenderer>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

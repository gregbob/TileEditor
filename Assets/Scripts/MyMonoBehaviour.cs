using UnityEngine;
using System.Collections;

public class MyMonoBehaviour : MonoBehaviour {


    public void SetX(float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
    public void SetY(float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
    public void SetZ(float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }

}

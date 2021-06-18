using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotationScript : MonoBehaviour
{
    private Quaternion rotation;
    private Vector3 forward;
    public float tSeconds = 2.0f;
    public float tDelta = 0f;

    private Transform parentTransform;
    // Start is called before the first frame update
    void Start()
    {
        rotation = transform.rotation;
        forward = transform.forward;
        parentTransform = GetComponentInParent<Transform>();
    }
    
    void Update()
    {
     //   transform.forward = forward;
       // tDelta += Time.deltaTime / tSeconds;
       // transform.forward = Vector3.Lerp(transform.forward, GetComponentInParent<Transform>().forward, tDelta);
       // forward = transform.forward;
    }

    internal void UpdateTime()
    {
        Vector3 newRotation = Vector2.zero;
        transform.rotation = rotation;
        //transform.forward = Vector3.SmoothDamp(
            //transform.forward, parentTransform.forward, ref velocity, 0.001f);
        //transform.rotation = new Quaternion();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    float Speed=2;
    [SerializeField]
    float WheelSpeed;
    float ZoomSpeed = 5;
    Camera thiscamera;
    private void Awake()
    {
        thiscamera = GetComponent<Camera>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal") * Speed, Input.GetAxis("Vertical") * Speed, 0);
        WheelSpeed=Input.GetAxis("Mouse ScrollWheel")* ZoomSpeed*-1;
        // camera.fieldOfView += WheelSpeed;
        // thiscamera.fieldOfView=Mathf.Clamp(thiscamera.fieldOfView += WheelSpeed, 1, 10);
        // thiscamera.fieldOfView += WheelSpeed;
        //transform.position += new Vector3(0, 0, WheelSpeed);
        if (thiscamera.orthographic)
        {
            thiscamera.orthographicSize = Mathf.Clamp(thiscamera.orthographicSize + WheelSpeed, 7, 10);
        }
        else
        {
            thiscamera.fieldOfView = Mathf.Clamp(thiscamera.fieldOfView += WheelSpeed, 15, 60);
        }
    }
}

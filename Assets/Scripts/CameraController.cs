using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementTime;

    [SerializeField] private float zoomAmount;
    [SerializeField] private Vector3 newPosition;
    private float newZoom;
    void Start()
    {
        newPosition = transform.position;
        newZoom = Camera.main.orthographicSize;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            newPosition += (Vector3.left * movementSpeed); 
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPosition += (Vector3.right * movementSpeed); 
        }
        if (Input.GetKey(KeyCode.W))
        {
            newPosition += (Vector3.up * movementSpeed); 
        }
        if (Input.GetKey(KeyCode.S))
        {
            newPosition += (Vector3.down * movementSpeed); 
            
        }
        //newPosition += Ve
        //newPosition.x = Mathf.Max(0,Mathf.Min(newPosition.x, 90));
        //newPosition.z = Mathf.Min(-20, Mathf.Max(newPosition.z, -130));
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
   
        newZoom -= zoomAmount * Input.mouseScrollDelta.y;
        Camera.main.orthographicSize = newZoom;
        //if(newZoom.y < 10 || newZoom.y > 70) newZoom -= zoomAmount * Input.mouseScrollDelta.y;
        //Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}

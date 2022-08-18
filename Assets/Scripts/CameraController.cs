using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Script to handle input for moving the camera and zooming to enable the player to see mazes of varying sizes as
/// clear as possible. 
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementTime;
    [SerializeField] private float zoomAmount;
    [SerializeField] private float zoomTime;
    [Header("Setup Fields")]
    [SerializeField] private Vector3 newPosition;
    [SerializeField] private Camera camera;
    private float newZoom;
    
    void Start()
    {
        newPosition = transform.position;
        newZoom = camera.orthographicSize;
    }
    
    void Update()
    {
        ApplyInput();
    }
    /// <summary>
    /// Handles the input by the player.
    /// In the first step a new position is adding all the directions given by the buttons pressed
    /// are added together - this enables diagonal movement.
    /// in the second the actual position of the camera is calculating by linearly interpolating between the
    /// old and the new value based on the speed selected in the editor
    /// </summary>
    private void ApplyInput()
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
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        newZoom -= zoomAmount * Input.mouseScrollDelta.y;
        newZoom = Mathf.Clamp(newZoom, 2, 200);
        camera.orthographicSize = Mathf.Lerp(newZoom, camera.orthographicSize, Time.deltaTime * movementTime); 
    }
}

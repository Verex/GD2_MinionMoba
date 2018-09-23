using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] public Vector3[] startPositions;
    [SerializeField] private float moveTime = 1.0f;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float zoomSpeed = 1.0f;
    [SerializeField] private float maxSpeed = 1.0f;
    
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;
    private int zoomDirection = 0;

    public void SetZoomDirection(int direction)
    {
        zoomDirection = direction;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }

    void Start()
    {

    }

    void Update()
    {
        // Check if we have assigned move direction.
        if (moveDirection != Vector3.zero || zoomDirection != 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position,
                transform.position + (moveDirection * moveSpeed) + (transform.forward * zoomDirection * zoomSpeed), 
                ref currentVelocity, moveTime, maxSpeed);
        }
        else
        {
            currentVelocity = Vector3.zero;
        }
    }
}

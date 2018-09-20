using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float moveTime = 1.0f;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float maxSpeed = 1.0f;
    [SerializeField] private Vector3 moveDirection = Vector3.zero;

    private Vector3 currentVelocity = Vector3.zero;

    void Start()
    {

    }

    void FixedUpdate()
    {

    }

    void Update()
    {
        // Check if we have assigned move direction.
        if (moveDirection != Vector3.zero)
        {
            transform.position = Vector3.SmoothDamp(transform.position,
                transform.position + (moveDirection * moveSpeed), ref currentVelocity, 0.2f, 1.0f);
        }
    }
}

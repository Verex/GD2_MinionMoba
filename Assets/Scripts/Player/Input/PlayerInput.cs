using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerActions actions;

    new private PlayerCamera camera;
    [SerializeField] private float updateDelay = 0.0f;

    private IEnumerator InputUpdate()
    {
        while (true)
        {
            // Get input move direction.
            Vector2 move = actions.Move;

            camera.SetMoveDirection(new Vector3(move.y, 0, -move.x));

            yield return new WaitForSeconds(updateDelay);
        }
    }

    void Start()
    {
        // Get player camera component.
        camera = GetComponent<PlayerCamera>();

        // Setup player input actions.
        actions = PlayerActions.CreateWithDefaultBindings();

        // Start input update coroutine.
        StartCoroutine(InputUpdate());
    }
}

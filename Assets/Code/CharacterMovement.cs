using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float MoveSpeed = 3f;

    private CharacterController controller;

    private Vector3 moveDirection = Vector3.zero;
    private Vector2 inputvector = Vector2.zero;

    private Vector2 currentInputVector = Vector2.zero;
    private Vector2 smoothInputVelocity = Vector2.zero;
    private float smoothInputSpeed = 0.1f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }

    /// <summary>
    /// Moves the player
    /// </summary>
    private void Move()
    {
        // Smooth rotation
        currentInputVector = Vector2.SmoothDamp(currentInputVector, inputvector, ref smoothInputVelocity, smoothInputSpeed);
        //Rotation
        if (moveDirection != Vector3.zero)
        {
            gameObject.transform.forward = moveDirection;
        }

        // Move player
        moveDirection = new Vector3(currentInputVector.x, 0, currentInputVector.y);
        moveDirection *= MoveSpeed;

        controller.Move(moveDirection * Time.deltaTime);
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        inputvector = value.ReadValue<Vector2>();
    }
}

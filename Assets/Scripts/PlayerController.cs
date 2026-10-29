using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private Animator animator;
    private Vector2 moveInput;
    private Camera mainCamera;

    public bool IsMoving { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();      
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    //private void Update()
    //{
    //    Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
    //    Plane plane = new Plane(Vector3.up, transform.position);

    //    if (plane.Raycast(ray, out float distance))
    //    {
    //        Vector3 targetPoint = ray.GetPoint(distance);
    //        Vector3 direction = (targetPoint - transform.position).normalized;
    //        direction.y = 0f; 

    //        if (direction.sqrMagnitude > 0.1f)
    //        {
    //            Quaternion targetRotation = Quaternion.LookRotation(direction);
    //            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    //        }
    //    }
    //}

    void FixedUpdate()
    {
        Vector3 move = mainCamera.transform.forward * moveInput.y + mainCamera.transform.right * moveInput.x;
        move.y = 0f; 

        if (move.sqrMagnitude > 1f) move.Normalize();
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);

        if(move.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        float speed = move.magnitude;
        animator.SetFloat("Speed", speed);

        IsMoving = speed > 0.1f;
    }
}
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool Activo { set; get; } = true;
    private Input inputPlayer;
    private Vector2 movement;
    private Rigidbody rigidBody;
    private Animator animator;
    private float speed;
    public bool IsCrossing { set; get; } = false;
    private int indicator = 1;
    public void Speed(float movement, float animation)
    {
        speed = movement;
        animator.speed = animation;
    }
    [SerializeField] private float rotationSpeed = 10f;
    void Awake()
    {
        inputPlayer = new Input();
        inputPlayer.Player.Move.Enable();
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movement = inputPlayer.Player.Move.ReadValue<Vector2>();
        indicator = IsCrossing ? -1 : 1;
    }

    void FixedUpdate()
    {
        if (Activo)
        {
            animator.SetBool("Stunned", false);
            Mover();
            Rotar();
        }
        else
        {
            animator.SetBool("Stunned", true);
            rigidBody.linearVelocity = Vector3.zero;
        }
    }

    private void Mover()
    {
        Vector3 direccion = new Vector3(movement.x * indicator, 0, movement.y * indicator).normalized * speed;
        rigidBody.linearVelocity = new Vector3(direccion.x, rigidBody.linearVelocity.y, direccion.z);
        animator.SetBool("Walk", direccion.magnitude != 0);
    }

    private void Rotar()
    {
        Vector3 direccion = new Vector3(movement.x * indicator, 0, movement.y * indicator).normalized;
        if (direccion.magnitude > 0.1f)
        {
            // Crear la rotación hacia donde miramos
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    void OnDisable()
    {
        inputPlayer.Player.Move.Disable();
    }
}

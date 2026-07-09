using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool Activo { set; get; } = true;
    private Input inputPlayer;
    private Vector2 entradaMovimiento;
    private Rigidbody rigidBody;
    private Animator animator;
    [SerializeField] private float velocidadMovimiento = 3f;
    [SerializeField] private float suavizadoRotacion = 10f;
    void Start()
    {
        inputPlayer = new Input();
        inputPlayer.Player.Move.Enable();
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        entradaMovimiento = inputPlayer.Player.Move.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (Activo)
        {
            Mover();
            Rotar();
        }
    }

    private void Mover()
    {
        Vector3 direccion = new Vector3(entradaMovimiento.x, 0, entradaMovimiento.y).normalized * velocidadMovimiento;
        rigidBody.linearVelocity = new Vector3(direccion.x, rigidBody.linearVelocity.y, direccion.z);
        animator.SetBool("Walk", direccion.magnitude != 0);
    }

    private void Rotar()
    {
        Vector3 direccion = new Vector3(entradaMovimiento.x, 0, entradaMovimiento.y).normalized;
        if (direccion.magnitude > 0.1f)
        {
            // Crear la rotación hacia donde nos movemos
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, suavizadoRotacion * Time.fixedDeltaTime);
        }
    }

    void OnDisable()
    {
        inputPlayer.Player.Move.Disable();
    }
}

using UnityEngine;
using UnityEngine.InputSystem; // ¡Esta línea es súper importante!

public class MovimientoJugador : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 5f;
    public InputAction movimientoInput;

    [Header("Configuración de Disparo")]
    public InputAction dispararInput; // NUEVO: Acción para el espacio
    public GameObject prefabProyectil; // NUEVO: El prefab de tu bala
    public float velocidadProyectil = 12f; // NUEVO: Velocidad de la bala

    // NUEVO: Guarda hacia dónde miraba el jugador (empieza mirando hacia el frente/norte)
    private Vector3 ultimaDireccion = Vector3.forward;

    private void OnEnable()
    {
        movimientoInput.Enable();
        dispararInput.Enable(); // NUEVO: Encendemos el espacio
    }

    private void OnDisable()
    {
        movimientoInput.Disable();
        dispararInput.Disable(); // NUEVO: Apagamos el espacio
    }

    void Update()
    {
        // 1. Leemos el valor de las teclas (nos da un vector 2D con X y Y)
        Vector2 entrada = movimientoInput.ReadValue<Vector2>();

        // 2. Convertimos ese 2D a 3D (la 'Y' de las teclas se vuelve la 'Z' de profundidad)
        Vector3 direccion = new Vector3(entrada.x, 0f, entrada.y);

        // 3. Mover al jugador
        transform.Translate(direccion * velocidad * Time.deltaTime, Space.World);

        // NUEVO: Si el jugador se está moviendo, actualizamos su dirección y lo rotamos
        if (direccion != Vector3.zero)
        {
            ultimaDireccion = direccion.normalized;

            // Esto hace que el modelo del jugador mire hacia donde camina automáticamente
            transform.forward = ultimaDireccion;
        }

        // NUEVO: Detectar si se presionó la barra espaciadora
        if (dispararInput.triggered)
        {
            Disparar();
        }
    }

    void Disparar()
    {
        if (prefabProyectil != null)
        {
            // Creamos la bala en la posición del jugador, pero rotada hacia la ULTIMA DIRECCIÓN guardada
            Instantiate(prefabProyectil, transform.position, Quaternion.LookRotation(ultimaDireccion));
            Debug.Log("¡Jugador disparó hacia: " + ultimaDireccion + "!");
        }
        else
        {
            Debug.LogWarning("¡Te falta asignar el Prefab Proyectil en el script del Jugador!");
        }
    }
}
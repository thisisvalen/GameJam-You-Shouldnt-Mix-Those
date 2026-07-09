using UnityEngine;
using UnityEngine.InputSystem; // ¡Esta línea es súper importante!

public class MovimientoJugador : MonoBehaviour
{
    public float velocidad = 5f;

    // Esta variable aparecerá en Unity para que configures las teclas ahí mismo
    public InputAction movimientoInput;

    // El nuevo sistema requiere que "encendamos" y "apaguemos" los controles
    private void OnEnable()
    {
        movimientoInput.Enable();
    }

    private void OnDisable()
    {
        movimientoInput.Disable();
    }

    void Update()
    {
        // 1. Leemos el valor de las teclas (nos da un vector 2D con X y Y)
        Vector2 entrada = movimientoInput.ReadValue<Vector2>();

        // 2. Convertimos ese 2D a 3D (la 'Y' de las teclas se vuelve la 'Z' de profundidad)
        Vector3 direccion = new Vector3(entrada.x, 0f, entrada.y);

        // 3. Movemos al jugador
        transform.Translate(direccion * velocidad * Time.deltaTime, Space.World);
    }
}
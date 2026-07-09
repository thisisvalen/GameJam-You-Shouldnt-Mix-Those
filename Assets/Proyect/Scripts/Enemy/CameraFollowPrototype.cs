using UnityEngine;

public class SeguirJugador : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform jugador; // Aquí vas a arrastrar a tu jugador

    [Header("Configuración")]
    [Tooltip("Distancia/Altura a la que se mantendrá la cámara del jugador")]
    public Vector3 distanciaOffset = new Vector3(0f, 10f, -6f);

    // Usamos LateUpdate para cámaras porque se ejecuta después del movimiento del jugador
    // Esto evita que la cámara "tiemble" cuando el jugador corre
    void LateUpdate()
    {
        if (jugador == null) return;

        // La cámara se mueve a la posición del jugador más la distancia que elegimos
        transform.position = jugador.position + distanciaOffset;
    }
}
using UnityEngine;

public class EnemigoPersigue : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 3f;
    public float rangoDeteccion = 5f;

    private Transform transformJugador;
    private Vector3 posicionInicial;

    void Start()
    {
        // Guarda el punto exacto donde empezó el juego
        posicionInicial = transform.position;

        // Busca automáticamente al objeto con la etiqueta "Player"
        GameObject jugador = GameObject.FindWithTag("Player");

        if (jugador != null)
        {
            transformJugador = jugador.transform;
        }
        else
        {
            Debug.LogWarning("¡Ojo! No encontré ningún objeto con la etiqueta 'Player' en la escena.");
        }
    }

    void Update()
    {
        // Si el jugador no existe (o fue destruido), no hace nada
        if (transformJugador == null) return;

        // Calcula la distancia matemática entre el enemigo y el jugador
        float distanciaAlJugador = Vector3.Distance(transform.position, transformJugador.position);

        if (distanciaAlJugador <= rangoDeteccion)
        {
            // Si está cerca, muévete hacia el jugador
            MoverHacia(transformJugador.position);
        }
        else
        {
            // Si se alejó, regresa a la posición inicial
            MoverHacia(posicionInicial);
        }
        // --- CÓDIGO PARA MIRAR AL JUGADOR ---
        // 1. Calculamos la dirección hacia el jugador
        Vector3 direccionMirada = transformJugador.position - transform.position;

        // 2. Anulamos la altura (Y) para que la calavera no cabecee hacia arriba o abajo
        direccionMirada.y = 0f;

        // 3. Si hay una dirección válida, rotamos al enemigo hacia allá
        if (direccionMirada != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direccionMirada);
        }
    }

    void MoverHacia(Vector3 objetivo)
    {
        // Esta línea calcula el movimiento cuadro por cuadro hacia el objetivo
        transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);
    }

    // Truco de Game Jam: Esto dibuja una esfera roja en la pestaña 'Scene' para que veas el rango real de tu enemigo
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}
using UnityEngine;

public class ProyectilEnemigo : MonoBehaviour
{
    [Header("Parámetros del Proyectil")]
    public float velocidad = 8f;
    public float tiempoDeVida = 3f; // Tiempo máximo antes de borrarse sola
    public int daño = 10;

    void Start()
    {
        // LA REGLA DE ORO DE LA EFICIENCIA:
        // En cuanto la bala nace, le programamos su muerte. 
        // Si no choca con nada en 3 segundos, se borra sola para no alentar el juego.
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        // Mueve la bala hacia adelante basándose en la dirección a la que apuntaba el enemigo
        transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
    }

    void OnTriggerEnter(Collider otro)
    {
        // 1. Si choca contra el Jugador
        if (otro.CompareTag("Player"))
        {
            Debug.Log("¡Alerta! El jugador fue impactado por un proyectil.");

            // Cuando tu compañero tenga la vida del jugador, descomentan esto:
            // otro.GetComponent<VidaJugador>().RecibirDaño(daño);

            Destroy(gameObject); // Destruye la bala inmediatamente
        }

        // 2. Si choca contra una pared u obstáculo del escenario
        // (Asegúrate de que tus paredes tengan la etiqueta "Pared" o "Obstaculo")
        if (otro.CompareTag("Pared") || otro.CompareTag("Obstaculo"))
        {
            Destroy(gameObject); // La bala se desvanece al impactar el muro
        }
    }
}
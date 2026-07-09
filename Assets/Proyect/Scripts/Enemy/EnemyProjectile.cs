using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Parámetros del Proyectil")]
    public float velocidad = 8f;
    public float tiempoDeVida = 3f;
    public int daño = 10;

    void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
    }

    void OnTriggerEnter(Collider otro)
    {
        // 1. Si choca contra el Jugador
        if (otro.CompareTag("Player"))
        {
            // ¡LÍNEAS ACTIVADAS! Buscamos la vida del jugador y le aplicamos el daño
            VidaJugador vida = otro.GetComponent<VidaJugador>();
            if (vida != null)
            {
                vida.RecibirDaño(daño);
            }

            Destroy(gameObject); // La bala se destruye tras impactar
        }

        // 2. Si choca contra una pared u obstáculo
        if (otro.CompareTag("Pared") || otro.CompareTag("Obstaculo"))
        {
            Destroy(gameObject);
        }
    }
}
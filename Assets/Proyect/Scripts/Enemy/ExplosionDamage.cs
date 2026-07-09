using UnityEngine;

public class ExplosionDaño : MonoBehaviour
{
    [Header("Configuración del Daño")]
    public int daño = 20;

    [Header("Configuración del Efecto Visual")]
    public float tiempoDeVida = 10f;
    public float tamanoMaximo = 30f;

    private Vector3 escalaInicial;
    private Vector3 escalaFinal;
    private float tiempoTranscurrido = 0f;

    void Start()
    {
        // 1. Guardamos el tamaño pequeñito inicial
        escalaInicial = transform.localScale;

        // 2. Calculamos el tamaño gigante final
        escalaFinal = new Vector3(tamanoMaximo, tamanoMaximo, tamanoMaximo);

        // 3. Programamos la destrucción automática de la esfera
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        // 4. Animación: Hacemos que la esfera crezca suavemente cada frame
        tiempoTranscurrido += Time.deltaTime;
        float porcentaje = tiempoTranscurrido / tiempoDeVida;
        transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, porcentaje);
    }

    void OnTriggerEnter(Collider otro)
    {
        // 5. Daño: Si toca al jugador mientras está creciendo...
        if (otro.CompareTag("Player"))
        {
            Debug.Log("¡BOOM! El jugador recibió daño de la explosión.");

            // Cuando el Programador A tenga listo el script de vida, descomentas esta línea:
            // otro.GetComponent<VidaJugador>().RecibirDaño(daño);
        }
    }
}
using UnityEngine;

public class ExplosionDaño : MonoBehaviour
{
    [Header("Configuración del Daño")]
    public int daño = 50;

    [Header("Configuración del Efecto Visual")]
    public float tiempoDeVida = 0.5f;
    public float tamanoMaximo = 4f;

    private Vector3 escalaInicial;
    private Vector3 escalaFinal;
    private float tiempoTranscurrido = 0f;

    void Start()
    {
        escalaInicial = transform.localScale;
        escalaFinal = new Vector3(tamanoMaximo, tamanoMaximo, tamanoMaximo);
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        tiempoTranscurrido += Time.deltaTime;
        float porcentaje = tiempoTranscurrido / tiempoDeVida;
        transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, porcentaje);
    }

    void OnTriggerEnter(Collider otro)
    {
        // Si toca al jugador mientras está creciendo...
        if (otro.CompareTag("Player"))
        {
            // ¡LÍNEAS ACTIVADAS!
            PlayerStats jugador = otro.GetComponent<PlayerStats>();
            if (jugador != null)
            {
                jugador.RecibirDaño(daño);
            }
        }
    }
}
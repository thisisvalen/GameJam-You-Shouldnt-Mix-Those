using UnityEngine;

public class ProyectilJugador : MonoBehaviour
{
    public float velocidadBala = 12f;

    void Start()
    {
        Destroy(gameObject, 2f); // Se borra sola en 2 segundos para ahorrar memoria
    }

    void Update()
    {
        // Como al nacer la bala el jugador le dio su misma rotación, 
        // solo necesita avanzar hacia adelante de sí misma.
        transform.Translate(Vector3.forward * velocidadBala * Time.deltaTime);
    }

    void OnTriggerEnter(Collider otro)
    {
        // Si choca con un enemigo...
        if (otro.CompareTag("Enemigo"))
        {
            // Buscamos el script del enemigo en el objeto que tocamos
            EnemyFollow3 enemigo = otro.GetComponent<EnemyFollow3>();

            if (enemigo != null)
            {
                enemigo.RecibirDaño(10); // Le resta 10 de vida (puedes cambiar este número)
            }

            Destroy(gameObject); // Destruye la bala inmediatamente
        }
    }
}
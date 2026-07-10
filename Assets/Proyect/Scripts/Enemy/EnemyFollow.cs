using UnityEngine;
using System.Collections;

public class EnemyFollow3 : MonoBehaviour
{
    public enum TipoComportamiento { Estandar, Kamikaze, Cobarde }

    [Header("Personalidad")]
    public TipoComportamiento comportamiento = TipoComportamiento.Estandar;

    [Header("Vida y Combate")]
    public int vidaMaxima = 50;
    private int vidaActual;
    public int dañoPorContacto = 10; // NUEVO: Cuánto daño hace al tocar al jugador

    [Header("Movimiento")]
    public float velocidad = 3f;
    public float rangoDeteccion = 6f;

    [Header("Parámetros de Cobarde (Tirador)")]
    public GameObject prefabProyectil;
    public float tiempoEntreDisparos = 2f;
    private float temporizadorDisparo = 0f;
    public float distanciaSegura = 6f;

    [Header("Parámetros de Kamikaze")]
    public float rangoExplosion = 1.5f;
    public GameObject prefabExplosion;
    public Color colorAlerta = Color.red;

    private Transform transformJugador;
    private Vector3 posicionInicial;
    private Renderer miRenderer;
    private Color colorOriginal;
    private bool estaExplotando = false;
    private bool estaParpadeandoPorGolpe = false;

    void Start()
    {
        posicionInicial = transform.position;
        vidaActual = vidaMaxima;

        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador != null)
        {
            transformJugador = jugador.transform;
        }

        miRenderer = GetComponentInChildren<Renderer>();
        if (miRenderer != null)
        {
            colorOriginal = miRenderer.material.color;
        }
    }

    void Update()
    {
        if (transformJugador == null) return;

        Vector3 posJugadorPlana = new Vector3(transformJugador.position.x, transform.position.y, transformJugador.position.z);
        float distanciaAlJugador = Vector3.Distance(transform.position, posJugadorPlana);

        if (distanciaAlJugador <= rangoDeteccion)
        {
            EjecutarComportamiento(distanciaAlJugador);
        }
        else
        {
            if (!estaExplotando)
            {
                MoverHacia(posicionInicial);
            }
        }

        RotarHaciaJugador();
    }

    void EjecutarComportamiento(float distancia)
    {
        switch (comportamiento)
        {
            case TipoComportamiento.Estandar:
                MoverHacia(transformJugador.position);
                break;

            case TipoComportamiento.Kamikaze:
                if (distancia <= rangoExplosion && !estaExplotando)
                {
                    StartCoroutine(SecuenciaAutodestruccion());
                }
                else if (!estaExplotando)
                {
                    MoverHacia(transformJugador.position);
                }
                break;

            case TipoComportamiento.Cobarde:
                temporizadorDisparo -= Time.deltaTime;

                if (temporizadorDisparo <= 0f)
                {
                    Disparar();
                    temporizadorDisparo = tiempoEntreDisparos;
                }

                if (distancia < distanciaSegura)
                {
                    Vector3 direccionHuida = transform.position - transformJugador.position;
                    direccionHuida.y = 0f;
                    Vector3 destinoHuida = transform.position + direccionHuida.normalized * 2f;
                    MoverHacia(destinoHuida);
                }
                break;
        }
    }

    public void RecibirDaño(int cantidad)
    {
        if (estaExplotando) return;

        vidaActual -= cantidad;

        if (vidaActual <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(EfectoGolpe());
        }
    }

    IEnumerator EfectoGolpe()
    {
        if (miRenderer != null && !estaParpadeandoPorGolpe && !estaExplotando)
        {
            estaParpadeandoPorGolpe = true;
            miRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            miRenderer.material.color = colorOriginal;
            estaParpadeandoPorGolpe = false;
        }
    }

    void Disparar()
    {
        if (prefabProyectil != null)
        {
            Instantiate(prefabProyectil, transform.position, transform.rotation);
        }
    }

    void MoverHacia(Vector3 objetivo)
    {
        objetivo.y = posicionInicial.y;
        transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, posicionInicial.y, transform.position.z);
    }

    void RotarHaciaJugador()
    {
        Vector3 direccionMirada = transformJugador.position - transform.position;
        direccionMirada.y = 0f;

        if (direccionMirada != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direccionMirada);
        }
    }

    IEnumerator SecuenciaAutodestruccion()
    {
        estaExplotando = true;

        if (miRenderer != null)
        {
            for (int i = 0; i < 4; i++)
            {
                miRenderer.material.color = colorAlerta;
                yield return new WaitForSeconds(0.15f);
                miRenderer.material.color = colorOriginal;
                yield return new WaitForSeconds(0.15f);
            }
        }
        else
        {
            yield return new WaitForSeconds(1.2f);
        }

        if (prefabExplosion != null)
        {
            Instantiate(prefabExplosion, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    // --- NUEVO: FÍSICAS DE CHOQUE ---
    void OnCollisionEnter(Collision colision)
    {
        // Si el objeto contra el que chocamos tiene la etiqueta "Player"
        if (colision.gameObject.CompareTag("Player"))
        {
            // Buscamos el script de vida en el jugador
            PlayerStats jugador = colision.gameObject.GetComponent<PlayerStats>();

            if (jugador != null)
            {
                // Le pasamos el daño
                jugador.RecibirDaño(dañoPorContacto);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        if (comportamiento == TipoComportamiento.Cobarde)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, distanciaSegura);
        }
        else if (comportamiento == TipoComportamiento.Kamikaze)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, rangoExplosion);
        }
    }
}
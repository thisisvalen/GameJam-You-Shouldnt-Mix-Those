using UnityEngine;
using System.Collections;

public class EnemyFollow3 : MonoBehaviour
{
    public enum TipoComportamiento { Estandar, Kamikaze, Cobarde }

    [Header("Personalidad")]
    [Tooltip("Elige cómo se comportará esta calavera desde el Inspector")]
    public TipoComportamiento comportamiento = TipoComportamiento.Estandar;

    [Header("Vida y Combate")]
    public int vidaMaxima = 50; // ¡Ajusta este valor en el Inspector!
    private int vidaActual;

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
    private bool estaParpadeandoPorGolpe = false; // Evita conflictos de color

    void Start()
    {
        posicionInicial = transform.position;
        vidaActual = vidaMaxima; // Inicializamos la vida al máximo

        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador != null)
        {
            transformJugador = jugador.transform;
        }

        // --- ARREGLADO: Con estas líneas básicas el enemigo lee su propio material asignado ---
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

    // --- NUEVA FUNCIÓN: Recibir daño y parpadear ---
    public void RecibirDaño(int cantidad)
    {
        if (estaExplotando) return; // Si el kamikaze ya inició su explosión, ignoramos daño

        vidaActual -= cantidad;
        Debug.Log(gameObject.name + " recibió daño. Vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            Destroy(gameObject); // Muere si se queda sin vida
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
            miRenderer.material.color = Color.red; // Se pinta de rojo brillante
            yield return new WaitForSeconds(0.1f); // Parpadeo súper rápido
            miRenderer.material.color = colorOriginal; // Regresa a su color base (azul, amarillo o blanco)
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
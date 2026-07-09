using UnityEngine;
using System.Collections; // ¡Súper importante para usar tiempos y corrutinas!

public class VidaJugador : MonoBehaviour
{
    [Header("Salud")]
    public int vidaMaxima = 100;
    private int vidaActual;

    [Header("Efectos Visuales")]
    private Renderer miRenderer;
    private Color colorOriginal;
    private bool estaParpadeando = false;

    void Start()
    {
        vidaActual = vidaMaxima;

        // Buscamos el componente que dibuja al jugador en pantalla
        miRenderer = GetComponentInChildren<Renderer>();
        if (miRenderer != null)
        {
            // Guardamos su color normal para poder regresar a él
            colorOriginal = miRenderer.material.color;
        }
    }

    public void RecibirDaño(int cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log("¡Ouch! El jugador recibió " + cantidad + " de daño. Vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            Debug.Log("¡GAME OVER! El jugador ha sido derrotado.");

            // Aquí puedes destruir al jugador, reiniciar el nivel, o mostrar una pantalla de perder
            // Destroy(gameObject);
        }
        else
        {
            // Si el jugador sigue vivo, activamos el parpadeo rojo
            StartCoroutine(EfectoGolpe());
        }
    }

    // La animación rápida de destello
    IEnumerator EfectoGolpe()
    {
        if (miRenderer != null && !estaParpadeando)
        {
            estaParpadeando = true;
            miRenderer.material.color = Color.red; // Se pinta de rojo brillante
            yield return new WaitForSeconds(0.1f); // Espera súper cortita
            miRenderer.material.color = colorOriginal; // Regresa a su color normal
            estaParpadeando = false;
        }
    }
}
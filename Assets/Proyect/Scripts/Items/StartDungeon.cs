using UnityEngine;

public class StartDungeon : MonoBehaviour
{
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Lógica para iniciar la mazmorra
            Debug.Log("Iniciando la mazmorra...");
            other.GetComponent<PlayerStats>().DontResetPowerUp();
            boxCollider.isTrigger = false;

        }
    }
}

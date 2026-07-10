using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    private Input inputPlayer;
    [SerializeField] private float attackCooldown = 4f; // Tiempo de espera entre ataques
    private bool isAttacking = false;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    public Projectile ProjectilePrefab()
    {
        return bulletPrefab.GetComponent<Projectile>();
    }

    void Start()
    {
        inputPlayer = new Input();
        inputPlayer.Player.Attack.Enable();
        inputPlayer.Player.Crouch.Enable();
        Activate();
    }

    private void Activate()
    {
        inputPlayer.Player.Attack.performed += Attack;
        inputPlayer.Player.Crouch.performed += ResetPowerUp;
    }

    private void Deactivate()
    {
        inputPlayer.Player.Attack.performed -= Attack;
        inputPlayer.Player.Crouch.performed -= ResetPowerUp;
    }

    private void Attack(InputAction.CallbackContext ctx)
    {
        if (isAttacking) return; // Evitar ataques mientras ya se está atacando
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        isAttacking = true;
        StartCoroutine(ColdDownAttack());
    }

    private void ResetPowerUp(InputAction.CallbackContext ctx)
    {
        print("Reset PowerUp");
    }

    private IEnumerator ColdDownAttack()
    {
        yield return null;
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void OnDisable()
    {
        Deactivate();
        inputPlayer.Player.Attack.Disable();
        inputPlayer.Player.Crouch.Disable();
    }
}

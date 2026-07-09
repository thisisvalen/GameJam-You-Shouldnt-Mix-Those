using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    private Input inputPlayer;
    void Start()
    {
        inputPlayer = new Input();
        inputPlayer.Player.Attack.Enable();
        inputPlayer.Player.Attack.performed += ctx => Attack();
    }

    private void Attack()
    {
        Debug.Log("Attack");
    }

    void OnDisable()
    {
        inputPlayer.Player.Attack.Disable();
    }
}

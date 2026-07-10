using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Stats", menuName = "Player/Stats")]
public class Stats : ScriptableObject
{
    [SerializeField] private float health = 5;
    [SerializeField] private float damage = 10;
    [SerializeField] private float defense = 0;
    [SerializeField] private float speed = 3;
    [SerializeField] private UnityEvent statChanged;

    public float Health
    {
        get => health;
        set
        {
            health = Math.Clamp(value, 0, 9999);
            statChanged.Invoke();
        }
    }

    public float Damage
    {
        get => damage;
        set
        {
            damage = Math.Clamp(value, 0, 9999);
            statChanged.Invoke();
        }
    }

    public float Defense
    {
        get => defense;
        set
        {
            defense = Math.Clamp(value, 0, 9999);
            statChanged.Invoke();
        }
    }

    public float Speed
    {
        get => speed;
        set
        {
            speed = Math.Clamp(value, 0, 9999);
            statChanged.Invoke();
        }
    }

    public void OnEnable()
    {
        statChanged ??= new UnityEvent();
    }

    public void OnValidate()
    {
        statChanged?.Invoke();
    }
    public void ResetStats()
    {
        Health = 5;
        Damage = 10;
        Defense = 0;
        Speed = 3;
    }
}

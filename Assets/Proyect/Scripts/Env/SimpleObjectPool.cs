using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pool mínimo por prefab. WaveManager mantiene un pool por cada EnemyData.
/// Evita el costo de Instantiate/Destroy en ráfagas de spawn, que es donde
/// más se nota el spike de GC/CPU en oleadas con muchos enemigos.
/// </summary>
public class SimpleObjectPool
{
    private readonly GameObject prefab;
    private readonly Transform parent;
    private readonly Stack<GameObject> inactive = new Stack<GameObject>();

    public SimpleObjectPool(GameObject prefab, Transform parent, int prewarm)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < prewarm; i++)
        {
            GameObject go = CreateNew();
            go.SetActive(false);
            inactive.Push(go);
        }
    }

    private GameObject CreateNew()
    {
        GameObject go = Object.Instantiate(prefab, parent);
        // El componente devuelve el objeto al pool en vez de destruirlo.
        PooledEnemy pooled = go.GetComponent<PooledEnemy>();
        if (pooled == null) pooled = go.AddComponent<PooledEnemy>();
        pooled.Initialize(this);
        return go;
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject go = inactive.Count > 0 ? inactive.Pop() : CreateNew();
        go.transform.SetPositionAndRotation(position, rotation);
        go.SetActive(true);
        return go;
    }

    public void Return(GameObject go)
    {
        go.SetActive(false);
        inactive.Push(go);
    }
}

/// <summary>
/// Componente marcador que permite a cualquier enemigo devolverse a su pool
/// al morir, en vez de llamar Destroy(). Tu script de vida/muerte del enemigo
/// debe llamar ReturnToPool() en lugar de Destroy(gameObject).
/// </summary>
public class PooledEnemy : MonoBehaviour
{
    private SimpleObjectPool ownerPool;

    public void Initialize(SimpleObjectPool pool)
    {
        ownerPool = pool;
    }

    public void ReturnToPool()
    {
        if (ownerPool != null)
            ownerPool.Return(gameObject);
        else
            Destroy(gameObject); // fallback si no fue creado por un pool
    }
}
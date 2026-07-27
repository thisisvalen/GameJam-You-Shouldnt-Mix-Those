using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador de oleadas con dificultad incremental.
///
/// FÓRMULA DE DIFICULTAD (serie híbrida aritmético-geométrica, con techo):
///
///   Budget(n) = clamp( B0 * (1 + g)^(n-1)  +  L * (n-1) ,  B0 ,  Bmax )
///
///   - B0   = presupuesto base (oleada 1)
///   - g    = tasa de crecimiento geométrico (ramp-up agresivo en oleadas tempranas)
///   - L    = crecimiento lineal (aporte estable en oleadas tardías, evita que
///            el término geométrico se dispare sin control)
///   - Bmax = techo de presupuesto, para que el juego no se vuelva imposible
///            a partir de cierto punto (curva se aplana, no crece indefinido)
///
/// El presupuesto se "gasta" instanciando enemigos hasta agotarlo. Qué tipo
/// de enemigo sale en cada instancia se decide con una ruleta ponderada,
/// donde el peso de cada tipo también es función de la oleada (ver
/// EnemyData.GetWeightForWave). Así, por ejemplo, el Exploder puede tener
/// weightGrowthPerWave positivo (cada vez más común) y el Ranged uno
/// negativo o cero (se mantiene o pierde protagonismo relativo).
///
/// El intervalo entre spawns dentro de la oleada decae con otra serie
/// geométrica (spawns cada vez más rápidos, con piso mínimo):
///
///   Interval(n) = max( Imin , I0 * r^(n-1) )   con 0 < r < 1
/// </summary>
public class WaveController : MonoBehaviour
{
    [Header("Enemigos disponibles")]
    [SerializeField] private List<EnemyData> enemyTypes = new List<EnemyData>();

    [Header("Puntos de spawn")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Fórmula de presupuesto (dificultad)")]
    [SerializeField] private float baseBudget = 10f;      // B0
    [SerializeField] private float geometricGrowth = 0.12f; // g
    [SerializeField] private float linearGrowth = 2f;       // L
    [SerializeField] private float maxBudget = 200f;        // Bmax

    [Header("Fórmula de ritmo de spawn")]
    [SerializeField] private float baseSpawnInterval = 1.5f; // I0
    [SerializeField] private float intervalDecay = 0.95f;     // r
    [SerializeField] private float minSpawnInterval = 0.25f;  // Imin

    [Header("Control de oleadas")]
    [SerializeField] private float delayBetweenWaves = 4f;
    [SerializeField] private int maxAliveEnemies = 25;
    [SerializeField] private bool autoStart = true;

    public int CurrentWave { get; private set; } = 0;
    public bool IsSpawning { get; private set; } = false;

    private readonly Dictionary<EnemyData, SimpleObjectPool> pools = new Dictionary<EnemyData, SimpleObjectPool>();
    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private int nextSpawnPointIndex = 0;

    private void Awake()
    {
        BuildPools();
    }

    private void Start()
    {
        if (autoStart) StartCoroutine(RunWaves());
    }

    private void BuildPools()
    {
        foreach (EnemyData data in enemyTypes)
        {
            if (data == null || data.prefab == null) continue;
            pools[data] = new SimpleObjectPool(data.prefab, transform, data.poolPrewarm);
        }
    }

    private IEnumerator RunWaves()
    {
        while (true)
        {
            CurrentWave++;
            yield return StartCoroutine(SpawnWave(CurrentWave));

            // Espera a que la oleada se vacíe (o casi) antes de dar el respiro
            yield return new WaitUntil(() => AliveCount() <= 0);
            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    private IEnumerator SpawnWave(int waveNumber)
    {
        IsSpawning = true;

        float budget = ComputeBudget(waveNumber);
        float interval = ComputeSpawnInterval(waveNumber);

        while (budget > 0f)
        {
            // Respeta el tope de enemigos vivos simultáneos para no saturar CPU/física
            if (AliveCount() >= maxAliveEnemies)
            {
                yield return new WaitForSeconds(interval);
                continue;
            }

            EnemyData chosen = PickWeightedEnemy(waveNumber);
            if (chosen == null) break; // no hay enemigos disponibles aún para esta oleada

            SpawnEnemy(chosen);
            budget -= chosen.threatCost;

            yield return new WaitForSeconds(interval);
        }

        IsSpawning = false;
    }

    /// <summary>Budget(n) — ver fórmula en el header de la clase.</summary>
    private float ComputeBudget(int waveNumber)
    {
        float geometricTerm = baseBudget * Mathf.Pow(1f + geometricGrowth, waveNumber - 1);
        float linearTerm = linearGrowth * (waveNumber - 1);
        float total = geometricTerm + linearTerm;
        return Mathf.Clamp(total, baseBudget, maxBudget);
    }

    /// <summary>Interval(n) — serie geométrica decreciente con piso mínimo.</summary>
    private float ComputeSpawnInterval(int waveNumber)
    {
        float interval = baseSpawnInterval * Mathf.Pow(intervalDecay, waveNumber - 1);
        return Mathf.Max(minSpawnInterval, interval);
    }

    /// <summary>Ruleta ponderada entre los enemigos desbloqueados para esta oleada.</summary>
    private EnemyData PickWeightedEnemy(int waveNumber)
    {
        float totalWeight = 0f;
        foreach (EnemyData data in enemyTypes)
            totalWeight += data.GetWeightForWave(waveNumber);

        if (totalWeight <= 0f) return null;

        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (EnemyData data in enemyTypes)
        {
            float w = data.GetWeightForWave(waveNumber);
            if (w <= 0f) continue;
            cumulative += w;
            if (roll <= cumulative) return data;
        }

        return null; // no debería llegar aquí
    }

    private void SpawnEnemy(EnemyData data)
    {
        if (!pools.TryGetValue(data, out SimpleObjectPool pool)) return;

        Transform point = GetSpawnPoint();
        if (point == null) return;

        GameObject enemy = pool.Get(point.position, point.rotation);

        aliveEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);
        aliveEnemies.Add(enemy);
    }

    private Transform GetSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return null;
        if (spawnPoints.Length == 1) return spawnPoints[0];

        int chosenIndex = nextSpawnPointIndex % spawnPoints.Length;
        nextSpawnPointIndex = (nextSpawnPointIndex + 1) % spawnPoints.Length;

        return spawnPoints[chosenIndex];
    }

    private int AliveCount()
    {
        aliveEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);
        return aliveEnemies.Count;
    }
}
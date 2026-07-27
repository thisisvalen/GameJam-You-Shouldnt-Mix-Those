using UnityEngine;
/// <summary>
/// Tipo de comportamiento del enemigo. Se usa para agrupar lógica de IA,
/// pero la config de spawn vive en EnemyData (composición sobre herencia rígida).
/// </summary>
public enum EnemyType
{
    Ranged,   // Ataca a distancia
    Melee,    // Ataca cuerpo a cuerpo
    Exploder  // Se acerca y explota
}
/// <summary>
/// Configuración de un tipo de enemigo, usada por WaveManager para decidir
/// qué instanciar y con qué peso relativo en cada oleada.
/// Un asset por tipo de enemigo (crear vía Assets > Create > Waves > EnemyData).
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "Waves/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Identidad")]
    public string displayName = "Enemy";
    public EnemyType type;
    public GameObject prefab;

    [Header("Costo de dificultad")]
    [Tooltip("Cuánto 'presupuesto' de la oleada consume una instancia de este enemigo.")]
    public int threatCost = 1;

    [Header("Disponibilidad")]
    [Tooltip("Oleada mínima en la que este enemigo puede empezar a aparecer.")]
    public int minWave = 1;

    [Header("Peso de aparición (serie lineal por oleada)")]
    [Tooltip("Peso relativo base en la oleada 1.")]
    public float baseWeight = 1f;
    [Tooltip("Cuánto sube (o baja, si es negativo) el peso por cada oleada transcurrida.")]
    public float weightGrowthPerWave = 0f;

    [Header("Pooling")]
    [Tooltip("Cuántas instancias pre-calentar en el pool al iniciar.")]
    public int poolPrewarm = 5;

    /// <summary>
    /// Peso efectivo de este enemigo en la oleada n (n empieza en 1).
    /// Nunca negativo: si la fórmula da un valor negativo, el enemigo
    /// deja de aparecer (peso 0) en vez de "restar" a otros.
    /// </summary>
    public float GetWeightForWave(int waveNumber)
    {
        if (waveNumber < minWave) return 0f;
        float w = baseWeight + weightGrowthPerWave * (waveNumber - 1);
        return Mathf.Max(0f, w);
    }
}
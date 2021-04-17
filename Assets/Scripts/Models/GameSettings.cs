using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "SPACE/GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("General")]
    public Vector2 GridSize;
    public int ScoreAdditive;
    public int ScoreBonusMultiplier;
    public int BombInitialPoint;
    public int BombInitialCounter;
    public float Gravity;
    public Color[] Colors;
    
    [Header("Prefabs")]
    public SlotController SlotPrefab;
    public HexagonController HexagonPrefab;
    public BombController BombPrefab;
    public ParticleSystem DebrisPrefab;
    public ParticleSystem ExplosionPrefab;
    public TMP_Text FloatingTextPrefab;
}

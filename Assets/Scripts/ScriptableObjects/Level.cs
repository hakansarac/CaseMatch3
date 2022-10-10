using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Level")]
public class Level : ScriptableObject {
  [Header("Board Dimensions")]
  public int Width;
  public int Height;

  [Header("Goals")]
  public Goal[] ScoreGoals;
}

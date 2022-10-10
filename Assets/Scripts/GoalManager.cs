using System.Collections.Generic;
using UnityEngine;



public class GoalManager : MonoBehaviour {
  public delegate void LevelCompAction();
  public static event LevelCompAction LevelUpdate;

  public GameObject GoalPrefab;
  public GameObject GoalParent;

  private Goal[] _allGoals;    
  private List<GameObject> _goalPrefabList = new List<GameObject>();    
  private List<GoalInfo> _curGoals = new List<GoalInfo>();
  private Board _board;

  /// <summary>
  /// it is called to set current level goals 
  /// </summary>
  public void NewLevel() {
    GetGoals();
    SetupGoals();
  }

  /// <summary>
  /// it is called to delete goals of previous level
  /// </summary>
  public void EndLevel() {
    int temp = _goalPrefabList.Count;
    for (int i = 0; i < temp; i++) {
      Destroy(_goalPrefabList[i]);
    }
    _goalPrefabList.Clear();
    _curGoals.Clear();
  }

  /// <summary>
  /// it is called to update goal text
  /// </summary>
  public void UpdateGoals() {
    int goalsCompleted = 0;
    for (int i = 0; i < _allGoals.Length; i++) {

      if (_allGoals[i].CurrentNum >= _allGoals[i].GoalNum) {
        goalsCompleted++;
        _curGoals[i].GoalText.text = _allGoals[i].GoalNum + "/" + _allGoals[i].GoalNum;
      } else {
        _curGoals[i].GoalText.text = _allGoals[i].CurrentNum + "/" + _allGoals[i].GoalNum;
      }
    }
        
    if (goalsCompleted >= _allGoals.Length) {
      LevelUpdate();
    }
  }

  /// <summary>
  /// it called to count each goal type
  /// </summary>
  /// <param name="goalToCompare"></param>
  public void CompareGoal(string goalToCompare) {
    for (int i = 0; i < _allGoals.Length; i++) {
      if (goalToCompare == _allGoals[i].MatchValue) {
        _allGoals[i].CurrentNum++;
      }
    }
  }


  private void Start() {
    _board = FindObjectOfType<Board>();
  }
    
  /// <summary>
  /// it called to get current level
  /// </summary>
  void GetGoals() {

    if (_board != null) {
      if (_board.CurWorld != null) {
        if (_board.CurWorld.AllLevels[GameManager.Instance.CurrentLevel-1] != null) {
          _allGoals = _board.CurWorld.AllLevels[GameManager.Instance.CurrentLevel - 1].ScoreGoals;
        }
      }
    }
  }

  /// <summary>
  /// it is called to fill goal panels
  /// </summary>
  void SetupGoals() {
    for (int i=0; i < _allGoals.Length; i++) {
      GameObject goal = Instantiate(GoalPrefab, GoalParent.transform.position, Quaternion.identity);
      goal.transform.SetParent(GoalParent.transform);
      goal.transform.localPosition = new Vector3(0, i*-200, 0);
      _goalPrefabList.Add(goal);

      GoalInfo infoPanel = goal.GetComponent<GoalInfo>();
      _curGoals.Add(infoPanel);
      infoPanel.GoalSprite = _allGoals[i].GoalType;
      infoPanel.GoalString = "0/" + _allGoals[i].GoalNum;
    }
  }    
}

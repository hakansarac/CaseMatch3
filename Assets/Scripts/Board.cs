using Cysharp.Threading.Tasks;
using UnityEngine;


public class Board : MonoBehaviour {
  //BoardStates to limit user operations
  public enum State { 
    PLAY, 
    LOCK 
  }
  public State BoardState;

  [Header("Scriptable Object Stuff")]
  public World CurWorld;
  public int Level;

  [Header("Board Dimensions")]
  public int Width;
  public int Height;
  public int OffSet;

  public GameObject[,] AllFruits { get; set; }

  //fruit background prefab
  [SerializeField] GameObject _backgroundPrefab;
  //fruit prefabs
  [SerializeField] private GameObject[] _fruits;
  //backgrounds list  
  private GameObject[,] _allBackgrounds { get; set; }
  //goal manager  
  private GoalManager _goalManager { get; set; }
    
  /// <summary>
  /// it is called when level end to clear level
  /// </summary>
  public void EndLevel() {
    for (int i = 0 ; i < Width ; i++) {
      for (int j = 0 ; j < Height ; j++) {
        Destroy(AllFruits[i, j]);
        Destroy(_allBackgrounds[i, j]);
        AllFruits[i, j] = null;
      }
    }
    for (int i=0 ; i < CurWorld.AllLevels[GameManager.Instance.CurrentLevel-1].ScoreGoals.Length ; i++) {
      CurWorld.AllLevels[GameManager.Instance.CurrentLevel - 1].ScoreGoals[i].CurrentNum = 0;
    }
  }

  /// <summary>
  /// it is called when new level begin to create new grid
  /// </summary>
  public void NewLevel() {
    if (CurWorld != null) {
      if (CurWorld.AllLevels[GameManager.Instance.CurrentLevel - 1] != null) {
        Width = CurWorld.AllLevels[GameManager.Instance.CurrentLevel - 1].Width;
        Height = CurWorld.AllLevels[GameManager.Instance.CurrentLevel - 1].Height;
      }
    }
    _allBackgrounds = new GameObject[Width, Height];
    AllFruits = new GameObject[Width, Height];
    SetBackgrounds();
    BoardState = State.PLAY;
  }

  /// <summary>
  /// check fruits to destroy 
  /// </summary>
  public void DestroyMatches() {
    BoardState = State.LOCK;
    for (int i = 0; i < Width; i++) {
      for (int j = 0; j < Height; j++) {
        if (AllFruits[i, j] != null) {
          DestroyMatchesAt(i, j);
        }
      }
    }
    DecreaseRow();
  }

  private void Start() {
    _goalManager = FindObjectOfType<GoalManager>();
    BoardState = State.LOCK;
  }

  /// <summary>
  /// it is called to setup grid
  /// </summary>
  private void SetBackgrounds() {
    for (int i = 0; i < Width; i++) {
      for (int j = 0; j < Height; j++) {
        Vector3 tempPos = new Vector3(i, j);
        GameObject background = Instantiate(_backgroundPrefab, tempPos, Quaternion.identity);
        background.transform.parent = this.transform;
        background.name = "(" + i + "," + j + ")";
        _allBackgrounds[i, j] = background;
        int fruitType = Random.Range(0, _fruits.Length);

        while (MatchesAt(i, j, _fruits[fruitType])) {
          fruitType = Random.Range(0, _fruits.Length);
        }
        Vector3 zPos = new Vector3(0, 0, -0.1f);
        GameObject fruit = Instantiate(_fruits[fruitType], tempPos + zPos, Quaternion.identity);
        fruit.GetComponent<Fruit>().Column = i;
        fruit.GetComponent<Fruit>().Row = j;
        fruit.transform.parent = this.transform;
        AllFruits[i, j] = fruit;  
      }
    }
  }

  /// <summary>
  /// it is called to detect where matches are
  /// </summary>
  /// <param name="column"></param>
  /// <param name="row"></param>
  /// <param name="piece"></param>
  /// <returns> if there is a match at input values return true else false</returns>
  private bool MatchesAt(int column, int row, GameObject piece) {
    if (column > 1 && row > 1) {
      if (AllFruits[column-1, row].tag == piece.tag && AllFruits[column-2, row].tag == piece.tag) {
        return true;
      }
      if (AllFruits[column, row-1].tag == piece.tag && AllFruits[column, row-2].tag == piece.tag) {
        return true;
      }
    } else if (column <= 1 || row <=1){
      if (row > 1) {
        if (AllFruits[column, row-1].tag == piece.tag && AllFruits[column, row-2].tag == piece.tag) {
          return true;
        }
      }
      if (column > 1) {
        if (AllFruits[column-1, row].tag == piece.tag && AllFruits[column-2, row].tag == piece.tag) {
          return true;
        }
      }
    }
    return false;
  }

  /// <summary>
  /// Destroy pieces which are matched 
  /// </summary>
  /// <param name="column"></param>
  /// <param name="row"></param>
  private void DestroyMatchesAt(int column, int row) {
    if (AllFruits[column, row].GetComponent<Fruit>().IsMatched) {
      if(_goalManager != null) {
        _goalManager.CompareGoal(AllFruits[column, row].tag);
        _goalManager.UpdateGoals();
      }
      Destroy(AllFruits[column, row]);
      AllFruits[column, row] = null;
    }
  }

    
  /// <summary>
  /// it is called when there are any destroyed pieces to set new row values of other pieces 
  /// </summary>
  /// <returns></returns>
  async UniTask DecreaseRow() {
    int nullCount = 0;
    for (int i = 0 ; i < Width ; i++) {
      for (int j = 0 ; j < Height ; j++) {
        if (AllFruits[i, j] == null) {
          nullCount++;
        } else if (nullCount > 0) {
          AllFruits[i, j].GetComponent<Fruit>().Row -= nullCount;
          AllFruits[i, j].GetComponent<Fruit>().PreviousRow -= nullCount;
          AllFruits[i, j] = null;
        }
      }
      nullCount = 0;
    }
    await UniTask.Delay(1000); 
    if (GameManager.Instance.GameState == GameManager.State.PLAY) {
      await FillBoard();
    }
  }


  /// <summary>
  /// it is called to refill the board after a destroying
  /// </summary>
  private void RefillBoard() {
    for (int i = 0 ; i < Width ; i++) {
      for (int j = 0 ; j < Height ; j++) {
        if (AllFruits[i, j] == null) {
          Vector3 tempPos = new Vector3(i, j+OffSet, -0.1f);
          int fruitType = Random.Range(0, _fruits.Length);
          GameObject piece = Instantiate(_fruits[fruitType], tempPos, Quaternion.identity);
          AllFruits[i, j] = piece;
          piece.GetComponent<Fruit>().Column = i;
          piece.GetComponent<Fruit>().PreviousColumn = i;
          piece.GetComponent<Fruit>().Row = j;
          piece.GetComponent<Fruit>().PreviousRow = j;
        }
      }
    }
  }


  private bool MatchesOnBoard() {
    for (int i = 0 ; i < Width ; i++) {
      for (int j = 0 ; j < Height ; j++) {
        if (AllFruits[i, j] != null) {
          if (AllFruits[i, j].GetComponent<Fruit>().IsMatched) {
            return true;
          }
        }
      }
    }
    return false;
  }


  async UniTask FillBoard() {
    RefillBoard();
    await UniTask.Delay(600);

    while (MatchesOnBoard()) {
      DestroyMatches();
      await UniTask.Delay(600);
    }
    BoardState = State.PLAY;
  }    
}

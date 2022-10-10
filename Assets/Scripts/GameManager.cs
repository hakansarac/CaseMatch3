using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
  //Game states
  public enum State { 
    MENU, 
    INIT, 
    PLAY, 
    LEVELCOMPLETED, 
    LOADLEVEL, 
    TRYAGAIN 
  }
  public State GameState;

  public delegate void GameStateAction();
  public static event GameStateAction LevelTextUpdate;
    
  public static GameManager Instance;

  public int CurrentLevel {
    get { return _currentLevel; }
  }


  [Header("Panels")]
  //Menu
  [SerializeField] private GameObject _panelMenu;
  //Play
  [SerializeField] private GameObject _panelPlay;
  //NextLevel
  [SerializeField] private GameObject _panelSuccess;

  //Current level
  private int _currentLevel{ get; set; }
  private Board _board{ get; set; }
  private GoalManager _goalManager { get; set; }



  /// <summary>
  ///     event trigger when user presses tap to start button
  /// </summary>
  public void PlayClicked() {
    SwitchState(State.INIT);
  }

  /// <summary>
  /// it is called when the level end
  /// </summary>
  public void Successed() {
    _board.EndLevel();
    _goalManager.EndLevel();
    SwitchState(State.LEVELCOMPLETED);
  }



  private void Awake() {
    Instance = this;
  }

  private void Start() {
    _currentLevel = 1;
    SwitchState(State.MENU);
    _board = FindObjectOfType<Board>();
    _goalManager = FindObjectOfType<GoalManager>();
  }

  /// <summary>
  ///     switchs gamemanager state
  /// </summary>
  /// <param name="newState"></param>
  /// <param name="delay"></param>
  private void SwitchState(State newState, float delay = 0) {
    StartCoroutine(SwitchDelay(newState, delay));
  }

  IEnumerator SwitchDelay(State newState, float delay) {
    yield return new WaitForSeconds(delay);
    EndState();
    GameState = newState;
    BeginState(newState);
  }

  /// <summary>
  ///     begin of new state
  /// </summary>
  /// <param name="newState"></param>
  void BeginState(State newState) {
    switch (newState) {
      case State.MENU:
        _panelMenu.SetActive(true);
        break;
      case State.INIT:
        _panelPlay.SetActive(true);
        SwitchState(State.LOADLEVEL);
        break;
      case State.PLAY:        
        break;
      case State.LEVELCOMPLETED:
        _currentLevel++;
        _panelSuccess.SetActive(true);
        break;
      case State.LOADLEVEL:
        LevelTextUpdate();
        SwitchState(State.PLAY);
        break;
    }
  }

  /// <summary>
  ///     end of last state
  /// </summary>
  void EndState() {
    switch (GameState) {
      case State.MENU:
        _panelMenu.SetActive(false);
        break;
      case State.INIT:
        _board.NewLevel();
        _goalManager.NewLevel();
        break;
      case State.PLAY:
        _panelPlay.SetActive(false);
        break;
      case State.LEVELCOMPLETED:
        _panelSuccess.SetActive(false);        
        break;
      case State.LOADLEVEL:
        break;
    }
  }

  

  private void OnEnable() {
    GoalManager.LevelUpdate += Successed;
  }

  private void OnDisable() {
    GoalManager.LevelUpdate -= Successed;
  }
}

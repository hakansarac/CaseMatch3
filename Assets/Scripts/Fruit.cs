using System.Collections;
using UnityEngine;

public class Fruit : MonoBehaviour {
  private const float _swipeResist = 1f;
  private const float _piecesPosZ = -0.1f;

  [Header("Board")]
  public int Column;
  public int Row;
  public int PreviousColumn;
  public int PreviousRow;
  public int TargetX;
  public int TargetY;
  public bool IsMatched = false;

  public float SwipeAngle = 0;

  private FindMatches _findMatches{ get; set; }
  private GameObject _otherFruit{ get; set; }
  private Board _board{ get; set; }
  private Vector3 _firstTouchPos{ get; set; }
  private Vector3 _lastTouchPos{ get; set; }
  private Vector3 _tempPos{ get; set; }

  /// <summary>
  /// move the fruits according to new match is exist or not exist 
  /// </summary>
  /// <returns></returns>
  public IEnumerator CheckMoveCo() {
    yield return new WaitForSeconds(.5f);
    if (_otherFruit != null) {

      if (!IsMatched && !_otherFruit.GetComponent<Fruit>().IsMatched) {
        _otherFruit.GetComponent<Fruit>().Row = Row;
        _otherFruit.GetComponent<Fruit>().Column = Column;
        Row = PreviousRow;
        Column = PreviousColumn;
      } else {
        _board.DestroyMatches();
      }
      _otherFruit = null;
    }
  }


  private void Start() {
    _board = FindObjectOfType<Board>();
    _findMatches = FindObjectOfType<FindMatches>();
    PreviousColumn = Column;
    PreviousRow = Row;
  }

  /// <summary>
  /// set location of pieces according to their column and row values
  /// </summary>
  private void Update() {
    if (IsMatched) {
      SpriteRenderer curSprite = GetComponent<SpriteRenderer>();
      curSprite.color = new Color(0f, 0f, 0f, .2f);
    }
    TargetX = Column;
    TargetY = Row;
    if (Mathf.Abs(TargetX - transform.position.x) > .1) {
      _tempPos = new Vector3(TargetX, transform.position.y, _piecesPosZ);
      transform.position = Vector3.Lerp(transform.position, _tempPos, .6f);
      if (_board.AllFruits[Column,Row]!= this.gameObject) {
        _board.AllFruits[Column, Row] = this.gameObject;
      }
      _findMatches.FindAllMatches();
    } else {
      _tempPos = new Vector3(TargetX, transform.position.y, _piecesPosZ);
      transform.position = _tempPos;            
    }
    if (Mathf.Abs(TargetY - transform.position.y) > .1) {
      _tempPos = new Vector3(transform.position.x, TargetY, _piecesPosZ);
      transform.position = Vector3.Lerp(transform.position, _tempPos, .6f);
      if (_board.AllFruits[Column, Row] != this.gameObject) {
        _board.AllFruits[Column, Row] = this.gameObject;
      }
      _findMatches.FindAllMatches();
    } else {
      _tempPos = new Vector3(transform.position.x, TargetY, _piecesPosZ);
      transform.position = _tempPos;
    }
  }

    

  private void OnMouseDown() {
    _firstTouchPos = Input.mousePosition;
  }

  /*private void OnMouseDrag()
  {
    _lastTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3 newPos = new Vector3(_lastTouchPos.x,transform.position.y,transform.position.z);
    transform.position = Vector3.Lerp(transform.position,newPos,0.5f*Time.deltaTime);
  }*/



  private void OnMouseUp() {
    _lastTouchPos = Input.mousePosition;
    CalculateAngle();
    if (_board.BoardState == Board.State.PLAY) {
      MovePieces();
    }
        
  }

  private void CalculateAngle() {
    if (Mathf.Abs(_lastTouchPos.y - _firstTouchPos.y) > _swipeResist
        || Mathf.Abs(_lastTouchPos.x - _firstTouchPos.x) > _swipeResist) {
      SwipeAngle = Mathf.Atan2(
        _lastTouchPos.y - _firstTouchPos.y, _lastTouchPos.x - _firstTouchPos.x
      ) * 180 / Mathf.PI;
    }        
  }

  /// <summary>
  /// it is called to detect direction
  /// </summary>
  private void MovePieces() {
        
    if ((SwipeAngle > -45 && SwipeAngle <= 45) && Column < _board.Width-1) {
      //Right
      _otherFruit = _board.AllFruits[Column + 1, Row];
      _otherFruit.GetComponent<Fruit>().Column--;
      Column++;
    } else if ((SwipeAngle > 45 && SwipeAngle <= 135) && Row < _board.Height-1) {
      //Up
      _otherFruit = _board.AllFruits[Column, Row+1];
      _otherFruit.GetComponent<Fruit>().Row--;
      Row++;
    } else if ((SwipeAngle > 135 || SwipeAngle <= -135) && Column > 0) {
      //Left
      _otherFruit = _board.AllFruits[Column - 1, Row];
      _otherFruit.GetComponent<Fruit>().Column++;
      Column--;
    } else if ((SwipeAngle < -45 && SwipeAngle >= -135) && Row > 0) {
      //Down
      _otherFruit = _board.AllFruits[Column, Row-1];
      _otherFruit.GetComponent<Fruit>().Row++;
      Row--;
    }
        
    StartCoroutine(CheckMoveCo());
  }
}

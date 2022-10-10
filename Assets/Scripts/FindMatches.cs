using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour {

  private Board _board{ get; set; }
    

  private void Start() {
    _board = FindObjectOfType<Board>();
  }

  public void FindAllMatches() {
    StartCoroutine(FindAllMatchesCo());
  }

  /// <summary>
  /// it is called to check matches
  /// </summary>
  /// <returns></returns>
  private IEnumerator FindAllMatchesCo() {
    yield return new WaitForSeconds(.3f);
    for (int i = 0; i < _board.Width; i++) {
      for (int j = 0 ; j < _board.Height; j++) {
        GameObject curFruit = _board.AllFruits[i, j];
        if (curFruit != null) {
          if (i > 0 && i < _board.Width - 1) {
            GameObject leftFruit = _board.AllFruits[i - 1, j];
            GameObject rightFruit = _board.AllFruits[i + 1, j];
            if (leftFruit !=null && rightFruit != null) {
              if (leftFruit.tag == curFruit.tag && rightFruit.tag == curFruit.tag) {
                leftFruit.GetComponent<Fruit>().IsMatched = true;
                rightFruit.GetComponent<Fruit>().IsMatched = true;
                curFruit.GetComponent<Fruit>().IsMatched = true;
              }
            }
          }

          if (j > 0 && j < _board.Height - 1) {
            GameObject upFruit = _board.AllFruits[i, j+1];
            GameObject downFruit = _board.AllFruits[i, j-1];
            if (upFruit != null && downFruit != null) {
              if (upFruit.tag == curFruit.tag && downFruit.tag == curFruit.tag) {
                upFruit.GetComponent<Fruit>().IsMatched = true;
                downFruit.GetComponent<Fruit>().IsMatched = true;
                curFruit.GetComponent<Fruit>().IsMatched = true;
              }
            }
          }
        }
      }
    }
  }
}

using UnityEngine;

public class CameraManager : MonoBehaviour {
  private const float _radio = 0.625f;
  private const float _padding = 2f;

  public float CameraOffset;

  private Board _board{ get; set; }
    
    

  private void Start() {
    _board = FindObjectOfType<Board>();
    if (_board != null) {
      RepositionCamera(_board.Width-1, _board.Height-1);
    }
  }

  private void LateUpdate() {
    _board = FindObjectOfType<Board>();
    if (_board != null) {
      RepositionCamera(_board.Width - 1, _board.Height - 1);
    }
  }

  private void RepositionCamera(float posX, float posY) {
    Vector3 tempPos = new Vector3(posX / 2, posY / 2, _board.Width > _board.Height ? _board.Width*-2f : _board.Height*-2f);
    transform.position = tempPos;
    if (_board.Width >= _board.Height) {
      Camera.main.orthographicSize = (_board.Width / 2 + _padding) / _radio;
    } else {
      Camera.main.orthographicSize = _board.Height / 2 + _padding;
    }
  }
}

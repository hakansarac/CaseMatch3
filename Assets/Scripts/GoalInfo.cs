using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GoalInfo : MonoBehaviour {
  public Image GoalImage;
  public Sprite GoalSprite;
  public TextMeshProUGUI GoalText;
  public string GoalString;

  // Start is called before the first frame update
  void Start() {
    Setup();
  }
    
  void Setup() {
    GoalImage.sprite = GoalSprite;
    GoalText.text = GoalString;
  }
}

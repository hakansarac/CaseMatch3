using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LevelText : MonoBehaviour {
  private void OnEnable() {
    GameManager.LevelTextUpdate += NewLevel;
  }

  private void OnDisable() {
    GameManager.LevelTextUpdate -= NewLevel;
  }

  /// <summary>
  ///     Sets level text
  /// </summary>
  private void NewLevel() {
    this.GetComponent<TextMeshProUGUI>().SetText("LEVEL " + GameManager.Instance.CurrentLevel);
  }
}

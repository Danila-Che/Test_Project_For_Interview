using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
internal class Tween {
	[SerializeField] private Animator _tween;

	public void PlayTween (Vector3 worldCoordinate, int id) {
		_tween.gameObject.SetActive (true);

		_tween.transform.localPosition = worldCoordinate;
		_tween.GetComponentInChildren<Text> ().text = id.ToString ();
		_tween.GetComponentInChildren<Animator> ().Play ("NumberAppearance", -1, 0.0f);
	}
}

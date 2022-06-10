using System.Collections.Generic;
using UnityEngine;

internal class SerifPool {
	public delegate Transform InstantiateDelegate ();

	private readonly Border _border;
	private readonly InstantiateDelegate _createSerif;

	private readonly List<Transform> _createdSerifs = new List<Transform> ();

	private Queue<Transform> _sefifs = new Queue<Transform> ();

	public SerifPool (Border border, InstantiateDelegate instantiate) {
		_createSerif = instantiate;
		_border = border;
	}

	public void Reset () {
		foreach (var serif in _createdSerifs) {
			serif.gameObject.SetActive (false);
		}

		_sefifs = new Queue<Transform> (_createdSerifs);
	}

	public bool CanSetSerif (Vector2 position) {
		return _border.IsPointInside (position);
	}

	public void SetSerif (Vector3 position) {
		if (_border.IsPointInside (position) == false) {
			return;
		}

		Transform serif;

		if (_sefifs.Count > 0) {
			serif = _sefifs.Dequeue ();
		} else {
			serif = _createSerif ();
			_createdSerifs.Add (serif);
		}

		serif.localPosition = position;
		serif.gameObject.SetActive (true);
	}
}

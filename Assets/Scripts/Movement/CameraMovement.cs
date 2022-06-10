using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement: MonoBehaviour {
	[SerializeField] private RealModel _model;

	private Vector3 _offset;
	private Transform _transform;

	private void Start () {
		_transform = transform;

		_offset = _model.transform.position - _transform.position;
	}

	private void Update () {
		_transform.position = _model.transform.position - _offset;
	}
}

using UnityEngine;

internal class LocalUnitBuilder {
	public delegate Transform InstantiateDelegate ();

	private readonly Axis _axisX;
	private readonly Axis _axisY;

	private readonly SerifPool _serifPool;

	private Vector2 _center;
	private float _equivalentUnit;

	public LocalUnitBuilder (Axis axisX, Axis axisY, SerifPool serifPool) {
		_axisX = axisX;
		_axisY = axisY;
		_serifPool = serifPool;
	}

	public float EquivalentUnit => _equivalentUnit;

	public void Init (Vector2 center) {
		_center = center;
	}

	public void Draw (Vector3 worldCoordinate) {
		SetWorldUnit (worldCoordinate);

		_serifPool.Reset ();
		_serifPool.SetSerif (_center);

		DrawRightWingOfSerif (_axisX);
		DrawLeftWingOfSerif (_axisX);

		DrawRightWingOfSerif (_axisY);
		DrawLeftWingOfSerif (_axisY);
	}

	private void SetWorldUnit (Vector3 worldCoordinate) {
		var unit1 = GetUnitCoordinateOnAxis (_axisX, _axisY, worldCoordinate);
		var unit2 = GetUnitCoordinateOnAxis (_axisY, _axisX, worldCoordinate);

		_equivalentUnit = Mathf.Max (unit1, unit2);
	}

	private float GetUnitCoordinateOnAxis (Axis mainAxis, Axis guideAxis, Vector3 worldCoordinate) {
		var pointOnAxis = mainAxis.GetPointProjectOnAxis (worldCoordinate, guideAxis);
		return Vector2.Distance (_center, pointOnAxis);
	}

	private void DrawRightWingOfSerif (Axis axis) {
		for (int i = 1; ; i++) {
			if (IsSerifCreated (axis, i, out Vector2 point) == false) {
				break;
			}

			_serifPool.SetSerif (point);
		}
	}

	private void DrawLeftWingOfSerif (Axis axis) {
		for (int i = -1; ; i--) {
			if (IsSerifCreated (axis, i, out Vector2 point) == false) {
				break;
			}

			_serifPool.SetSerif (point);
		}
	}

	private bool IsSerifCreated (Axis axis, int i, out Vector2 outputPoint) {
		outputPoint = GetWorldPointOnAxis (axis, i);

		return _serifPool.CanSetSerif (outputPoint);
	}

	private Vector2 GetWorldPointOnAxis (Axis axis, int coordinateOnAxis) {
		return _center + coordinateOnAxis * EquivalentUnit * axis.NormalizedVector;
	}
}

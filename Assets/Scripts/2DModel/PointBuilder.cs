using UnityEngine;

internal class PointBuilder {
	public delegate void SetTextOnRegularPointDelegate (Transform regularPoint, string text);
	public delegate void FirstPointWasPlanted ();
	public delegate Transform InstantiateTDelegate ();
	public delegate LineRenderer InstantiateLRDelegate ();

	public event FirstPointWasPlanted OnFirstPointWasPlanted;

	private readonly PointStorage _points;

	private readonly SetTextOnRegularPointDelegate _setTextOnRegularPoint;
	private readonly InstantiateTDelegate _createPoint;
	private readonly InstantiateTDelegate _createStartPoint;
	private readonly InstantiateTDelegate _createFinishPoint;
	private readonly InstantiateLRDelegate _createGuide;

	private CoordinateSystem _coordinateSystem;
	private Vector2 _center;

	private (Transform point, int id) _finishFlagPoint;
	private (Transform point, int id) _currentPoint;
	private GuideLineVisualisation _visual1;
	private GuideLineVisualisation _visual2;

	public PointBuilder (
		PointStorage points,
		SetTextOnRegularPointDelegate setTextOnRegularPoint,
		InstantiateTDelegate createPoint,
		InstantiateTDelegate createStartPoint,
		InstantiateTDelegate createFinishPoint,
		InstantiateLRDelegate createGuide
	) {
		_points = points;
		_setTextOnRegularPoint = setTextOnRegularPoint;
		_createStartPoint = createStartPoint;
		_createFinishPoint = createFinishPoint;
		_createPoint = createPoint;
		_createGuide = createGuide;
	}

	public void Init (Vector2 center, CoordinateSystem coordinateSystem) {
		_center = center;
		_coordinateSystem = coordinateSystem;

		CreateNewSet ();
	}

	public void PutPoint (Vector2 worldCoordinate) {
		var newCoordinate = _coordinateSystem.ConvertToLocal (_center, worldCoordinate);

		_points.Push (newCoordinate);
		_points.SaveToXML ();

		if (_currentPoint.id == 1) {
			OnFirstPointWasPlanted?.Invoke ();
		}

		if (_currentPoint.id > 2) {
			SwapFinishFlagAndNextPoint ();
			SetTextOnRegularPoint ();
		}

		CreateNewSet ();
	}

	public void ShowPoint (Vector2 worldCoordinate) {
		_currentPoint.point.localPosition = worldCoordinate;

		var (pointOnX, pointOnY) = _coordinateSystem.GetWorldPointOnAxis (_center, worldCoordinate);

		_visual1.ShowGuideLine (worldCoordinate, pointOnX);
		_visual2.ShowGuideLine (worldCoordinate, pointOnY);
	}

	private void SwapFinishFlagAndNextPoint () {
		var (point, id) = _finishFlagPoint;
		var lastPostion = point.localPosition;

		_finishFlagPoint.point.localPosition = _currentPoint.point.localPosition;
		_finishFlagPoint.id = _currentPoint.id;

		_currentPoint.point.localPosition = lastPostion;
		_currentPoint.id = id;
	}
	
	private void CreateNewSet () {
		if (_points.HasStart == false && _points.HasPath == false) {
			CreateFirstPoint ();
		}

		if (_points.HasStart && _points.HasPath == false) {
			CreateSecondPoint ();
		}

		if (_points.HasPath) {
			CreateRegularPoint ();
		}

		CreateGuideLines ();
	}

	private void CreateFirstPoint () {
		_currentPoint.point = _createStartPoint ();
		_currentPoint.id = 1;
	}

	private void CreateSecondPoint () {
		_currentPoint.point = _createFinishPoint ();
		_currentPoint.id = 2;

		_finishFlagPoint = _currentPoint;
	}

	private void CreateRegularPoint () {
		var point = _createPoint ();

		_currentPoint.point = point;
		_currentPoint.id = _points.LastIndex + 1;

		SetTextOnRegularPoint ();
	}

	private void SetTextOnRegularPoint () {
		_setTextOnRegularPoint (_currentPoint.point, _currentPoint.id.ToString ());
	}

	private void CreateGuideLines () {
		_visual1 = new GuideLineVisualisation (_createGuide ());
		_visual2 = new GuideLineVisualisation (_createGuide ());
	}
}

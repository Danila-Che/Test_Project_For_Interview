using UnityEngine;
using UnityEngine.UI;

internal class CoordinateSystemPresenter: MonoBehaviour {
	private enum CoordinateSystemDrawingStatus {
		SetupAxesX,
		SetupAxesY,
		SetupUnitsOfMeasure,
		InputSpeed,
		PutPoints,
	}

	[Header ("Две оси")]
	[SerializeField] private Axis _axisX;
	[Space]
	[SerializeField] private Axis _axisY;
	[Header ("Настройка границ координатной системы")]
	[SerializeField, Range (0.0f, 1.0f)] private float _borderScale;
	[Header ("Объекты, которые нужно инициализировать, когда создастся координатная система")]
	[SerializeField] private PointModel _pointModel;
	[SerializeField] private RealModel _realModel;
	[SerializeField] private Model3DEnviroment _model3DEnviroment;
	[Header ("Префабы (элемнты координатной системы)")]
	[SerializeField] private GameObject _serifPrefab;
	[SerializeField] private GameObject _pointPrefab;
	[SerializeField] private GameObject _firstPointPrefab;
	[SerializeField] private GameObject _lastPointPrefab;
	[SerializeField] private LineRenderer _guideLinePrefab;
	[Header ("Родитель новых объектов")]
	[SerializeField] private Transform _dynamicParent;
	[Header ("Различные меню")]
	[SerializeField] private GameObject _speedInputMenu;
	[SerializeField] private GameObject _movementPointMenu;
	[SerializeField] private GameObject _switchingMenu;

	private AxisBuilder _axisBuilderX;
	private AxisBuilder _axisBuilderY;

	private Border _border;

	private SerifPool _serifPool;
	private LocalUnitBuilder _localUnitBuilder;

	private CoordinateSystem _coordinateSystem;

	private PointStorage _points;
	private PointBuilder _pointBuilder;

	private float _speed;

	private Camera _camera;
	private Vector3 _worldCoordinate;

	private CoordinateSystemDrawingStatus _status = CoordinateSystemDrawingStatus.SetupAxesX;

	private const int LEFT_MOUSE_CLICK_EVENT = 0;

	private void Start () {
		_camera = Camera.main;

		float halfHeight = _camera.orthographicSize;
		float halfWidth = _camera.aspect * halfHeight;

		_border = new Border (halfWidth: halfWidth * _borderScale, halfHeight: halfHeight * _borderScale);

		InitAxis (_axisX, new Vector2 (-halfWidth, 0), new Vector2 (halfWidth, 0));

		_axisBuilderX = new AxisBuilder (_axisX);
		_axisBuilderX.OnEndDrawing += () => _status = CoordinateSystemDrawingStatus.SetupAxesY;

		InitAxis (_axisY, new Vector2 (0, -halfHeight), new Vector2 (0, halfHeight));

		_axisBuilderY = new AxisBuilder (_axisY);
		_axisBuilderY.OnEndDrawing += () => _status = CoordinateSystemDrawingStatus.SetupUnitsOfMeasure;
		_axisBuilderY.OnEndDrawing += () => _localUnitBuilder.Init (GetCenter ());

		InitUnitBuilder ();
		InitPointBuilder ();
	}

	private void Update () {
		SetCoordinate ();

		if (_border.IsPointInside (_worldCoordinate) == false) {
			return;
		}

		switch (_status) {
			case CoordinateSystemDrawingStatus.SetupAxesX:
				_axisBuilderX.Drawing (_worldCoordinate);
				break;
			case CoordinateSystemDrawingStatus.SetupAxesY:
				_axisBuilderY.Drawing (_worldCoordinate);
				break;
			case CoordinateSystemDrawingStatus.SetupUnitsOfMeasure:
				_localUnitBuilder.Draw (_worldCoordinate);
				break;
			case CoordinateSystemDrawingStatus.PutPoints:
				_pointBuilder.ShowPoint (_worldCoordinate);
				break;
		}

		CheckMouseDown ();
	}

	public void ReadSpeed (InputField speedField) {
		bool result = float.TryParse (speedField.text, out float speed);

		if (result == false) {
			Debug.LogWarning ("Введенное значение не является форматом числа");

			return;
		}

		_speed = speed;
		_speedInputMenu.SetActive (false);
	}

	private void SetCoordinate () {
		_worldCoordinate = _camera.ScreenToWorldPoint (Input.mousePosition);
		_worldCoordinate.z = 0.0f;
	}

	private void CheckMouseDown () {
		if (Input.GetMouseButtonDown (LEFT_MOUSE_CLICK_EVENT)) {
			switch (_status) {
				case CoordinateSystemDrawingStatus.SetupAxesX:
					_axisBuilderX.ExecuteClick (_worldCoordinate);
					break;
				case CoordinateSystemDrawingStatus.SetupAxesY:
					_axisBuilderY.ExecuteClick (_worldCoordinate);
					break;
				case CoordinateSystemDrawingStatus.SetupUnitsOfMeasure:
					_status = CoordinateSystemDrawingStatus.InputSpeed;

					ShowSpeedMenu ();
					break;
				case CoordinateSystemDrawingStatus.InputSpeed:
					_status = CoordinateSystemDrawingStatus.PutPoints;

					InitCoordinateSystem ();
					_pointBuilder.Init (GetCenter (), _coordinateSystem);
					OpenMovementPointMenu ();
					break;
				case CoordinateSystemDrawingStatus.PutPoints:
					_pointBuilder.PutPoint (_worldCoordinate);
					break;
			}
		}
	}

	private void InitAxis (Axis axis, Vector2 start, Vector2 end) {
		axis.Init (
			border: _border,
			start: start,
			end: end
		);

		axis.FitWithinBorders ();
	}

	private void InitUnitBuilder () {
		_serifPool = new SerifPool (_border, () => Instantiate (_serifPrefab, _dynamicParent).transform);
		_localUnitBuilder = new LocalUnitBuilder (_axisX, _axisY, _serifPool);
	}

	private void InitPointBuilder () {
		_points = new PointStorage ();
		_pointBuilder = new PointBuilder (
			points: _points,
			setTextOnRegularPoint: (regularPoint, text) => regularPoint.GetComponentInChildren<Text> ().text = text,
			createPoint: () => Instantiate (_pointPrefab, _dynamicParent).transform,
			createStartPoint: () => Instantiate (_firstPointPrefab, _dynamicParent).transform,
			createFinishPoint: () => Instantiate (_lastPointPrefab, _dynamicParent).transform,
			createGuide: () => Instantiate (_guideLinePrefab, _dynamicParent)
		);

		_pointBuilder.OnFirstPointWasPlanted += () => _pointModel.Init (_speed, GetCenter (), _coordinateSystem, _points);
		_pointBuilder.OnFirstPointWasPlanted += () => _realModel.Init (_speed, _coordinateSystem, _points);
		_pointBuilder.OnFirstPointWasPlanted += () => _model3DEnviroment.Init (_coordinateSystem);
		_pointBuilder.OnFirstPointWasPlanted += () => _switchingMenu.SetActive (true);
	}

	private void InitCoordinateSystem () {
		_coordinateSystem = new CoordinateSystem (_axisX, _axisY, _localUnitBuilder.EquivalentUnit);
	}

	private void OpenMovementPointMenu () {
		_movementPointMenu.SetActive (true);
	}

	private Vector2 GetCenter () {
		return _axisX.GetIntersectionPoint (_axisY);
	}

	private void ShowSpeedMenu () {
		_speedInputMenu.SetActive (true);
	}
}

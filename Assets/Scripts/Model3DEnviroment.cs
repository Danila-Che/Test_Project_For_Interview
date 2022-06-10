using UnityEngine;
using UnityEngine.UI;

internal class Model3DEnviroment: MonoBehaviour, ICenter {
    [Header ("Настройка отображения системы координат")]
    [SerializeField, Range (1.0f, 10.0f)] private float _equivalentUnitsOfMeasure = 1;
    [SerializeField] private Vector2 _center;

    [Header ("Отслеживать положение")]
    [SerializeField] private RealModel _realModel;

    [Header ("Префабы")]
    [SerializeField] private GameObject _serifPrefab;
    [SerializeField] private Transform _dynamic;

    [Header ("Элементы системы координат")]
    [SerializeField] private AxisVisualisation _axisX;
    [SerializeField] private AxisVisualisation _axisY;
    [SerializeField] private LineRenderer _guideLine1;
    [SerializeField] private LineRenderer _guideLine2;
    [SerializeField] private Transform _coordinateInfo1;
    [SerializeField] private Transform _coordinateInfo2;

    private SerifPool _serifPool;
    private CoordinateSystem _coordinateSystem;
    private GuideLineVisualisation _guide1;
    private GuideLineVisualisation _guide2;

    public Vector2 Center => _center;

	public void Start () {
        _coordinateSystem.EquivalentUnitsOfMeasure = _equivalentUnitsOfMeasure;

        DrawRightWingOfSerif (_coordinateSystem.AxisX.NormalizedVector);
        DrawLeftWingOfSerif (_coordinateSystem.AxisX.NormalizedVector);

        DrawRightWingOfSerif (_coordinateSystem.AxisY.NormalizedVector);
        DrawLeftWingOfSerif (_coordinateSystem.AxisY.NormalizedVector);
    }

	public void Init (CoordinateSystem coordinateSystem) {
        _coordinateSystem = coordinateSystem;
        _serifPool = new SerifPool (new Border (100, 100), () => Instantiate (_serifPrefab, _dynamic).transform);

        ShowAxisLine (_coordinateSystem.AxisX, _axisX);
        ShowAxisLine (_coordinateSystem.AxisY, _axisY);

        _guide1 = new GuideLineVisualisation (_guideLine1);
        _guide2 = new GuideLineVisualisation (_guideLine2);
    }

    private void Update () {
        var currentPosition = GetCoordinateInCoordinateSystem (_realModel.transform.localPosition);

        var (pointOnX, pointOnY) = _coordinateSystem.GetWorldPointOnAxis (_center, currentPosition);

        DrawGuideLines (_guide1, pointOnX);
        DrawGuideLines (_guide2, pointOnY);

        var coordinate = _coordinateSystem.ConvertToLocal (_center, currentPosition);

        ShowCoordinateInfo (_coordinateInfo1, GetCoordinateOnFlat (pointOnX), coordinate.x);
        ShowCoordinateInfo (_coordinateInfo2, GetCoordinateOnFlat (pointOnY), coordinate.y);
    }

    private void ShowAxisLine (Axis axis, AxisVisualisation visual) {
        var point1 = GetCoordinateOnFlat (axis.NormalizedVector * 10);
        var point2 = GetCoordinateOnFlat (axis.NormalizedVector * -10);

        visual.ShowAxisLine (point1 * _equivalentUnitsOfMeasure, point2 * _equivalentUnitsOfMeasure, Vector3.up);
    }

    private Vector3 GetCoordinateOnFlat (Vector2 coordinate) {
        return new Vector3 (coordinate.x, 0.0f, coordinate.y);
    }

    private Vector2 GetCoordinateInCoordinateSystem (Vector3 coordinate) {
        return new Vector2 (coordinate.x, coordinate.z);
    }

    private void DrawGuideLines (GuideLineVisualisation guide, Vector2 pointOnAxis) {
        guide.ShowGuideLine (_realModel.transform.localPosition, GetCoordinateOnFlat (pointOnAxis));
    }

    private void DrawingCoordinateInfo (Vector2 worldPosition) {
        var (pointOnX, pointOnY) = _coordinateSystem.GetWorldPointOnAxis (_center, worldPosition);

        var coordinate = _coordinateSystem.ConvertToLocal (_center, worldPosition);

        ShowCoordinateInfo (_coordinateInfo1, pointOnX, coordinate.x);
        ShowCoordinateInfo (_coordinateInfo2, pointOnY, coordinate.y);
    }

    private void ShowCoordinateInfo (Transform _coordinateInfo, Vector3 worldCoordinateOnAxis, float coordinate) {
        _coordinateInfo.localPosition = worldCoordinateOnAxis;
        _coordinateInfo.GetComponentInChildren<Text> ().text = coordinate.ToString ("N2");
    }

    private void DrawRightWingOfSerif (Vector2 direction) {
        for (int i = 1; i <= 20; i++) {
            _serifPool.SetSerif (GetCoordinateOnFlat (_center) + GetCoordinateOnFlat (direction) * i * _coordinateSystem.EquivalentUnitsOfMeasure);
        }
    }

    private void DrawLeftWingOfSerif (Vector2 direction) {
        for (int i = -1; i >= -20; i--) {
            _serifPool.SetSerif (GetCoordinateOnFlat (_center) + GetCoordinateOnFlat (direction) * i * _coordinateSystem.EquivalentUnitsOfMeasure);
        }
    }
}

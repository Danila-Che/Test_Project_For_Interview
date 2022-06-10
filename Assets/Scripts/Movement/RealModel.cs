using UnityEngine;

internal class RealModel: MoveableModel {
    [Header ("Анимация")]
    [SerializeField] private Tween _tween;
    [Header ("Переменные окружения")]
    [SerializeField] private Model3DEnviroment _enviroment;

    private ICenter _coordinateSystemCenter;

    protected override void OnInit () {
        _coordinateSystemCenter = _enviroment;

        _center = _coordinateSystemCenter.Center;
    }

    protected override void OnUpdate () {
        TurnModelByDirection ();
    }

    protected override Vector3 GetCoordinateOnFlat (Vector2 coordinate) => new Vector3 (coordinate.x, 0.0f, coordinate.y);

    protected override void OnMoveToNextPoint () {
        _tween.PlayTween (_transform.localPosition, _points.LastId);
    }

	public void OnMouseUpAsButton () {
        _points.LoadFromXML ();
        InitMovement ();

        if (_movement == null) {
            return;
        }

        if (_movement.InMovement) {
            return;
        }

        _tween.PlayTween (_movement.WorldPosition, 1);

        MoveAllPathThrough ();
    }

    private void OnTriggerEnter (Collider _) {
        Debug.LogError ("Столкновение двигающегося объекта с посторонним объектом");
    }

    private void TurnModelByDirection () {
        var end = new Vector3 (_movement.EndCoordinate.x, 0, _movement.EndCoordinate.y);
        var start = new Vector3 (_movement.StartCoordinate.x, 0, _movement.StartCoordinate.y);
        var direction = (end - start).normalized;
        _transform.localRotation = Quaternion.LookRotation (direction, Vector3.up);
    }
}

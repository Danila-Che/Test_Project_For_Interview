using System.Collections;
using UnityEngine;
using UnityEngine.UI;

internal class PointModel : MoveableModel {
    [Header ("Метки для отображения времени пути")]
    [SerializeField] private Text _timeTextFromPoint;
    [SerializeField] private Text _timeTextToPoint;

	protected override void OnInit () {
        _movement.OnEndOfWay += () => _timeTextFromPoint.text = "--";
        _movement.OnEndOfWay += () => _timeTextToPoint.text = "--";

        gameObject.SetActive (true);
    }

    protected override void OnUpdate () {
        SetTimeInWayText ();
    }

    protected override Vector3 GetCoordinateOnFlat (Vector2 coordinate) => coordinate;

    protected override void OnMoveToNextPoint () { }

    private void SetTimeInWayText () {
        _timeTextFromPoint.text = (Vector2.Distance (_movement.StartCoordinate, _transform.localPosition) / _movement.WorldSpeed).ToString ("N1");
        _timeTextToPoint.text = (Vector2.Distance (_transform.localPosition, _movement.EndCoordinate) / _movement.WorldSpeed).ToString ("N1");
    }
}

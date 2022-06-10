using System.Collections;
using UnityEngine;

internal abstract class MoveableModel: MonoBehaviour {
    protected Movement _movement;
    protected Transform _transform;

    protected PointStorage _points;
    protected Vector2 _center;
    private CoordinateSystem _coordinateSystem;
    private float _localSpeed;

    public void Init (float speed, CoordinateSystem coordinateSystem, PointStorage points) {
        _points = points;
        _transform = transform;
        _localSpeed = speed;
        _coordinateSystem = coordinateSystem;

        OnInit ();
    }

    public void Init (float speed, Vector2 center, CoordinateSystem coordinateSystem, PointStorage points) {
        _points = points;
        _transform = transform;
        _localSpeed = speed;
        _coordinateSystem = coordinateSystem;

        _center = center;

        InitMovement ();

        OnInit ();
    }

    protected abstract void OnInit ();

    private void Update () {
        if (_movement == null) {
            return;
        }

        if (_movement.InMovement) {
            OnUpdate ();

            var newPosition = _movement.Move (Time.deltaTime);
            _transform.localPosition = GetCoordinateOnFlat (newPosition);
        }
    }

    protected abstract void OnUpdate ();
    protected abstract Vector3 GetCoordinateOnFlat (Vector2 coordinate);

    public void MoveToNextPoint () {
        if (_points.HasPath == false) {
            return;
        }

        if (_movement.InMovement) {
            return;
        }

        if (_points.CanGetPoint (out Vector2 outputPoint)) {
            _movement.MoveToNextPoint (outputPoint);
            OnMoveToNextPoint ();
        }
    }

    public void MoveAllPathThrough () {
        if (_points.HasPath == false) {
            return;
        }

        if (_movement.IsWaitStart == false) {
            return;
        }

        StartCoroutine (MoveFromPointToPoint ());
    }

    private IEnumerator MoveFromPointToPoint () {
        Vector2 outputPoint = default;

        while (true) {
            yield return new WaitUntil (() => _points.CanGetPoint (out outputPoint));
            _movement.MoveToNextPoint (outputPoint);
            OnMoveToNextPoint ();
            yield return new WaitUntil (() => _movement.CanMakeNextMove);
        }
    }

    protected abstract void OnMoveToNextPoint ();

    protected void InitMovement () {
        if (_points.CanGetPoint (out Vector2 firstPoint)) {
            _movement = new Movement (_coordinateSystem, _center, firstPoint, _localSpeed);
            _movement.InitStart ();
            _transform.localPosition = GetCoordinateOnFlat (_movement.WorldPosition);
        }
    }
}

using UnityEngine;

internal class Movement {
    private enum MovementStatus {
        WaitStart,
        Move,
        WaitForNextMove,
    }

    public delegate void EndOfWayDelegate ();
    public event EndOfWayDelegate OnEndOfWay;


    private readonly CoordinateSystem _coordinateSystem;
    private Vector2 _center;
    private Vector2 _startCoordinate;
    private readonly float _localSpeed;

    private Vector2 _startLocalPosition;
    private Vector2 _currentLocalPosition;
    private Vector2 _localTarget;

    private MovementStatus _status = MovementStatus.WaitStart;

    public Movement (CoordinateSystem coordinateSystem, Vector2 center, Vector2 startCoordinate, float localSpeed) {
		_coordinateSystem = coordinateSystem;
        _center = center;
        _startCoordinate = startCoordinate;
		_localSpeed = localSpeed;
	}

    public Vector2 LocalPosition => _currentLocalPosition;
    public Vector2 WorldPosition => _coordinateSystem.ConvertToWorld (_center, _currentLocalPosition);

    public bool CanMakeNextMove => _status == MovementStatus.WaitForNextMove;
    public bool InMovement => _status == MovementStatus.Move;
    public bool IsWaitStart => _status == MovementStatus.WaitStart;
    public Vector2 StartCoordinate => _coordinateSystem.ConvertToWorld (_center, _startLocalPosition);
    public Vector2 EndCoordinate => _coordinateSystem.ConvertToWorld (_center, _localTarget);
    public float WorldSpeed => _coordinateSystem.WorldSpeed (_localSpeed);

    public Vector2 Move (float deltaTime) {
        if (_status != MovementStatus.Move) {
            return WorldPosition;
        }

        CheckEndOfWay ();

        _currentLocalPosition = Vector2.MoveTowards (_currentLocalPosition, _localTarget, deltaTime * _localSpeed);
        return WorldPosition;
    }

	public void MoveToNextPoint (Vector2 localTarget) {
        _status = MovementStatus.Move;

        _startLocalPosition = _localTarget;
        _localTarget = localTarget;
    }

    public void InitStart () {
        _currentLocalPosition = _startCoordinate;
        _localTarget = _startCoordinate;
    }

    private void CheckEndOfWay () {
        if (_currentLocalPosition != _localTarget) {
            return;
        }

        _status = MovementStatus.WaitForNextMove;
        OnEndOfWay?.Invoke ();
    }
}

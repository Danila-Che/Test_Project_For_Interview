using UnityEngine;

internal class Border {
    private readonly float _halfWidth;
    private readonly float _halfHeight;

    private readonly Side _up;
    private readonly Side _down;
    private readonly Side _left;
    private readonly Side _right;

    public Border (float halfWidth, float halfHeight) {
        _halfWidth = halfWidth;
        _halfHeight = halfHeight;

        var leftUp = new Vector2 (-halfWidth, halfHeight);
        var rightUp = new Vector2 (halfWidth, halfHeight);
        var leftDown = new Vector2 (-halfWidth, -halfHeight);
        var rightDown = new Vector2 (halfWidth, -halfHeight);

        _up = new Side (leftUp, rightUp);
        _down = new Side (leftDown, rightDown);
        _left = new Side (leftDown, leftUp);
        _right = new Side (rightDown, rightUp);
    }

    public bool IsPointInside (Vector2 point) {
        return -_halfWidth <= point.x && point.x <= _halfWidth
            && -_halfHeight <= point.y && point.y <= _halfHeight;
    }

	public (Vector2 firstPoint, Vector2 secondPoint) GetTwoPointsLaedOnBorders (Vector2 startPosition, Vector2 endPostion) {
        var newPoints = new Vector2 [2];
        var id = 0;
        var line = new Line (startPosition, endPostion);

		if (_up.IsLiesWithinInterval (line, out Vector2 newPoint)) {
			newPoints [id] = newPoint;
			id++;
		}

		if (_down.IsLiesWithinInterval (line, out newPoint)) {
            newPoints [id] = newPoint;
            id++;
        }

        if (_left.IsLiesWithinInterval (line, out newPoint)) {
            newPoints [id] = newPoint;
            id++;
        }

        if (_right.IsLiesWithinInterval (line, out newPoint)) {
            newPoints [id] = newPoint;
        }

        return (newPoints [0], newPoints [1]);
    }
}

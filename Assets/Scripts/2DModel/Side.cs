using UnityEngine;

internal class Side: Line {
	public Side (Vector2 point1, Vector2 point2) : base (point1, point2) { }

	public bool IsLiesWithinInterval (Line line, out Vector2 outputPoint) {
        var point = GetIntersectionPoint (line);

        outputPoint = point;

        if (Point1.x == Point2.x) {
            return IsValueBetween (point.y, Point1.y, Point2.y);
        }

        if (Point1.y == Point2.y) {
            return IsValueBetween (point.x, Point1.x, Point2.x);
        }

        return false;
    }

    private bool IsValueBetween (float value, float a, float b) {
        var min = Mathf.Min (a, b);
        var max = Mathf.Max (a, b);

        if (min < value && value < max) {
            return true;
        }

        return false;
    }
}

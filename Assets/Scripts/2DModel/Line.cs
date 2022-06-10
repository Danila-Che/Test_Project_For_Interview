using UnityEngine;

internal class Line {
    protected Vector2 _point1;
    protected Vector2 _point2;

	public Line (Vector2 point1, Vector2 point2) {
		_point1 = point1;
		_point2 = point2;
	}

    public Vector2 Point1 => _point1;
    public Vector2 Point2 => _point2;

    public Vector2 GetIntersectionPoint (Line line) {
        return GetIntersectionPoint (line._point1, line._point2);
    }

    private Vector2 GetIntersectionPoint (Vector2 point1, Vector2 point2) {
        var x1 = point1.x;
        var y1 = point1.y;

        var x2 = point2.x;
        var y2 = point2.y;

        var x3 = _point1.x;
        var y3 = _point1.y;

        var x4 = _point2.x;
        var y4 = _point2.y;

        var result = Vector2.zero;

        // находит пересечение двух прямых линий по формуле:
        // https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection

        result.x =
            ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4))
            / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

        result.y =
            ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4))
            / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

        return result;
    }
}

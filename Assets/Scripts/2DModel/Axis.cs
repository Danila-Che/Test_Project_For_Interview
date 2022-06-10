using UnityEngine;

[System.Serializable]
internal class Axis: Line {
    [SerializeField] private AxisVisualisation _visual;
    private Border _border;

    public void Init (Border border, Vector2 start, Vector2 end) {
        _border = border;

        StartEdge = start;
        EndEdge = end;

        _visual.ShowAxisLine (this, Vector3.forward);
    }

    public Axis (AxisVisualisation visual, Border border, Vector2 start, Vector2 end) : base (start, end) {
        _visual = visual;

        Init (border, start, end);
    }

    public Vector2 NormalizedVector => (EndEdge - StartEdge).normalized;

    public Vector2 StartEdge { get => _point1; private set => _point1 = value; }
    public Vector2 EndEdge { get => _point2; private set => _point2 = value; }

    public void SartDrawing (Vector2 point) {
        StartEdge = EndEdge = point;

        _visual.ShowAxisLine (this, Vector3.forward);
    }

    public void SetEndPoint (Vector2 point) {
        EndEdge = point;
        _visual.ShowAxisLine (this, Vector3.forward);
    }

    public void FitWithinBorders () {
        var (firstPoint, secondPoint) = _border.GetTwoPointsLaedOnBorders (StartEdge, EndEdge);

        if (StartEdge.x < EndEdge.x && firstPoint.x < secondPoint.x || StartEdge.x > EndEdge.x && firstPoint.x > secondPoint.x) {
            StartEdge = firstPoint;
            EndEdge = secondPoint;
        } else {
            StartEdge = secondPoint;
            EndEdge = firstPoint;
        }

        _visual.ShowAxisLine (this, Vector3.forward);
    }

    public Vector2 GetPointProjectOnAxis (Vector2 point1, Axis guideAxis) {
        var direction = guideAxis.NormalizedVector;
        var point2 = point1 + direction;

        return GetIntersectionPoint (new Line (point1, point2));
    }
}

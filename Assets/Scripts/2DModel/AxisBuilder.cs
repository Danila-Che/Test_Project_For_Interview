using UnityEngine;

internal class AxisBuilder {
    private enum AxisDrawingStatus {
        Wait,
        InProcess,
        End,
    }

    public delegate void OnEndDrawingDelegate ();

    public event OnEndDrawingDelegate OnEndDrawing;

    private readonly Axis _axis;
    private AxisDrawingStatus _status = AxisDrawingStatus.Wait;

    public AxisBuilder (Axis axis) {
		_axis = axis;
	}

    public bool IsActive => _status == AxisDrawingStatus.InProcess;

    public void Drawing (Vector2 worldCoordinate) {
        if (_status != AxisDrawingStatus.InProcess) {
            return;
        }

        _axis.SetEndPoint (worldCoordinate);
    }

    public void ExecuteClick (Vector2 worldCoordinate) {
        switch (_status) {
            case AxisDrawingStatus.Wait:
                _status = AxisDrawingStatus.InProcess;

                StartDrawing (worldCoordinate);
                break;
            case AxisDrawingStatus.InProcess:
                _status = AxisDrawingStatus.End;

                EndDrawing ();
                break;
        }
    }

    private void StartDrawing (Vector2 startPoint) {
        _axis.SartDrawing (startPoint);
    }

    private void EndDrawing () {
        _axis.FitWithinBorders ();

        OnEndDrawing?.Invoke ();
    }
}

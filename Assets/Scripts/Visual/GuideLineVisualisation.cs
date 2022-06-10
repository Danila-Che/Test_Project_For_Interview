using UnityEngine;

internal class GuideLineVisualisation {
	private LineRenderer _guideLine;

	public GuideLineVisualisation (LineRenderer guideLine) {
		_guideLine = guideLine;
	}

	public void ShowGuideLine (Vector3 point1, Vector3 point2) {
        _guideLine.SetPosition (0, point1);
        _guideLine.SetPosition (1, point2);
    }
}

using UnityEngine;

[System.Serializable]
internal class AxisVisualisation {
    [SerializeField] private LineRenderer _axis;
    [SerializeField] private Transform _arrowhead;

    public void ShowAxisLine (Axis axis, Vector3 forward) {
        ShowAxisLine (axis.StartEdge, axis.EndEdge, forward);
    }

    public void ShowAxisLine (Vector3 start, Vector3 end, Vector3 forward) {
        _axis.SetPosition (0, start);
        _axis.SetPosition (1, end);

        SetArrowheadAtTheEnd (end);
        TurnArrowhead (start, end, forward);
    }

    private void SetArrowheadAtTheEnd (Vector3 endPosition) {
        _arrowhead.localPosition = endPosition;
    }

    private void TurnArrowhead (Vector3 start, Vector3 end, Vector3 forward) {
        var direction = (end - start).normalized;
        _arrowhead.localRotation = Quaternion.LookRotation (forward, direction);
    }
}

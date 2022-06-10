using UnityEngine;

internal class CoordinateSystem {
    private readonly Axis _axisX;
    private readonly Axis _axisY;
    private float _equivalentUnitsOfMeasure;

	public CoordinateSystem (Axis axisX, Axis axisY, float equivalentUnitsOfMeasure) {
		_axisX = axisX;
		_axisY = axisY;
		_equivalentUnitsOfMeasure = equivalentUnitsOfMeasure;
	}

	public Axis AxisX => _axisX;
	public Axis AxisY => _axisY;

	public float EquivalentUnitsOfMeasure { get => _equivalentUnitsOfMeasure; set => _equivalentUnitsOfMeasure = value; } 

	public float WorldSpeed (float localSpeed) => localSpeed * _equivalentUnitsOfMeasure;

	public (Vector2 pointOnX, Vector2 pointOnY) GetWorldPointOnAxis (Vector2 center, Vector2 worldCoordinate) {
		var coordinate = ConvertToLocal (center, worldCoordinate);

		var worldPointOnX = ConvertToWorld (center, new Vector2 (coordinate.x, 0.0f));
		var worldPointOnY = ConvertToWorld (center, new Vector2 (0.0f, coordinate.y));

		return (worldPointOnX, worldPointOnY);
	}

	public Vector2 ConvertToLocal (Vector2 center, Vector2 worldCoordinate) {
		// разложение вектора по базисным векторам (axisX и axisY)

		var vectorX = _axisX.NormalizedVector;
		var vectorY = _axisY.NormalizedVector;

		var vectorTarget = worldCoordinate - center;

		// решение СЛАУ
		// vectorX.x * X + vectorY.x * Y = vectorTarget.x
		// vectorX.y * X + vectorY.y * Y = vectorTarget.y

		var k = vectorX.y / vectorX.x;

		var y = (vectorTarget.y - vectorTarget.x * k) / (vectorY.y - vectorY.x * k);
		var x = (vectorTarget.x - vectorY.x * y) / (vectorX.x);

		return new Vector2 (x, y) / _equivalentUnitsOfMeasure;
	}

	public Vector2 ConvertToWorld (Vector2 center, Vector2 localCoordinate) {
		var vectorX = _axisX.NormalizedVector * (localCoordinate.x * _equivalentUnitsOfMeasure);
		var vectorY = _axisY.NormalizedVector * (localCoordinate.y * _equivalentUnitsOfMeasure);

		return center + vectorX + vectorY;
	}
}

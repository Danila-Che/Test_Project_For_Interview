using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

internal class PointStorage {
    private readonly List<Vector2> _points;

    private int _currentId = 0;

    public PointStorage () {
        _points = new List<Vector2> ();
    }

    public PointStorage (List<Vector2> points) {
        _points = points;
    }

    public int LastIndex => _points.Count;
    public int LastId => _currentId;

    public Vector2 FirstPoint => _points.First ();
    public Vector2 LastPoint => _points.Last ();

    public bool HasPath => _points.Count >= 2;
    public bool HasStart => _points.Count == 1;

    public void Push (Vector2 point) {
        _points.Add (point);
    }

    public bool CanGetPoint (out Vector2 outputPoint) {
        if (_currentId >= _points.Count) {
            outputPoint = default;

            return false;
        }

        outputPoint = _points [_currentId];
        _currentId++;

        return true;
    }

    public void SaveToXML () {
		using var fileStream = new FileStream ("Assets/point.xml", FileMode.Create);
		var serializer = new XmlSerializer (typeof (List<Vector2>));
		serializer.Serialize (fileStream, _points);
	}

    public void LoadFromXML () {
		using var fileStream = new FileStream ("Assets/point.xml", FileMode.Open);
		var serializer = new XmlSerializer (typeof (List<Vector2>));

        _currentId = 0;
        _points.Clear ();
        _points.AddRange ((List <Vector2>) serializer.Deserialize (fileStream));
	}
}

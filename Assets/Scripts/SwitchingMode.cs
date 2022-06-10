using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
internal class Mode {
	[SerializeField] private GameObject _rootOfSpace;
	[SerializeField] private string labelText;

	public void Open (Text labelOnButton) {
		_rootOfSpace.SetActive (true);
		labelOnButton.text = labelText;
	}

	public void Close () {
		_rootOfSpace.SetActive (false);
	}
}

public class SwitchingMode: MonoBehaviour {
	private enum SwitchModes {
		Editor,
		Simulation,
	}

	[SerializeField] private Text _labelOnButton;
	[SerializeField] private SwitchModes _mode;
	[Header ("Настрока отображения")]
	[SerializeField] private Mode _simulationMode;
	[SerializeField] private Mode _editorMode;

	public void SwitchMode () {
		switch (_mode) {
			case SwitchModes.Simulation:
				_mode = SwitchModes.Editor;

				ApplyEditorMode ();
				break;
			case SwitchModes.Editor:
				_mode = SwitchModes.Simulation;

				ApplySimulationMode ();
				break;
		}
	}

	private void ApplyEditorMode () {
		_editorMode.Open (_labelOnButton);

		_simulationMode.Close ();
	}

	private void ApplySimulationMode () {
		_simulationMode.Open (_labelOnButton);

		_editorMode.Close ();
	}
}

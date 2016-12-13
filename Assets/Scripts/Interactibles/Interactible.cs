using UnityEngine;

/// <summary>
/// The Interactible class flags a Game Object as being "Interactible".
/// Determines what happens when an Interactible is being gazed at.
/// </summary>
public class Interactible : MonoBehaviour {

    [SerializeField]
    private InteractibleAction iAction;

    void GazeEntered() {
        if (iAction != null) iAction.ActionGazeEntered();
    }

    void GazeExited() {
        if (iAction != null) iAction.ActionGazeExited();
    }

    void OnSelect() {
        if (iAction != null) iAction.ActionOnSelect();
    }

    public void SetIAction(InteractibleAction iAction) {
        this.iAction = iAction;
    }

}
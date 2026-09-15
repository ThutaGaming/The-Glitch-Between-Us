using UnityEngine;

/// <summary>
/// Drives Room 3's two-stage objective: "reach the terminal" on entry, then "reach the door"
/// once the network puzzle is solved and the blast door finishes sliding open. Each stage lights
/// the relevant object with the same blue ObjectiveGlow outline every other mission marker in
/// this project uses. Wire <see cref="OnRoomEntered"/> to the Room2->Room3 door's
/// DoorAutoCloseZone.onPlayerEntered, and <see cref="OnDoorOpened"/> to
/// Room3BlastGateController.onOpened.
/// </summary>
public class Room3ObjectiveFlow : MonoBehaviour
{
    [SerializeField] private MissionHUD mission;
    [SerializeField] private ObjectiveGlow terminalGlow;
    [SerializeField] private ObjectiveGlow doorGlow;
    [SerializeField] private string terminalObjective = "Head to the terminal to open the door";
    [SerializeField] private string doorObjective = "Head to the door";

    public void OnRoomEntered()
    {
        if (mission != null) mission.SetObjective(terminalObjective);
        if (terminalGlow != null) terminalGlow.SetGlowing(true);
    }

    public void OnDoorOpened()
    {
        if (terminalGlow != null) terminalGlow.SetGlowing(false);
        if (mission != null) mission.SetObjective(doorObjective);
        if (doorGlow != null) doorGlow.SetGlowing(true);
    }
}

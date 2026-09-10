using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using InfimaGames.LowPolyShooterPack;
public class TrainingWeaponBox : MonoBehaviour
{
    public Transform lid;
    public Collider boxTarget;
    public Collider[] gunTargets;
    public GameObject[] gunDisplays;
    public WeaponBehaviour[] prefabs;
    public string[] labels;
    public bool IsOpen {get;private set;}
    public bool Busy {get;private set;}
    public int CollectedCount {get;private set;}
    Character player;
    TrainingBedIntro intro;
    Quaternion closed;
    int target=-2;
    GUIStyle style;
    void Awake(){closed=lid.localRotation;IsOpen=false; foreach(var c in gunTargets)c.enabled=false;}
    void Start(){player=FindFirstObjectByType<Character>();intro=player.GetComponent<TrainingBedIntro>();}
    public int FindTarget()
    {
        if(!player || Busy || (intro && !intro.Complete))return -2;
        var camera=player.GetCameraWorld();
        // RaycastAll, not Raycast: looking down into the open box, the crate's own solid rim/wall
        // sits closer to the camera than the gun triggers and would otherwise win a single-hit
        // raycast, making the guns unreachable from any but a narrow, level viewing angle.
        var hits=Physics.RaycastAll(camera.transform.position,camera.transform.forward,3f,~0,QueryTriggerInteraction.Collide);
        if(!IsOpen)
        {
            foreach(var h in hits) if(h.collider==boxTarget || h.collider.transform.IsChildOf(lid))return -1;
            return -2;
        }
        foreach(var h in hits)
            for(int i=0;i<gunTargets.Length;i++)
                if(gunTargets[i].enabled && h.collider==gunTargets[i])return i;
        return -2;
    }
    void Update()
    {
        target=FindTarget();
        if(target!=-2 && Keyboard.current!=null && (target==-1 ? Keyboard.current.gKey.wasPressedThisFrame : Keyboard.current.eKey.wasPressedThisFrame))TryInteract();
    }
    public bool TryInteract()
    {
        int selected=FindTarget();
        if(selected==-2)return false;
        if(selected==-1){StartCoroutine(Open());return true;}
        if(!player.AcquirePickup(prefabs[selected]))return false;
        gunTargets[selected].enabled=false;
        gunDisplays[selected].SetActive(false);
        CollectedCount++;target=-2;
        return true;
    }
    IEnumerator Open()
    {
        Busy=true;
        var end=closed*Quaternion.Euler(-105,0,0);
        for(float t=0;t<.8f;t+=Time.deltaTime){lid.localRotation=Quaternion.Slerp(closed,end,Mathf.SmoothStep(0,1,t/.8f));yield return null;}
        lid.localRotation=end;IsOpen=true;Busy=false;
        foreach(var c in gunTargets)c.enabled=true;
    }
    void OnGUI()
    {
        if(target==-2)return;
        if(style==null)style=new GUIStyle(GUI.skin.box){fontSize=23,alignment=TextAnchor.MiddleCenter};
        GUI.Box(new Rect(Screen.width/2-220,Screen.height-110,440,45),target==-1?"[G] Open weapon box":"[E] Pick up "+labels[target],style);
    }
}
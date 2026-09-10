using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using InfimaGames.LowPolyShooterPack;

public class TrainingCratePickup : MonoBehaviour
{
    public WeaponBehaviour weaponPrefab;
    public Transform lid;
    public GameObject displayWeapon;
    public string weaponLabel;
    public bool Collected { get; private set; }
    public bool Opened { get; private set; }
    Character player;
    TrainingBedIntro intro;
    bool busy, targeted;
    Quaternion closed;
    GUIStyle prompt;
    void Start()
    {
        player=FindFirstObjectByType<Character>();
        intro=player ? player.GetComponent<TrainingBedIntro>() : null;
        if(lid)closed=lid.localRotation;
    }
    void Update()
    {
        targeted=false;
        if(!player || Collected || busy || (intro && !intro.Complete))return;
        var camera=player.GetCameraWorld();
        Vector3 target=transform.position+Vector3.up*.45f;
        Vector3 delta=target-camera.transform.position;
        targeted=delta.magnitude<2.8f && Vector3.Angle(camera.transform.forward,delta)<48;
        if(!targeted)return;
        if(Keyboard.current!=null && Keyboard.current.eKey.wasPressedThisFrame)TryInteract();
    }
    public bool TryInteract()
    {
        if(!player || Collected || busy || (intro && !intro.Complete))return false;
        if(Vector3.Distance(player.GetCameraWorld().transform.position,transform.position+Vector3.up*.45f)>2.8f)return false;
        if(!Opened){StartCoroutine(Open());return true;}
        if(!player.AcquirePickup(weaponPrefab))return false;
        Collected=true;targeted=false;
        if(displayWeapon)displayWeapon.SetActive(false);
        Debug.Log("Collected from Crate_01: "+weaponLabel);
        return true;
    }
    IEnumerator Open()
    {
        busy=true;
        if(lid)
        {
            var end=closed*Quaternion.Euler(-105,0,0);
            for(float t=0;t<.65f;t+=Time.deltaTime){lid.localRotation=Quaternion.Slerp(closed,end,Mathf.SmoothStep(0,1,t/.65f));yield return null;}
            lid.localRotation=end;
        }
        Opened=true;busy=false;
    }
    void OnGUI()
    {
        if(!targeted)return;
        if(prompt==null){prompt=new GUIStyle(GUI.skin.box){fontSize=23,alignment=TextAnchor.MiddleCenter};}
        GUI.Box(new Rect(Screen.width/2-220,Screen.height-110,440,45),Opened?"[E] Pick up "+weaponLabel:"[E] Open weapon crate",prompt);
    }
}
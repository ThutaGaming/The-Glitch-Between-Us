using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using InfimaGames.LowPolyShooterPack;

[DefaultExecutionOrder(-50)]
public class TrainingBedIntro : MonoBehaviour
{
    public Transform bed;
    public Vector3 lyingEye, sittingEye, exitPosition;
    public float facingYaw;
    public Font dialogueFont;

    [Tooltip("Fires once, right after the player finishes standing up.")]
    public UnityEvent onComplete;

    public bool Complete { get; private set; }
    public string Phase { get; private set; }
    Camera view;
    Transform viewParent;
    Vector3 cameraLocal;
    Quaternion cameraRotation;
    Character character;
    Rigidbody body;
    CapsuleCollider capsule;
    Movement movement;
    CameraLook look;
    PlayerInput input;
    Animator armsAnimator;
    Renderer[] arms;
    bool movementEnabled, lookEnabled, inputEnabled, animatorEnabled, capsuleEnabled, wasKinematic;
    bool showDialogue;
    float darkness = 1;
    GUIStyle speakerStyle, lineStyle;

    void Awake()
    {
        character = GetComponent<Character>();
        view = character.GetCameraWorld();
        body = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        movement = GetComponent<Movement>();
        look = GetComponentInChildren<CameraLook>();
        input = GetComponent<PlayerInput>();
        armsAnimator = GetComponentInChildren<Animator>();
        arms = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        movementEnabled = movement.enabled; lookEnabled = look.enabled;
        inputEnabled = input.enabled; animatorEnabled = armsAnimator.enabled;
        capsuleEnabled = capsule.enabled; wasKinematic = body.isKinematic;
        movement.enabled = false; look.enabled = false; input.enabled = false;
        armsAnimator.enabled = false;
        foreach (var r in arms) r.enabled = false;
        body.isKinematic = true; capsule.enabled = false;
        viewParent = view.transform.parent;
        cameraLocal = view.transform.localPosition; cameraRotation = view.transform.localRotation;
        view.transform.SetParent(transform, true);
        transform.rotation = Quaternion.Euler(0, facingYaw, 0);
        view.transform.SetPositionAndRotation(lyingEye, Quaternion.Euler(-78, facingYaw, -4));
    }

    IEnumerator Start()
    {
        Phase = "Lying";
        yield return new WaitForSeconds(0.5f);
        for(float t=0;t<1.3f;t+=Time.deltaTime) { darkness=1-Mathf.SmoothStep(0,1,t/1.3f); yield return null; }
        darkness=0;
        Phase = "Sitting up";
        yield return MoveEye(sittingEye, Quaternion.Euler(0,facingYaw,0), 1.8f);
        Phase = "Thuta dialogue";
        showDialogue=true;

        // Small, hesitant glances convey surprise while seated.
        yield return MoveEye(sittingEye, Quaternion.Euler(-3,facingYaw-9,-1),0.5f);
        yield return new WaitForSeconds(1.2f);
        yield return MoveEye(sittingEye, Quaternion.Euler(2,facingYaw+8,1),0.7f);
        yield return new WaitForSeconds(1.5f);
        yield return MoveEye(sittingEye, Quaternion.Euler(0,facingYaw,0),0.5f);
        yield return new WaitForSeconds(0.6f);
        showDialogue=false;
        Phase="Turning right";
        yield return MoveEye(sittingEye,Quaternion.Euler(0,facingYaw+90,0),1.0f);
        var edge = new Vector3(exitPosition.x,sittingEye.y-0.12f,exitPosition.z);
        yield return MoveEye(edge,Quaternion.Euler(8,facingYaw+90,0),0.8f);
        Phase="Standing";
        var eyeBefore=view.transform.position;
        var rotBefore=view.transform.rotation;
        var exitRotation=Quaternion.Euler(0,facingYaw+90,0);
        transform.SetPositionAndRotation(exitPosition,exitRotation);
        // The rigidbody is still kinematic here, but drive it explicitly too - relying on the
        // Transform alone to reach the physics body left it holding the pre-move position across
        // this multi-second yield in some cases, so the player snapped back once it went dynamic.
        body.position=exitPosition; body.rotation=exitRotation;
        Physics.SyncTransforms();
        // Resolve the normal eye position from the original camera socket for a seamless handoff.
        var standingEye=viewParent.TransformPoint(cameraLocal);
        view.transform.SetPositionAndRotation(eyeBefore,rotBefore);
        yield return MoveEye(standingEye,Quaternion.Euler(0,facingYaw+90,0),1.2f);
        view.transform.SetParent(viewParent,false);
        view.transform.localPosition=cameraLocal; view.transform.localRotation=cameraRotation;
        // Re-affirm the root position/rotation right before handing control back, in case anything
        // during the yield (physics, animation) nudged the still-kinematic body.
        transform.SetPositionAndRotation(exitPosition,exitRotation);
        body.position=exitPosition; body.rotation=exitRotation;
        Physics.SyncTransforms();
        capsule.enabled=capsuleEnabled; body.isKinematic=wasKinematic;
        if(!wasKinematic)body.linearVelocity=Vector3.zero;
        movement.enabled=movementEnabled; look.enabled=lookEnabled; input.enabled=inputEnabled;
        Complete=true; Phase="Complete";
        Debug.Log("TrainingBedIntro complete: unarmed, standing at bed right side.");
        onComplete?.Invoke();
    }

    IEnumerator MoveEye(Vector3 end,Quaternion rotation,float duration)
    {
        var start=view.transform.position;var initial=view.transform.rotation;
        for(float t=0;t<duration;t+=Time.deltaTime)
        {
            float p=Mathf.SmoothStep(0,1,t/duration);
            view.transform.SetPositionAndRotation(Vector3.Lerp(start,end,p),Quaternion.Slerp(initial,rotation,p));
            yield return null;
        }
        view.transform.SetPositionAndRotation(end,rotation);
    }

    void LateUpdate()
    {
        if(!Complete)return;
        bool armed=character.GetInventory().GetEquipped()!=null;
        foreach(var r in arms)if(r)r.enabled=armed;
        armsAnimator.enabled=armed && animatorEnabled;
    }

    void OnGUI()
    {
        var old=GUI.color;
        if(darkness>0){GUI.color=new Color(0,0,0,darkness);GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Texture2D.whiteTexture);}
        GUI.color=old;
        if(!showDialogue)return;
        if(speakerStyle==null)
        {
            speakerStyle=new GUIStyle(GUI.skin.label){fontSize=22,fontStyle=FontStyle.Bold};
            speakerStyle.normal.textColor=new Color(1,.82f,.35f);
            lineStyle=new GUIStyle(GUI.skin.label){font=dialogueFont,fontSize=30,wordWrap=true};
            lineStyle.normal.textColor=Color.white;
        }
        float w=Mathf.Min(800,Screen.width-48),x=(Screen.width-w)/2,y=Screen.height-155;
        GUI.color=new Color(0,0,0,.78f);GUI.DrawTexture(new Rect(x,y,w,120),Texture2D.whiteTexture);GUI.color=old;
        GUI.Label(new Rect(x+24,y+10,w-48,32),"Thuta",speakerStyle);
        GUI.Label(new Rect(x+24,y+46,w-48,65),"Where... where am I?",lineStyle);
    }
}
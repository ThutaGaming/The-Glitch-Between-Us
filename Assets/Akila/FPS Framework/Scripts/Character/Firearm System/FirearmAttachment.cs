using UnityEngine;

namespace Akila.FPSFramework
{
    [RequireComponent(typeof(Attachment)), AddComponentMenu("")]
    public class FirearmAttachment : MonoBehaviour
    {
        public Firearm firearm { get; set; }
        public FirearmAttachmentsManager attachmentsManager { get; set; }
        public Attachment attachment { get; set; }
        public ItemInput itemInput { get; set; }

        protected virtual void Awake()
        {
            firearm = GetComponentInParent<Firearm>();
            attachmentsManager = GetComponentInParent<FirearmAttachmentsManager>();
            attachment = GetComponent<Attachment>();
            itemInput = GetComponentInParent<ItemInput>();
        }
    }
}
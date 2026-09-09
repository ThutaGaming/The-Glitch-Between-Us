using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Player/Pinger")]
    public class Pinger : MonoBehaviour
    {
        public InputAction inputAction;
        public LayerMask pingableLayers = -1;
        public Ping ping;
        public Canvas canvas;
        public float range = 100;
        public float pingLifetime = 15;
        public float maxPings = 5;

        public List<Ping> pings = new List<Ping>();

        private void Start()
        {
            inputAction.Enable();
            inputAction.performed += context => LookAndPing();
        }

        private void LookAndPing()
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range, pingableLayers))
            {
                Ping newPing = Instantiate(ping, canvas.transform);
                newPing.GetComponent<FloatingRect>().position = hit.point;
                pings.Add(newPing);

                while (pings.Count > maxPings)
                {
                    if (pings[0] != null)
                        Destroy(pings[0].gameObject);
                    pings.RemoveAt(0);
                }

                StartCoroutine(AutoDestroyPing(newPing));

                OnPinged(newPing);
            }
        }

        private IEnumerator AutoDestroyPing(Ping ping)
        {
            yield return new WaitForSeconds(pingLifetime);
            if (ping != null)
            {
                pings.Remove(ping);
                Destroy(ping.gameObject);
            }
        }


        public virtual void OnPinged(Ping ping)
        {

        }
    }
}

using UnityEngine;

namespace Akila.FPSFramework
{
    public static class FPSFrameworkUtility
    {
        public static Camera GetMainCamera()
        {
            if (Camera.main) return Camera.main;

            return GameObject.FindFirstObjectByType<Camera>();
        }

        public static Vector3 RandomVector3(Vector3 refrecne)
        {
            return new Vector3(Random.Range(-refrecne.x, refrecne.x), Random.Range(-refrecne.y, refrecne.y), Random.Range(-refrecne.z, refrecne.z));
        }

        public static bool IsVector3Nan(Vector3 vector)
        {
            if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z)) return true;

            return false;
        }
    }
}
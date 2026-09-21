using UnityEngine;
using System.Collections;
using Vuforia;  // Needed for RuntimeMeshRenderingBehaviour

public class HideTowerMeshesByComponent : MonoBehaviour
{
    IEnumerator Start()
    {
        // Wait one frame so Vuforia can create its runtime‐mesh objects
        yield return new WaitForEndOfFrame();

        // Try for up to 10 frames, in case some meshes appear a frame or two after tracking begins
        for (int attempt = 1; attempt <= 10; attempt++)
        {
            Debug.Log($"[HideTowerMeshesByComponent] Attempt {attempt}: Looking for RuntimeMeshRenderingBehaviour components…");

            // 1) Disable any Vuforia RuntimeMeshRenderingBehaviour
            var vuforiaRenderers =
                GetComponentsInChildren<RuntimeMeshRenderingBehaviour>(includeInactive: true);

            foreach (var vr in vuforiaRenderers)
            {
                if (vr.gameObject.activeSelf)
                {
                    Debug.Log($"[HideTowerMeshesByComponent] Disabling Vuforia mesh GO: '{vr.gameObject.name}' (path: {GetFullPath(vr.transform)})");
                    vr.gameObject.SetActive(false);
                }
            }

            // 2) Also disable any plain MeshRenderer (just in case Vuforia uses those)
            var plainRenderers =
                GetComponentsInChildren<MeshRenderer>(includeInactive: true);

            foreach (var mr in plainRenderers)
            {
                // Only disable mesh objects that still look pink/error‐shaded
                if (mr.gameObject.activeSelf && mr.sharedMaterial == null)
                {
                    // A missing sharedMaterial typically shows up pink in Editor. 
                    // If you want to be absolutely certain, remove this check:
                    // if you know all MeshRenderers under ModelTarget are from Vuforia, just disable them unconditionally.
                    Debug.Log($"[HideTowerMeshesByComponent] Disabling fallback mesh GO: '{mr.gameObject.name}' (path: {GetFullPath(mr.transform)})");
                    mr.gameObject.SetActive(false);
                }
            }

            // If nothing remains to disable, we can break early. Otherwise wait another frame.
            if (vuforiaRenderers.Length == 0 && plainRenderers.Length == 0)
            {
                Debug.Log("[HideTowerMeshesByComponent] No more mesh components found. Stopping attempts.");
                yield break;
            }

            yield return null;
        }

        Debug.LogWarning("[HideTowerMeshesByComponent] Finished 10 attempts. If pink still remains, some mesh wasn’t found by components.");
    }

    // Helper that builds the full “parent/child/.../this” path for debugging
    string GetFullPath(Transform t)
    {
        string path = t.name;
        var parent = t.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }
}

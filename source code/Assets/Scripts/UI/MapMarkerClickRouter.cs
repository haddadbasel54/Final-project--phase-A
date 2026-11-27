
using UnityEngine;
using OnlineMaps;

public class MapMarkerClickRouter : MonoBehaviour
{
    [Header("Routing")]
    [SerializeField] private SiteRouter siteRouter;

    private Marker2DManager manager;

    private void Start()
    {
        if (siteRouter == null)
        {
            siteRouter = FindFirstObjectByType<SiteRouter>();

            if (siteRouter == null)
            {
                Debug.LogError("[MapMarkerClickRouter] No SiteRouter found.");
                return;
            }
        }

        manager = Marker2DManager.instance ??
                  FindFirstObjectByType<Marker2DManager>();

        if (manager == null)
        {
            Debug.LogError("[MapMarkerClickRouter] Marker2DManager not found.");
            return;
        }

        if (manager.items != null)
        {
            Debug.Log("[MapMarkerClickRouter] Found " +
                      manager.items.Count + " markers.");

            foreach (Marker2D marker in manager.items)
            {
                marker.OnClick += OnMarkerClick;
            }
        }
    }

    private void OnDestroy()
    {
        if (manager == null || manager.items == null) return;

        foreach (Marker2D marker in manager.items)
        {
            marker.OnClick -= OnMarkerClick;
        }
    }

    private void OnMarkerClick(Marker marker)
    {
        if (siteRouter == null) return;

        string rawLabel = marker.label ?? "";
        string label = rawLabel.ToLowerInvariant();

        Debug.Log("[MapMarkerClickRouter] Marker clicked: " + rawLabel);

        string siteId = null;

        if (label.Contains("tower of flies") || label.Contains("tower"))
        {
            siteId = "tower_of_flies";
        }
        else if (label.Contains("khan el hamir") ||
                 label.Contains("khan al hameer") ||
                 label.Contains("khan"))
        {
            siteId = "khan_el_hamir";
        }
        else if (label.Contains("wall cannons") ||
                 label.Contains("cannon") ||
                 label.Contains("cannons"))
        {
            siteId = "wall_cannons";
        }

        if (!string.IsNullOrEmpty(siteId))
        {
            MarkVisitedOnAndroid(siteId);

            Debug.Log("[MapMarkerClickRouter] Routing to: " + siteId);

            siteRouter.ChooseSite(siteId);
        }
        else
        {
            Debug.LogWarning(
                "[MapMarkerClickRouter] No site mapping for: " + rawLabel);
        }
    }

    private void MarkVisitedOnAndroid(string siteId)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass plugin =
                   new AndroidJavaClass(
                       "com.example.sitegeofence.GeofencePlugin"))
            using (AndroidJavaClass unityPlayer =
                   new AndroidJavaClass(
                       "com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity =
                   unityPlayer.GetStatic<AndroidJavaObject>(
                       "currentActivity"))
            {
                plugin.CallStatic("markVisited", activity, siteId);
            }

            Debug.Log("[MapMarkerClickRouter] Saved visited site: " + siteId);
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "[MapMarkerClickRouter] Could not save visited site: " + e);
        }
#endif
    }
}

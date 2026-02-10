using UnityEngine;
using OnlineMaps; // Ensure this matches your plugin (Infinity Code Online Maps)

public class MapMarkerClickRouter : MonoBehaviour
{
    [Header("Routing")]
    [SerializeField] private SiteRouter siteRouter;

    private Marker2DManager manager;

    private void Start()
    {
        // 1) Find SiteRouter
        if (siteRouter == null)
        {
            siteRouter = FindFirstObjectByType<SiteRouter>();
            if (siteRouter == null)
            {
                Debug.LogError("[MapMarkerClickRouter] ❌ No SiteRouter found in scene.");
                return;
            }
        }

        // 2) Get Marker2DManager
        manager = Marker2DManager.instance ?? FindFirstObjectByType<Marker2DManager>();
        if (manager == null)
        {
            Debug.LogError("[MapMarkerClickRouter] ❌ Marker2DManager not found. " +
                           "Make sure 'Marker 2D Manager' is on the Map object.");
            return;
        }

        // 3) Subscribe to ALL current markers
        if (manager.items != null)
        {
            Debug.Log("[MapMarkerClickRouter] Found " + manager.items.Count + " markers. Subscribing...");
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

        Debug.Log("[MapMarkerClickRouter] 🟢 Marker clicked: " + rawLabel);

        string siteId = null;

        // --- MAPPING LOGIC ---
        if (label.Contains("tower of flies") || label.Contains("tower"))
        {
            siteId = "tower_of_flies";
        }
        else if (label.Contains("khan el hamir") || label.Contains("khan al hameer") || label.Contains("khan"))
        {
            siteId = "khan_el_hamir";
        }
        // NEW: Wall Cannons Mapping
        else if (label.Contains("wall cannons") || label.Contains("cannon") || label.Contains("cannons"))
        {
            siteId = "wall_cannons";
        }

        // --- ROUTING ---
        if (!string.IsNullOrEmpty(siteId))
        {
            Debug.Log("[MapMarkerClickRouter] 🚀 Routing to siteId = " + siteId);
            siteRouter.ChooseSite(siteId);
        }
        else
        {
            Debug.LogWarning("[MapMarkerClickRouter] No site mapping found for label: " + rawLabel);
        }
    }
}

package com.example.sitegeofence;

import android.Manifest;
import android.app.Activity;
import android.app.PendingIntent;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.os.Build;
import android.util.Log;

import com.google.android.gms.location.Geofence;
import com.google.android.gms.location.GeofencingClient;
import com.google.android.gms.location.GeofencingRequest;
import com.google.android.gms.location.LocationServices;

import java.util.ArrayList;
import java.util.List;

public class GeofencePlugin {

    private static final String TAG = "SiteGeofence";
    private static final String PREFS = "site_geofence_prefs";

    public static void registerGeofences(Activity activity) {

        if (activity.checkSelfPermission(
                Manifest.permission.ACCESS_FINE_LOCATION)
                != PackageManager.PERMISSION_GRANTED) {
            Log.e(TAG, "Fine location permission missing");
            return;
        }

        if (Build.VERSION.SDK_INT >= 29 &&
                activity.checkSelfPermission(
                        Manifest.permission.ACCESS_BACKGROUND_LOCATION)
                        != PackageManager.PERMISSION_GRANTED) {
            Log.e(TAG, "Background location permission missing");
            return;
        }

        GeofencingClient client =
                LocationServices.getGeofencingClient(activity);

        List<Geofence> fences = new ArrayList<>();

        fences.add(createFence(
                "tower_of_flies", 32.91919, 35.070781));

        fences.add(createFence(
                "khan_wall_group", 32.9232344, 35.0730018));

        GeofencingRequest request =
                new GeofencingRequest.Builder()
                        .setInitialTrigger(
                                GeofencingRequest.INITIAL_TRIGGER_ENTER)
                        .addGeofences(fences)
                        .build();

        Intent intent = new Intent(
                activity, SiteGeofenceReceiver.class);

        int flags = PendingIntent.FLAG_UPDATE_CURRENT;

        if (Build.VERSION.SDK_INT >= 31) {
            flags |= PendingIntent.FLAG_MUTABLE;
        }

        PendingIntent pendingIntent =
                PendingIntent.getBroadcast(
                        activity, 1001, intent, flags);

        try {
            client.addGeofences(request, pendingIntent)
                    .addOnSuccessListener(unused ->
                            Log.d(TAG, "Geofences registered"))
                    .addOnFailureListener(error ->
                            Log.e(TAG, "Registration failed", error));
        } catch (SecurityException error) {
            Log.e(TAG, "Location permission error", error);
        }
    }

    private static Geofence createFence(
            String id, double latitude, double longitude) {

        return new Geofence.Builder()
                .setRequestId(id)
                .setCircularRegion(latitude, longitude, 100)
                .setExpirationDuration(Geofence.NEVER_EXPIRE)
                .setTransitionTypes(
                        Geofence.GEOFENCE_TRANSITION_ENTER)
                .build();
    }

    public static void markVisited(
            Activity activity, String siteId) {

        SharedPreferences prefs =
                activity.getSharedPreferences(
                        PREFS, Activity.MODE_PRIVATE);

        prefs.edit()
                .putBoolean("visited_" + siteId, true)
                .apply();

        Log.d(TAG, "Visited site saved: " + siteId);
    }
}

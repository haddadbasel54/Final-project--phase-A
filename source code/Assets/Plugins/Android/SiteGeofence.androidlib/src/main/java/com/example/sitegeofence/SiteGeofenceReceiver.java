
package com.example.sitegeofence;

import android.Manifest;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.os.Build;
import android.util.Log;

import com.google.android.gms.location.Geofence;
import com.google.android.gms.location.GeofencingEvent;

public class SiteGeofenceReceiver extends BroadcastReceiver {

    private static final String TAG = "SiteGeofence";
    private static final String PREFS = "site_geofence_prefs";
    private static final String CHANNEL = "nearby_sites";
    private static final long COOLDOWN = 24 * 60 * 60 * 1000L;

    @Override
    public void onReceive(Context context, Intent intent) {

        GeofencingEvent event = GeofencingEvent.fromIntent(intent);

        if (event == null || event.hasError()) {
            Log.e(TAG, "Geofence event error");
            return;
        }

        if (event.getGeofenceTransition()
                != Geofence.GEOFENCE_TRANSITION_ENTER) {
            return;
        }

        SharedPreferences prefs =
                context.getSharedPreferences(
                        PREFS, Context.MODE_PRIVATE);

        for (Geofence fence : event.getTriggeringGeofences()) {

            String id = fence.getRequestId();

            if ("tower_of_flies".equals(id)) {
                notifyIfAllowed(context, prefs, "tower_of_flies");
            }

            if ("khan_wall_group".equals(id)) {

                boolean khanVisited = prefs.getBoolean(
                        "visited_khan_el_hamir", false);

                boolean wallVisited = prefs.getBoolean(
                        "visited_wall_cannons", false);

                if (!khanVisited || !wallVisited) {
                    notifyIfAllowed(context, prefs, "khan_wall_group");
                }
            }
        }
    }

    private void notifyIfAllowed(
            Context context,
            SharedPreferences prefs,
            String group) {

        long lastTime = prefs.getLong(
                "last_notification_" + group, 0);

        long now = System.currentTimeMillis();

        if (now - lastTime < COOLDOWN) {
            return;
        }

        if (Build.VERSION.SDK_INT >= 33 &&
                context.checkSelfPermission(
                        Manifest.permission.POST_NOTIFICATIONS)
                        != PackageManager.PERMISSION_GRANTED) {
            Log.e(TAG, "Notification permission missing");
            return;
        }

        NotificationManager manager =
                (NotificationManager) context.getSystemService(
                        Context.NOTIFICATION_SERVICE);

        if (Build.VERSION.SDK_INT >= 26) {
            NotificationChannel channel =
                    new NotificationChannel(
                            CHANNEL,
                            "Nearby historical sites",
                            NotificationManager.IMPORTANCE_DEFAULT);

            manager.createNotificationChannel(channel);
        }

        Intent launchIntent =
                context.getPackageManager()
                        .getLaunchIntentForPackage(
                                context.getPackageName());

        PendingIntent contentIntent = null;

        if (launchIntent != null) {
            contentIntent = PendingIntent.getActivity(
                    context,
                    2001,
                    launchIntent,
                    PendingIntent.FLAG_UPDATE_CURRENT |
                            PendingIntent.FLAG_IMMUTABLE);
        }

        Notification.Builder builder;

        if (Build.VERSION.SDK_INT >= 26) {
            builder = new Notification.Builder(context, CHANNEL);
        } else {
            builder = new Notification.Builder(context);
        }

        builder.setSmallIcon(context.getApplicationInfo().icon)
                .setContentTitle("Nearby historical site")
                .setContentText(
                        "You're near a site! Check it out on the map.")
                .setAutoCancel(true);

        if (contentIntent != null) {
            builder.setContentIntent(contentIntent);
        }

        try {
            manager.notify(group.hashCode(), builder.build());

            prefs.edit()
                    .putLong("last_notification_" + group, now)
                    .apply();

            Log.d(TAG, "Nearby-site notification sent.");

        } catch (SecurityException error) {
            Log.e(TAG, "Notification failed", error);
        }
    }
}

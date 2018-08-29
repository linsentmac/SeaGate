package com.lenovo.aarforseagate;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import com.unity3d.player.UnityPlayerActivity;

public class MainActivity extends UnityPlayerActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    public int screenOff() {
        Log.e("chao", "aarforseagate send broadcast to screenOff.........................................");
        Intent screenOff = new Intent();
        screenOff.setAction("android.intent.action.ARLAUNCHER_SCREEN_OFF");
        sendBroadcast(screenOff);
        return 1;
    }
}

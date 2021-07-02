package com.lab_am.lab_06;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity {

    private String ipAddress = Settings.CONFIG_IP_ADDRESS;
    private int sampleTime = Settings.DEFAULT_SAMPLE_TIME;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);


    }


    public void btns_onClick(View v) {
        switch (v.getId()) {
            case R.id.config: {
                openSettings();
                break;
            }
            case R.id.ld: {
                openLed();
                break;
            }
            case R.id.lv: {
                openListview();
                break;
            }
            case R.id.rpyc: {
                openGraphchart();
                break;
            }
            case R.id.phtc: {
                openGraphPHTchart();
                break;
            }
            case R.id.jc: {
                openJoystick();
                break;
            }
            default: {
                // do nothing
            }
        }
    }

    private void openSettings() {
        Intent openConfigIntent = new Intent(this, Settings.class);
        Bundle configBundle = new Bundle();
        configBundle.putString(Settings.CONFIG_IP_ADDRESS, ipAddress);
        configBundle.putInt(Settings.CONFIG_SAMPLE_TIME, sampleTime);
        openConfigIntent.putExtras(configBundle);
        startActivityForResult(openConfigIntent, Settings.REQUEST_CODE_CONFIG);
    }

    private void openGraphchart() {
        Intent openGraphIntent = new Intent(this, Graph.class);
        Bundle configBundle = new Bundle();
        configBundle.putString(Settings.CONFIG_IP_ADDRESS, ipAddress);
        configBundle.putInt(Settings.CONFIG_SAMPLE_TIME, sampleTime);
        openGraphIntent.putExtras(configBundle);
        startActivity(openGraphIntent);
    }

    private void openGraphPHTchart() {
        Intent openGraphIntent = new Intent(this, Graphpht.class);
        Bundle configBundle = new Bundle();
        configBundle.putString(Settings.CONFIG_IP_ADDRESS, ipAddress);
        configBundle.putInt(Settings.CONFIG_SAMPLE_TIME, sampleTime);
        openGraphIntent.putExtras(configBundle);
        startActivity(openGraphIntent);
    }

    private void openLed() {
        Intent openLedIntent = new Intent(this, Led.class);
        Bundle configBundle = new Bundle();
        configBundle.putString(Settings.CONFIG_IP_ADDRESS, ipAddress);
        configBundle.putInt(Settings.CONFIG_SAMPLE_TIME, sampleTime);
        openLedIntent.putExtras(configBundle);
        startActivity(openLedIntent);
    }

    private void openListview() {
        Intent openListIntent = new Intent(this, MeasurementList.class);
        Bundle configBundle = new Bundle();
        configBundle.putString(Settings.CONFIG_IP_ADDRESS, ipAddress);
        configBundle.putInt(Settings.CONFIG_SAMPLE_TIME, sampleTime);
        openListIntent.putExtras(configBundle);
        startActivityForResult(openListIntent, Settings.REQUEST_CODE_CONFIG);
    }

    private void openJoystick() {
        Intent openJoystickIntent = new Intent(this, Joystick.class);
        Bundle configBundle = new Bundle();
        configBundle.putString(Settings.CONFIG_IP_ADDRESS, ipAddress);
        configBundle.putInt(Settings.CONFIG_SAMPLE_TIME, sampleTime);
        openJoystickIntent.putExtras(configBundle);
        startActivityForResult(openJoystickIntent, Settings.REQUEST_CODE_CONFIG);
    }
}



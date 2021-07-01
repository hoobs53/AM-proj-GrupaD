package com.lab_am.lab_06;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;

import java.util.HashMap;
import java.util.Map;

public class Settings extends AppCompatActivity {
    //Server's Variables
    // activities request codes
    public final static int REQUEST_CODE_CONFIG = 1;

    // configuration info: names and default values
    public static String CONFIG_IP_ADDRESS = "192.168.1.24";
    public final static String DEFAULT_IP_ADDRESS = "192.168.1.24";

    public  static String CONFIG_SAMPLE_TIME = "500";
    public final static int DEFAULT_SAMPLE_TIME = 500;
    public static String CONFIG_SAMPLE_LIMIT = "1000";
    public final static int DEFAULT_SAMPLE_LIMIT = 1000;
    public String url;

    // IoT server data
    public final static String FILE_NAME = "/resource.php";
    public final static String FILE_NAME_RPY = "/cgi-bin/get_rpy.py";
    public final static String LED_BACKEND = "/cgi-bin/led_display.py";
    public final static String LED_BACKEND2 = "/cgi-bin/get_pixels.py";
    public final static String MEASURMENTS = "/get_measurements.php";

    /* BEGIN config TextViews */
    EditText ipEditText;
    EditText sampleTimeEditText;
    /* END config TextViews */

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);

        // get the Intent that started this Activity
        Intent intent = getIntent();

        // get the Bundle that stores the data of this Activity
        Bundle configBundle = intent.getExtras();

         configureSaveBtn();

        if(configBundle != null) {
            ipEditText = (EditText)findViewById(R.id.ipEditTextConfig);
            String ip = configBundle.getString(CONFIG_IP_ADDRESS,DEFAULT_IP_ADDRESS);
            ipEditText.setText(ip);

            sampleTimeEditText = (EditText)findViewById(R.id.sampleTimeEditTextConfig);
            int st = configBundle.getInt(CONFIG_SAMPLE_TIME,DEFAULT_SAMPLE_TIME);
            sampleTimeEditText.setText(Integer.toString(st));
        }
    }

    public final static String geturlscript(String ip){
        return ("http://" + ip + "/" + Settings.FILE_NAME);
    }

    public final static String geturlrpy(String ip) {
        return ("http://" + ip + "/" + Settings.FILE_NAME_RPY);
    }

    public final static String geturlled(String ip) {
        return ("http://" + ip + "/" + Settings.LED_BACKEND);
    }

    public final static String geturlled2(String ip) {
        return ("http://" + ip + "/" + Settings.LED_BACKEND2);
    }

    public final static String geturlmeasurment(String ip) {
        return ("http://" + ip + "/" + Settings.MEASURMENTS);
    }


     private void configureSaveBtn(){
        Button savebtn = (Button) findViewById(R.id.save_btn );
        savebtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                CONFIG_IP_ADDRESS = ipEditText.getText().toString();
                CONFIG_SAMPLE_TIME = sampleTimeEditText.getText().toString();

                Intent intent = new Intent();
                intent.putExtra(CONFIG_IP_ADDRESS, ipEditText.getText().toString());
                intent.putExtra(CONFIG_SAMPLE_TIME, sampleTimeEditText.getText().toString());
                setResult(RESULT_OK, intent);
                finish();
            }
        });
     }
}
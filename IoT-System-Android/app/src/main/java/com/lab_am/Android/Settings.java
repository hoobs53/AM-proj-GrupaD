package com.lab_am.Android;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;
import IoS.System.Android.R;

import org.json.JSONObject;

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
    public static String CONFIG_SAMPLE_AMOUNT = "100";
    public final static int DEFAULT_SAMPLE_AMOUNT = 100;
    public String url;
    private RequestQueue queue;

    // IoT server data
    public final static String FILE_CONFIG_LOAD = "/get_config.php";
    public final static String FILE_CONFIG_SAVE = "/save_params.php";
    public final static String FILE_NAME = "/resource.php";
    public final static String FILE_NAME_RPY = "/cgi-bin/get_rpy.py";
    public final static String LED_BACKEND = "/cgi-bin/led_display.py";
    public final static String LED_BACKEND2 = "/cgi-bin/get_pixels.py";
    public final static String MEASUREMENTS = "/get_measurements.php";

    /* BEGIN config TextViews */
    EditText ipEditText;
    EditText sampleTimeEditText;
    EditText sampleAmountEditText;
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
         configureDefaultBtn();

        if(configBundle != null) {
            ipEditText = (EditText)findViewById(R.id.ipEditTextConfig);
            String ip = configBundle.getString(CONFIG_IP_ADDRESS,DEFAULT_IP_ADDRESS);

            sampleTimeEditText = (EditText)findViewById(R.id.sampleTimeEditTextConfig);
            int st = configBundle.getInt(CONFIG_SAMPLE_TIME,DEFAULT_SAMPLE_TIME);

            sampleAmountEditText = (EditText)findViewById(R.id.sampleAmountEditTextConfig);
            int sa = configBundle.getInt(CONFIG_SAMPLE_AMOUNT,DEFAULT_SAMPLE_AMOUNT);

            updateTextViews();
        }

        queue = Volley.newRequestQueue(Settings.this);
        loadConfig(queue);
    }

    public final static String geturlscript(String ip,String file){
        return ("http://" + ip + "/" + file);
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
        return ("http://" + ip + "/" + Settings.MEASUREMENTS);
    }

    private void updateTextViews(){
        ipEditText.setText(CONFIG_IP_ADDRESS);
        sampleTimeEditText.setText(CONFIG_SAMPLE_TIME);
        sampleAmountEditText.setText(CONFIG_SAMPLE_AMOUNT);
    }

    private void saveConfig(){
            url = Settings.geturlscript(CONFIG_IP_ADDRESS,FILE_CONFIG_SAVE);
            StringRequest postRequest = new StringRequest(Request.Method.POST, url,
                    new Response.Listener<String>() {
                        @Override
                        public void onResponse(String response) {
                            // response
                            // Log.d("Response", response);

                        }
                    },
                    new Response.ErrorListener() {
                        @Override
                        public void onErrorResponse(VolleyError error) {
                            // handle error
                            //Log.d("Error.Response", response);
                        }
                    }
            ) {
                @Override
                protected Map<String, String> getParams() {
                    Map<String, String> params = new HashMap<String, String>();
                    params.put("ip",CONFIG_IP_ADDRESS );
                    params.put("sample_time",CONFIG_SAMPLE_TIME );
                    params.put("sample_amount",CONFIG_SAMPLE_AMOUNT );

                    return params;
                }
            };
            queue.add(postRequest);
    }

    public final static void loadConfig(RequestQueue queue){
           // url = Settings.geturlscript(CONFIG_IP_ADDRESS,FILE_CONFIG_LOAD);

            String url2 = "http://" + CONFIG_IP_ADDRESS + "/" + FILE_CONFIG_LOAD;
            StringRequest postRequest = new StringRequest(Request.Method.POST, url2,
                    new Response.Listener<String>() {
                        @Override
                        public void onResponse(String response) {
                            JSONObject object;
                            try{
                                object = new JSONObject(response);
                                CONFIG_IP_ADDRESS = object.getString("ip");
                                CONFIG_SAMPLE_TIME = object.getString("sample_time");
                                CONFIG_SAMPLE_AMOUNT = object.getString("sample_amount");
                            }catch(Exception e){
                                Log.d("Error.Response", response);
                                CONFIG_IP_ADDRESS = DEFAULT_IP_ADDRESS;
                                CONFIG_SAMPLE_TIME = Integer.toString(DEFAULT_SAMPLE_TIME);
                                CONFIG_SAMPLE_AMOUNT = Integer.toString(DEFAULT_SAMPLE_AMOUNT);
                            }
                        }
                    },
                    new Response.ErrorListener() {
                        @Override
                        public void onErrorResponse(VolleyError error) {
                            // handle error
                            //Log.d("Error.Response", response);
                        }
                    }
            );
            queue.add(postRequest);
    }

     private void configureSaveBtn(){
        Button savebtn = (Button) findViewById(R.id.save_btn );
        savebtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                CONFIG_IP_ADDRESS = ipEditText.getText().toString();
                CONFIG_SAMPLE_TIME = sampleTimeEditText.getText().toString();
                CONFIG_SAMPLE_AMOUNT = sampleAmountEditText.getText().toString();

                Intent intent = new Intent();
                intent.putExtra(CONFIG_IP_ADDRESS, ipEditText.getText().toString());
                intent.putExtra(CONFIG_SAMPLE_TIME, sampleTimeEditText.getText().toString());
                intent.putExtra(CONFIG_SAMPLE_AMOUNT, sampleAmountEditText.getText().toString());
                updateTextViews();
                saveConfig();

                setResult(RESULT_OK, intent);
                Toast.makeText(getApplicationContext(),"Settings saved",Toast.LENGTH_SHORT).show();
                // finish();
            }
        });
     }

    private void configureDefaultBtn(){
        Button default_btn = (Button) findViewById(R.id.default_btn);
        default_btn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                CONFIG_IP_ADDRESS = DEFAULT_IP_ADDRESS;
                CONFIG_SAMPLE_TIME = Integer.toString(DEFAULT_SAMPLE_TIME);
                CONFIG_SAMPLE_AMOUNT = Integer.toString(DEFAULT_SAMPLE_AMOUNT);


                Intent intent = new Intent();
                intent.putExtra(CONFIG_IP_ADDRESS, ipEditText.getText().toString());
                intent.putExtra(CONFIG_SAMPLE_TIME, sampleTimeEditText.getText().toString());
                intent.putExtra(CONFIG_SAMPLE_AMOUNT, sampleAmountEditText.getText().toString());
                updateTextViews();
                saveConfig();

                setResult(RESULT_OK, intent);
                Toast.makeText(getApplicationContext(),"Default settings set",Toast.LENGTH_SHORT).show();
                //  finish();
            }
        });
    }
}
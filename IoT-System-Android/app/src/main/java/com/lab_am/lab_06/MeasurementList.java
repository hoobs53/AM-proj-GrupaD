package com.lab_am.lab_06;
import android.os.Bundle;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.Volley;

import androidx.recyclerview.widget.DefaultItemAnimator;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.appcompat.app.AppCompatActivity;

import android.os.Handler;
import android.view.View;

import java.util.Objects;
import java.util.Timer;
import java.util.TimerTask;

import org.json.JSONArray;
import org.json.JSONObject;

import java.util.ArrayList;


public class MeasurementList extends AppCompatActivity {
    private ArrayList<Measurement> measurementList;
    private ArrayList<Measurement> measurementList2;
    private ArrayList<Measurement> measurementList3;
    private RecyclerView recyclerView;
    private RecyclerView recyclerView2;
    private RecyclerView recyclerView3;


    private String ipAddress;
    private int sampleTime;


    private RequestQueue queue;
    private Timer requestTimer;
    private TimerTask requestTimerTask;
    private final Handler handler = new Handler();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_measure);
        recyclerView = findViewById(R.id.recyclerView);
        recyclerView2 = findViewById(R.id.recyclerView2);
        recyclerView3 = findViewById(R.id.recyclerView3);
        measurementList = new ArrayList<>();
        measurementList2 = new ArrayList<>();
        measurementList3 = new ArrayList<>();

        ipAddress = Settings.CONFIG_IP_ADDRESS;

    }

    public void start(View v) {
        sampleTime =  Integer.parseInt(Settings.CONFIG_SAMPLE_TIME);

        if(requestTimer == null) {
            // set a new Timer
            requestTimer = new Timer();

            // initialize the TimerTask's job
            requestTimerTask = new TimerTask() {
                public void run() {
                    handler.post(new Runnable() {
                        public void run() { getData(); }
                    });
                }
            };
            requestTimer.schedule(requestTimerTask, 0, sampleTime);

        }

    }

    public void stop(View v){
        if (requestTimer != null) {
            requestTimer.cancel();
            requestTimer = null;
        }
    }
    private void setAdapter(RecyclerView rec_view, ArrayList<Measurement> meas_list){
        recyclerAdapter adapter = new recyclerAdapter(meas_list);
        RecyclerView.LayoutManager layoutManager = new LinearLayoutManager(getApplicationContext());
        rec_view.setLayoutManager(layoutManager);
        rec_view.setItemAnimator(new DefaultItemAnimator());
        rec_view.setAdapter(adapter);
    }
    public void setMeasurementList(JSONArray array_, ArrayList<Measurement> meas_list, RecyclerView rec_view){
        try {
            int len = array_.length();
            for (int i = 0; i < len; i++) {
                JSONObject object2 = array_.getJSONObject(i);
                String name = object2.getString("name");
                String value = object2.getString("value");
                String unit = object2.getString("unit");
                meas_list.add(new Measurement(name, value, unit));
            }
            setAdapter(rec_view, meas_list);
        } catch (Exception e) {
            System.out.println(e);
        }
    }


    public void getData() {
        try {
            measurementList.clear();
            measurementList2.clear();
            measurementList3.clear();
            String url = Settings.geturlmeasurment(ipAddress);
            RequestQueue queue = Volley.newRequestQueue(this);
            JsonObjectRequest jsonArrayRequest = new JsonObjectRequest(Request.Method.GET, url, null, new Response.Listener<JSONObject>() {
                @Override
                public void onResponse(JSONObject response) {
                    try {
                        JSONArray array;
                        array = response.getJSONArray("measurements");
                        setMeasurementList(array, measurementList, recyclerView);
                        array = response.getJSONArray("orientation");
                        setMeasurementList(array, measurementList2, recyclerView2);
                        array = response.getJSONArray("joystick");
                        setMeasurementList(array, measurementList3, recyclerView3);
                    } catch (Exception e) {
                        System.out.println(e);
                        // TODO: handle
                    }

                }
            }, new Response.ErrorListener() {
                @Override
                public void onErrorResponse(VolleyError error) {
                    System.out.println("error");
                    // TODO: handle error
                }
            });
            queue.add(jsonArrayRequest);

        } catch (Exception e){
            // TODO: handle
        }
    }
}
package com.lab_am.Android;

import androidx.appcompat.app.AppCompatActivity;

import android.graphics.Color;
import android.os.Bundle;
import android.os.Handler;
import android.os.SystemClock;
import android.view.View;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;
import com.jjoe64.graphview.GraphView;
import com.jjoe64.graphview.GridLabelRenderer;
import com.jjoe64.graphview.series.DataPoint;
import com.jjoe64.graphview.series.LineGraphSeries;
import IoS.System.Android.R;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;
import java.util.Timer;
import java.util.TimerTask;

public class Graph extends AppCompatActivity {

    private String ipAddress;
    private String url;
    private  int dataGraphMaxDataPointsNumber;
    private int sampleTime;

    private GraphView dataGraph;
    private LineGraphSeries<DataPoint> dataSeries;
    private LineGraphSeries<DataPoint> dataSeries2;
    private LineGraphSeries<DataPoint> dataSeries3;
    private final double dataGraphMaxX = 10.0d;
    private final double dataGraphMinX =  0.0d;
    private final double dataGraphMaxY =  360.0d;
    private final double dataGraphMinY =  0.0d;

    private RequestQueue queue;
    private Timer requestTimer;
    private long requestTimerTimeStamp = 0;
    private long requestTimerPreviousTime = -1;
    private boolean requestTimerFirstRequest = true;
    private boolean requestTimerFirstRequestAfterStop;
    private TimerTask requestTimerTask;
    private final Handler handler = new Handler();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_graph);

        ipAddress = Settings.CONFIG_IP_ADDRESS;


        dataGraph = (GraphView)findViewById(R.id.dataGraph);
        dataSeries = new LineGraphSeries<>(new DataPoint[]{});
        dataSeries2 = new LineGraphSeries<>(new DataPoint[]{});
        dataSeries3 = new LineGraphSeries<>(new DataPoint[]{});
        dataGraph.addSeries(dataSeries);
        dataGraph.addSeries(dataSeries2);
        dataGraph.addSeries(dataSeries3);
        dataGraph.getViewport().setXAxisBoundsManual(true);
        dataGraph.getViewport().setMinX(dataGraphMinX);
        dataGraph.getViewport().setMaxX(dataGraphMaxX);
        dataGraph.getViewport().setYAxisBoundsManual(true);
        dataGraph.getViewport().setMinY(dataGraphMinY);
        dataGraph.getViewport().setMaxY(dataGraphMaxY);
        dataGraph.getViewport().setDrawBorder(true);



        dataGraph.setTitle("Orientation data");
        GridLabelRenderer gridLabel = dataGraph.getGridLabelRenderer();
        gridLabel.setHorizontalAxisTitle("Time [sec]");
        gridLabel.setVerticalAxisTitle("Orientation [deg]");
        gridLabel.setPadding(48);
        gridLabel.setHumanRounding(false);
        gridLabel.setNumHorizontalLabels(6);
        gridLabel.setNumVerticalLabels(10);

        dataSeries.setTitle("Roll");
        dataSeries.setColor(Color.RED);
        dataSeries2.setColor(Color.BLUE);
        dataSeries3.setColor(Color.GREEN);
        dataSeries2.setTitle("Pitch");
        dataSeries3.setTitle("Yaw");
        dataGraph.getLegendRenderer().setVisible(true);

        queue = Volley.newRequestQueue(Graph.this);
    }

    public void loadconfig(){
        sampleTime =  Integer.parseInt(Settings.CONFIG_SAMPLE_TIME);
        dataGraphMaxDataPointsNumber = Integer.parseInt(Settings.CONFIG_SAMPLE_AMOUNT);
    }

    public void startBtn(View view) {
       // EditText sampleTimeText = findViewById(R.id.sampleTimeText)
        loadconfig();
        if (requestTimer == null) {
            requestTimer = new Timer();
            requestTimerTask = new TimerTask() {
                public void run() {
                    handler.post(new Runnable() {
                        public void run() {
                            sendRequestv2("rpy");
                        }
                    });
                }
            };
            requestTimer.schedule(requestTimerTask, 0, sampleTime);

        }
    }
    public void stopBtn(View view){
        if (requestTimer != null) {
            requestTimer.cancel();
            requestTimer = null;
            requestTimerFirstRequestAfterStop = true;
        }
    }
    private long getValidTimeStampIncrease(long currentTime)
    {
        // Right after start remember current time and return 0
        if(requestTimerFirstRequest)
        {
            requestTimerPreviousTime = currentTime;
            requestTimerFirstRequest = false;
            return 0;
        }

        // After each stop return value not greater than sample time
        // to avoid "holes" in the plot
        if(requestTimerFirstRequestAfterStop)
        {
            if((currentTime - requestTimerPreviousTime) > sampleTime)
                requestTimerPreviousTime = currentTime - sampleTime;

            requestTimerFirstRequestAfterStop = false;
        }

        // If time difference is equal zero after start
        // return sample time
        if((currentTime - requestTimerPreviousTime) == 0)
            return sampleTime;

        // Return time difference between current and previous request
        return (currentTime - requestTimerPreviousTime);
    }


    private void sendRequestv2(String file) {
        url = Settings.geturlscript(ipAddress,Settings.FILE_NAME);
        StringRequest postRequest = new StringRequest(Request.Method.POST, url,
                new Response.Listener<String>() {
                    @Override
                    public void onResponse(String response) {
                        // response
                        // Log.d("Response", response);
                        handleResponse(response);
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
                params.put("filename", file);

                return params;
            }
        };
        queue.add(postRequest);
    }

    private void handleResponse(String response){
        if(requestTimer != null) {
            // get time stamp with SystemClock
            long requestTimerCurrentTime = SystemClock.uptimeMillis(); // current time
            requestTimerTimeStamp += getValidTimeStampIncrease(requestTimerCurrentTime);

            // get data from JSON response
            double [] data = getData(response);


            // update plot series
            double timeStamp = requestTimerTimeStamp / 1000.0; // [sec]
            boolean scrollGraph = (timeStamp > dataGraphMaxX);
            dataSeries.appendData(new DataPoint(timeStamp, data[0]), scrollGraph, dataGraphMaxDataPointsNumber);
            dataSeries2.appendData(new DataPoint(timeStamp, data[1]), scrollGraph, dataGraphMaxDataPointsNumber);
            dataSeries3.appendData(new DataPoint(timeStamp, data[2]), scrollGraph, dataGraphMaxDataPointsNumber);

            // refresh chart
            dataGraph.onDataChanged(true, true);

            // remember previous time stamp
            requestTimerPreviousTime = requestTimerCurrentTime;
        }
    }
    private double [] getData(String response){
        JSONObject object;
        double r = Double.NaN;
        double p = Double.NaN;
        double y = Double.NaN;
        double [] result = new double[3];
        try{
            object = new JSONObject(response);
        }catch(Exception e){
            result[0] = r;
            result[1] = p;
            result[2] = y;
            return result;
        }

        try{
            r = object.getDouble("roll");
            p = object.getDouble("pitch");
            y = object.getDouble("yaw");
            result[0] = r;
            result[1] = p;
            result[2] = y;
        } catch (JSONException e){
            // TODO: handle error
        }
        return result;
    }
}
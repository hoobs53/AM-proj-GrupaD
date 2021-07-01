package com.lab_am.lab_06;

import androidx.appcompat.app.AppCompatActivity;

import android.app.AlertDialog;
import android.graphics.Color;
import android.os.Bundle;
import android.os.Handler;
import android.os.SystemClock;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;

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

import org.json.JSONException;
import org.json.JSONObject;

import java.util.Timer;
import java.util.TimerTask;

public class Graphpht extends AppCompatActivity {

    private String ipAddress;
    private String url;

    private GraphView dataGraph;
    private LineGraphSeries<DataPoint> dataSeries;
    private LineGraphSeries<DataPoint> dataSeries2;
    private LineGraphSeries<DataPoint> dataSeries3;
    private final int dataGraphMaxDataPointsNumber = 1000;
    private final double dataGraphMaxX = 10.0d;
    private final double dataGraphMinX =  0.0d;
    private final double dataGraphMaxY =  130.0d;
    private final double dataGraphMinY =  -35.0d;

    private final double dataGraphMaxhY =  100.0d;
    private final double dataGraphMinhY =  0.0d;

    private RequestQueue queue;
    private Timer requestTimer;
    private long requestTimerTimeStamp = 0;
    private long requestTimerPreviousTime = -1;
    private boolean requestTimerFirstRequest = true;
    private boolean requestTimerFirstRequestAfterStop;
    private TimerTask requestTimerTask;
    private final Handler handler = new Handler();
    private int sampleTime;

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
        dataGraph.getViewport().setXAxisBoundsManual(true);
        dataGraph.getViewport().setMaxX(dataGraphMaxX);
        dataGraph.getViewport().setMinX(dataGraphMinX);
        dataGraph.getViewport().setYAxisBoundsManual(true);
        dataGraph.getViewport().setMaxY(dataGraphMaxY);
        dataGraph.getViewport().setMinY(dataGraphMinY);
        dataSeries.setTitle("Pressure");
        dataSeries.setColor(Color.RED);

        dataGraph.addSeries(dataSeries2);
        dataSeries2.setColor(Color.GREEN);
        dataSeries2.setTitle("Temperature");

        dataGraph.setTitle("Orientation data");
        GridLabelRenderer gridLabel = dataGraph.getGridLabelRenderer();
        gridLabel.setHorizontalAxisTitle("Time [sec]");
        gridLabel.setVerticalAxisTitle("Orientation [deg]");


        dataGraph.getSecondScale().addSeries(dataSeries3);
        dataGraph.getSecondScale().setMaxY(dataGraphMaxhY);
        dataGraph.getSecondScale().setMinY(dataGraphMinhY);
        dataGraph.getSecondScale().setVerticalAxisTitle("%");
        dataSeries3.setColor(Color.BLUE);
        dataSeries3.setTitle("Humidity");

        dataGraph.getLegendRenderer().setVisible(true);

        queue = Volley.newRequestQueue(Graphpht.this);
    }

    public void startBtn(View view) {
        // EditText sampleTimeText = findViewById(R.id.sampleTimeText)
        sampleTime =  Integer.parseInt(Settings.CONFIG_SAMPLE_TIME);
        if (requestTimer == null) {
            requestTimer = new Timer();
            requestTimerTask = new TimerTask() {
                public void run() {
                    handler.post(new Runnable() {
                        public void run() {
                            sendRequest();
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
    private void sendRequest(){
        url = Settings.geturlrpy(ipAddress);
        StringRequest stringRequest = new StringRequest(Request.Method.GET, url,
                new Response.Listener<String>() {
                    @Override
                    public void onResponse(String response) { handleResponse(response); }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        // TODO: handle error
                    }
                });
        // Add the request to the RequestQueue.
        queue.add(stringRequest);
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
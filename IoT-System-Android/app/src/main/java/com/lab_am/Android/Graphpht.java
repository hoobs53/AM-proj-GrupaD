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

public class Graphpht extends AppCompatActivity {

    //IoT server data
    private String ipAddress;
    private String url;
    private  int dataGraphMaxDataPointsNumber;
    private int sampleTime;

    // Graph's variables settings
    private GraphView dataGraph;
    private LineGraphSeries<DataPoint> dataSeries;
    private LineGraphSeries<DataPoint> dataSeries2;
    private LineGraphSeries<DataPoint> dataSeries3;

    private final double dataGraphMaxX = 10.0d;
    private final double dataGraphMinX =  0.0d;
    private final double dataGraphMaxY =  2000.0d;
    private final double dataGraphMinY =  0.0d;

    private final double dataGraphMaxhY =  105.0d;
    private final double dataGraphMinhY =  -30.1d;


    // Timer's variables
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
        setContentView(R.layout.activity_graphpht);

        // get server's ip addres from settings
        ipAddress = Settings.CONFIG_IP_ADDRESS;

        // initialize graph, set title, add data series
        dataGraph = (GraphView)findViewById(R.id.dataGraphpht);
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
        dataGraph.getViewport().setDrawBorder(true);

        dataGraph.setTitle("Measurements data");
        GridLabelRenderer gridLabel = dataGraph.getGridLabelRenderer();
        gridLabel.setHorizontalAxisTitle("Time [s]");
        gridLabel.setVerticalAxisTitle("Pressure [hPa]");


        // Set second scale
        dataGraph.getSecondScale().setMaxY(dataGraphMaxhY);
        dataGraph.getSecondScale().setMinY(dataGraphMinhY);
        dataGraph.getSecondScale().setVerticalAxisTitle("Humidity, Temp [%, C]");
        dataGraph.getSecondScale().setVerticalAxisTitleTextSize(gridLabel.getVerticalAxisTitleTextSize());
        dataGraph.getSecondScale().setVerticalAxisTitleColor(gridLabel.getVerticalAxisTitleColor());

        dataGraph.getSecondScale().addSeries(dataSeries2);
        dataGraph.getSecondScale().addSeries(dataSeries3);

        gridLabel.setNumHorizontalLabels(6);
        gridLabel.setNumVerticalLabels(6);

        dataSeries2.setColor(Color.BLUE);
        dataSeries2.setTitle("Temperature");
        dataSeries3.setColor(Color.GREEN);
        dataSeries3.setTitle("Humidity");
        gridLabel.setPadding(56);

        dataGraph.getLegendRenderer().setVisible(true);
        queue = Volley.newRequestQueue(Graphpht.this);
    }


    // load config from server
    public void loadconfig(){
        sampleTime =  Integer.parseInt(Settings.CONFIG_SAMPLE_TIME);
        dataGraphMaxDataPointsNumber = Integer.parseInt(Settings.CONFIG_SAMPLE_AMOUNT);
    }

    // Start downloading data from server
    public void startTimer(View view) {
        loadconfig();
        if (requestTimer == null) {
            requestTimer = new Timer();
            requestTimerTask = new TimerTask() {
                public void run() {
                    handler.post(new Runnable() {
                        public void run() {
                            sendRequest("pht");
                        }
                    });
                }
            };
            requestTimer.schedule(requestTimerTask, 0, sampleTime);

        }
    }

    // Stop downloading data from server
    public void stopTimer(View view){
        if (requestTimer != null) {
            requestTimer.cancel();
            requestTimer = null;
            requestTimerFirstRequestAfterStop = true;
        }
    }

    // Calculate time stamps
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


    // Send request for server's data
    private void sendRequest(String file) {
        url = Settings.geturlscript(ipAddress,Settings.FILE_NAME);
        StringRequest postRequest = new StringRequest(Request.Method.POST, url,
                new Response.Listener<String>() {
                    @Override
                    public void onResponse(String response) {
                        // response
                        // Log.d("Response", response);
                        updatePlot(response);
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

    // Update plot based on servers response
    private void updatePlot(String response){
        if(requestTimer != null) {
            // get time stamp with SystemClock
            long requestTimerCurrentTime = SystemClock.uptimeMillis(); // current time
            requestTimerTimeStamp += getValidTimeStampIncrease(requestTimerCurrentTime);

            // get data from JSON response
            double [] data = getData(response);


            // update plot series
            double timeStamp = requestTimerTimeStamp / 1000.0; // [sec]
            boolean scrollGraph = (timeStamp > dataGraphMaxX);
            dataSeries.appendData(new DataPoint(timeStamp, data[1]), scrollGraph, dataGraphMaxDataPointsNumber);
            dataSeries2.appendData(new DataPoint(timeStamp, data[2]), scrollGraph, dataGraphMaxDataPointsNumber);
            dataSeries3.appendData(new DataPoint(timeStamp, data[0]), scrollGraph, dataGraphMaxDataPointsNumber);

            // refresh chart
            dataGraph.onDataChanged(true, true);

            // remember previous time stamp
            requestTimerPreviousTime = requestTimerCurrentTime;
        }
    }

    // Handle response from server
    private double [] getData(String response){
        JSONObject object;
        double h = Double.NaN;
        double t = Double.NaN;
        double p = Double.NaN;
        double [] result = new double[3];
        try{
            object = new JSONObject(response);
        }catch(Exception e){
            result[0] = h;
            result[1] = p;
            result[2] = t;
            return result;
        }

        try{
            h = object.getDouble("humidity");
            p = object.getDouble("pressure");
            t = object.getDouble("temperature");
            result[0] = h;
            result[1] = p;
            result[2] = t;
        } catch (JSONException e){
            // TODO: handle error
        }
        return result;
    }
}
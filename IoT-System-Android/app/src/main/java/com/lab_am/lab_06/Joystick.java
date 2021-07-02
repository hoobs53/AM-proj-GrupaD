package com.lab_am.lab_06;

import androidx.appcompat.app.AppCompatActivity;

import android.graphics.Color;
import android.os.Bundle;
import android.os.Handler;
import android.os.SystemClock;
import android.view.View;
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
import com.jjoe64.graphview.series.PointsGraphSeries;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;
import java.util.Timer;
import java.util.TimerTask;

public class Joystick extends AppCompatActivity {

    private String ipAddress;
    private String url;

    private GraphView dataGraph;
    private PointsGraphSeries<DataPoint> dataSeries;

    private final int dataGraphMaxDataPointsNumber = 1000;
    private final double dataGraphMaxX = 25.0d;
    private final double dataGraphMinX =  -25.0d;
    private final double dataGraphMaxY =  25.0d;
    private final double dataGraphMinY =  -25.0d;

    double temp_x = 0;
    double temp_y = 0;

    private RequestQueue queue;
    private Timer requestTimer;
    private long requestTimerTimeStamp = 0;
    private long requestTimerPreviousTime = -1;
    private boolean requestTimerFirstRequest = true;
    private boolean requestTimerFirstRequestAfterStop;
    private TimerTask requestTimerTask;
    private final Handler handler = new Handler();
    private int sampleTime;

    TextView counterView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_joystick);
        counterView = (TextView)findViewById(R.id.counter);
        counterView.setText("Counter: 0");

        ipAddress = Settings.CONFIG_IP_ADDRESS;


        dataGraph = (GraphView)findViewById(R.id.dataGraphjoystick);

        dataGraph.getViewport().setXAxisBoundsManual(true);
        dataGraph.getViewport().setMinX(dataGraphMinX);
        dataGraph.getViewport().setMaxX(dataGraphMaxX);
        dataGraph.getViewport().setYAxisBoundsManual(true);
        dataGraph.getViewport().setMinY(dataGraphMinY);
        dataGraph.getViewport().setMaxY(dataGraphMaxY);
        dataGraph.getViewport().setDrawBorder(true);
        dataSeries = new PointsGraphSeries<>(new DataPoint[]{});
        dataSeries.setColor(Color.RED);
        dataGraph.addSeries(dataSeries);


        dataGraph.setTitle("Joystick position data");
        GridLabelRenderer gridLabel = dataGraph.getGridLabelRenderer();
        gridLabel.setHorizontalAxisTitle("Coordinate x [-]");
        gridLabel.setVerticalAxisTitle("Coordinate [-]");
        gridLabel.setHumanRounding(false);
        gridLabel.setNumHorizontalLabels(11);
        gridLabel.setNumVerticalLabels(11);
        gridLabel.setPadding(48);

        queue = Volley.newRequestQueue(Joystick.this);
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
                            sendRequestv2("joystick");
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

    private void createNewDataseries(){
        dataSeries = new PointsGraphSeries<>(new DataPoint[]{});
        dataSeries.setColor(Color.RED);
        dataSeries.setSize(15);
    }

    private void sendRequestv2(String file) {
        url = Settings.geturlscript(ipAddress);
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

            double timeStamp = requestTimerTimeStamp / 1000.0; // [sec]
            boolean scrollGraph = false;

            if(temp_x != data[0] || temp_y!=data[1]){
                dataGraph.removeAllSeries();
                temp_x = data[0];
                temp_y = data[1];
                createNewDataseries();
                dataGraph.addSeries(dataSeries);
                dataSeries.appendData(new DataPoint(data[0], data[1]),scrollGraph,dataGraphMaxDataPointsNumber);
            }
            else
            {
                dataSeries.appendData(new DataPoint(data[0], data[1]),scrollGraph,dataGraphMaxDataPointsNumber);
            }
            counterView.setText("Counter: " + (int)data[2]);
            // update plot series


            // refresh chart
            dataGraph.onDataChanged(true, true);

            // remember previous time stamp
            requestTimerPreviousTime = requestTimerCurrentTime;
        }
    }
    private double [] getData(String response){
        JSONObject object;
        double x = Double.NaN;
        double y = Double.NaN;
        double mid = Double.NaN;
        double [] result = new double[3];
        try{
            object = new JSONObject(response);
        }catch(Exception e){
            result[0] = x;
            result[1] = y;
            result[2] = mid;
            return result;
        }

        try{
            x = object.getDouble("x");
            y = object.getDouble("y");
            mid = object.getDouble("mid_counter");
            result[0] = x;
            result[1] = y;
            result[2] = mid;
        } catch (JSONException e){
            // TODO: handle error
        }
        return result;
    }
}
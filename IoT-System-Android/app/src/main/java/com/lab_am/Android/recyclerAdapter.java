package com.lab_am.Android;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import IoS.System.Android.R;
import java.util.ArrayList;


public class recyclerAdapter extends RecyclerView.Adapter<recyclerAdapter.MyViewHolder>{
    private ArrayList<Measurement> measurementList;

    public recyclerAdapter(ArrayList<Measurement> measurementList){
        this.measurementList = measurementList;
    }

    public class MyViewHolder extends RecyclerView.ViewHolder{
        private TextView nameTxt;
        private TextView valueTxt;
        private TextView unitTxt;

        public MyViewHolder(final View view){
            super(view);
            nameTxt = view.findViewById(R.id.textView);
            valueTxt = view.findViewById(R.id.textView2);
            unitTxt = view.findViewById(R.id.textView3);
        }
    }
    @NonNull
    @Override
    public recyclerAdapter.MyViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View itemView = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_measurement_data, parent, false);
        return new MyViewHolder(itemView);
    }

    @Override
    public void onBindViewHolder(@NonNull recyclerAdapter.MyViewHolder holder, int position) {
        String name = measurementList.get(position).getName();
        String value = measurementList.get(position).getValue();
        String unit = measurementList.get(position).getUnit();
        holder.nameTxt.setText(name);
        holder.valueTxt.setText(value);
        holder.unitTxt.setText(unit);
    }

    @Override
    public int getItemCount() {
        return measurementList.size();
    }
}


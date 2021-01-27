using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StatRoll))]
public class StatRollEditor : Editor
{
    const int BucketCount = 20;
    const int SampleCount = 10000;

    bool expanded;
    int previewStatValue;
    int[] buckets = new int[BucketCount];

    public override void OnInspectorGUI()
    {
        var t = (target as StatRoll);

        DrawDefaultInspector();

        EditorGUILayout.Space();

        expanded = EditorGUILayout.BeginToggleGroup("Preview", expanded);
        if (expanded)
        {
            previewStatValue = Mathf.RoundToInt(EditorGUILayout.Slider("Stat value", previewStatValue, 0, 10));
            DrawPreviewCurve();
        }
        EditorGUILayout.EndToggleGroup();
    }
    
    private void DrawPreviewCurve()
    {
        RefreshBuckets();

        float maxBucketSamples = 0;
        foreach (var bucket in buckets)
            maxBucketSamples = Mathf.Max(maxBucketSamples, bucket);

        var curve = new AnimationCurve();
        //curve.AddKey(new Keyframe(0, 0, 0, 0, 0, 0));
        float bucketWidth = 1f / BucketCount;
        for (int i = 0; i < BucketCount; ++i)
        {
            float t0 = (i + .001f) * bucketWidth;
            float t1 = (i + .999f) * bucketWidth;
            float value = buckets[i] / maxBucketSamples;
            //key.weightedMode = WeightedMode.None;
            curve.AddKey(new Keyframe(t0, value, 0, 0, 0, 0));
            curve.AddKey(new Keyframe(t1, value, 0, 0, 0, 0));
        }
        //Statcurve.AddKey(new Keyframe(1, 0, 0, 0, 0, 0));

        EditorGUILayout.CurveField("Distribution", curve, Color.white, Rect.MinMaxRect(0,0,1,1));
    }

    private void RefreshBuckets()
    {
        var t = (target as StatRoll);

        for (int i = 0; i < BucketCount; ++i)
            buckets[i] = 0;

        for (int i = 0; i < SampleCount; ++i)
        {
            float sample = t.Roll(previewStatValue);
            int bucket = Mathf.Clamp(Mathf.FloorToInt(sample * BucketCount), 0, BucketCount - 1);
            ++buckets[bucket];
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using Windows.Kinect;
using System;

public class MovingExpAverageFilter : Filter
{
    float k;
    Dictionary<JointType, Avatar.Joint> avgJoints;

    public MovingExpAverageFilter(int maxSamples) : base(maxSamples)
    {
        k = 2.0f / (MaxSamples + 1.0f);
    }

    public override Avatar.Body GetFilteredData(Avatar.Body body)
    {
        AddToSamples(body);
        if (MaxSamples <= 1 || BodySamples.Count != MaxSamples)
        {
            return BodySamples[0];
        }

        return GetMovingExpAverageData(BodySamples[0]);
    }

    private Avatar.Body GetMovingExpAverageData(Avatar.Body lastBody)
    {
        if (avgJoints != null)
        {
            for (JointType jt = 0; jt <= JointType.ThumbRight; jt++)
            {
                avgJoints[jt].Position = ((lastBody.Joints[jt].Position - avgJoints[jt].Position) * k) + avgJoints[jt].Position;
                avgJoints[jt].Rotation = Quaternion.identity;
            }

            for (int i = BodySamples.Count - 1; i >= 0; i--)
            {
                for (JointType jt = 0; jt <= JointType.ThumbRight; jt++)
                {
                    Quaternion avgRot = avgJoints[jt].Rotation;
                    Quaternion sampleRot = BodySamples[i].Joints[jt].Rotation;
                    avgJoints[jt].Rotation = new Quaternion(
                        avgRot.x + sampleRot.x,
                        avgRot.y + sampleRot.y,
                        avgRot.z + sampleRot.z,
                        avgRot.w + sampleRot.w);

                    if (i == 0)
                    {
                        avgJoints[jt].TrackingState = BodySamples[i].Joints[jt].TrackingState;
                    }
                }
            }

            for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
            {
                Quaternion avgRot = avgJoints[jt].Rotation;
                float k = 1.0f / Mathf.Sqrt(avgRot.x * avgRot.x + avgRot.y * avgRot.y + avgRot.z * avgRot.z + avgRot.w * avgRot.w);
                avgJoints[jt].Rotation = new Quaternion(avgRot.x * k, avgRot.y * k, avgRot.z * k, avgRot.w * k);
            }

            return new Avatar.Body(avgJoints);
        }
        else
        {
            avgJoints = new Dictionary<JointType, Avatar.Joint>();
            for (int i = BodySamples.Count - 1; i >= 0; i--)
            {
                for (JointType jt = 0; jt <= JointType.ThumbRight; jt++)
                {
                    if (avgJoints.ContainsKey(jt))
                    {
                        avgJoints[jt].Position += BodySamples[i].Joints[jt].Position;

                        Quaternion avgRot = avgJoints[jt].Rotation;
                        Quaternion sampleRot = BodySamples[i].Joints[jt].Rotation;
                        avgJoints[jt].Rotation = new Quaternion(
                            avgRot.x + sampleRot.x,
                            avgRot.y + sampleRot.y,
                            avgRot.z + sampleRot.z,
                            avgRot.w + sampleRot.w);
                    }
                    else
                    {
                        avgJoints[jt] = new Avatar.Joint();
                        avgJoints[jt].Position = BodySamples[i].Joints[jt].Position;
                        avgJoints[jt].Rotation = BodySamples[i].Joints[jt].Rotation;
                        avgJoints[jt].JointType = BodySamples[i].Joints[jt].JointType;
                    }

                    //tracking state
                    if (i == 0)
                    {
                        avgJoints[jt].TrackingState = BodySamples[i].Joints[jt].TrackingState;
                    }
                }
            }
            for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
            {
                avgJoints[jt].Position = avgJoints[jt].Position / (float)MaxSamples;
                Quaternion avgRot = avgJoints[jt].Rotation;
                float k = 1.0f / Mathf.Sqrt(avgRot.x * avgRot.x + avgRot.y * avgRot.y + avgRot.z * avgRot.z + avgRot.w * avgRot.w);
                avgJoints[jt].Rotation = new Quaternion(avgRot.x * k, avgRot.y * k, avgRot.z * k, avgRot.w * k);
            }
        }
        return new Avatar.Body(avgJoints);
    }
}

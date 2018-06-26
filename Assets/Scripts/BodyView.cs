using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class BodyView : MonoBehaviour {

    public Material BoneMaterial;
    public GameObject BodyManager;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodyManager _BodyManager;

    private Dictionary<JointType, JointType> _BoneMap = new Dictionary<JointType, JointType>()
{
    { JointType.FootLeft, JointType.AnkleLeft },
    { JointType.AnkleLeft, JointType.KneeLeft },
    { JointType.KneeLeft, JointType.HipLeft },
    { JointType.HipLeft, JointType.SpineBase },

    { JointType.FootRight, JointType.AnkleRight },
    { JointType.AnkleRight, JointType.KneeRight },
    { JointType.KneeRight, JointType.HipRight },
    { JointType.HipRight, JointType.SpineBase },

    { JointType.HandTipLeft, JointType.HandLeft }, //Need this for HandSates
    { JointType.ThumbLeft, JointType.HandLeft },
    { JointType.HandLeft, JointType.WristLeft },
    { JointType.WristLeft, JointType.ElbowLeft },
    { JointType.ElbowLeft, JointType.ShoulderLeft },
    { JointType.ShoulderLeft, JointType.SpineShoulder },

    { JointType.HandTipRight, JointType.HandRight }, //Needthis for Hand State
    { JointType.ThumbRight, JointType.HandRight },
    { JointType.HandRight, JointType.WristRight },
    { JointType.WristRight, JointType.ElbowRight },
    { JointType.ElbowRight, JointType.ShoulderRight },
    { JointType.ShoulderRight, JointType.SpineShoulder },

    { JointType.SpineBase, JointType.SpineMid },
    { JointType.SpineMid, JointType.SpineShoulder },
    { JointType.SpineShoulder, JointType.Neck },
    { JointType.Neck, JointType.Head },
};

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (BodyManager == null)
        {
            return;
        }

        _BodyManager = BodyManager.GetComponent<BodyManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }

    }

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }

        return body;
    }

    private void RefreshBodyObject(Body body, GameObject bodyObject)
    {
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            Windows.Kinect.Joint sourceJoint = body.Joints[jt];
            Windows.Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }
            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.startColor = GetColorForState(sourceJoint.TrackingState);
                lr.endColor = GetColorForState(targetJoint.Value.TrackingState);
            }
            else
            {
                lr.enabled = false;
            }
        }
    }

    private static Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint, float scale = 5.0f)
    {
        return new Vector3(joint.Position.X * scale, joint.Position.Y * scale, joint.Position.Z * -scale);
    }

    private static Color GetColorForState(TrackingState state)
    {
        switch (state)
        {
            case TrackingState.Tracked:
                return Color.green;

            case TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }
}

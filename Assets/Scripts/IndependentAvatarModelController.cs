using UnityEngine;
using Windows.Kinect;
using System.Collections.Generic;

public class IndependentAvatarModelController : MonoBehaviour
{
    public GameObject Spine_Base;
    public GameObject Spine_Mid;
    public GameObject Neck;
    public GameObject Head;
    public GameObject Shoulder_Left;
    public GameObject Elbow_Left;
    public GameObject Wrist_Left;
    public GameObject Hand_Left;
    public GameObject Shoulder_Right;
    public GameObject Elbow_Right;
    public GameObject Wrist_Right;
    public GameObject Hand_Right;
    public GameObject Hip_Left;
    public GameObject Knee_Left;
    public GameObject Ankle_Left;
    public GameObject Foot_Left;
    public GameObject Hip_Right;
    public GameObject Knee_Right;
    public GameObject Ankle_Right;
    public GameObject Foot_Right;
    public GameObject Spine_Shoulder;
    public GameObject Hand_Tip_Left;
    public GameObject Thumb_Left;
    public GameObject Hand_Tip_Right;
    public GameObject Thumb_Right;

    public int modelIndex;

    public GameObject BodyManager;
    private BodyManager _BodyManager;

    public float shoulderLeftRotX = 0.0f;
    public float shoulderLeftRotY = 0.0f;
    public float shoulderLeftRotZ = 0.0f;

    public float shoulderRightRotX = 0.0f;
    public float shoulderRightRotY = 0.0f;
    public float shoulderRightRotZ = 0.0f;

    public float elbowLeftRotX = 0.0f;
    public float elbowLeftRotY = 0.0f;
    public float elbowLeftRotZ = 0.0f;

    public float elbowRightRotX = 0.0f;
    public float elbowRightRotY = 0.0f;
    public float elbowRightRotZ = 0.0f;

    public float wristLeftRotX = 0.0f;
    public float wristLeftRotY = 0.0f;
    public float wristLeftRotZ = 0.0f;

    public float wristRightRotX = 0.0f;
    public float wristRightRotY = 0.0f;
    public float wristRightRotZ = 0.0f;

    public float handLeftRotX = 0.0f;
    public float handLeftRotY = 0.0f;
    public float handLeftRotZ = 0.0f;

    public float handRightRotX = 0.0f;
    public float handRightRotY = 0.0f;
    public float handRightRotZ = 0.0f;

    public float hipRightRotX = 0.0f;
    public float hipRightRotY = 0.0f;
    public float hipRightRotZ = 0.0f;

    public float kneeRightRotX = 0.0f;
    public float kneeRightRotY = 0.0f;
    public float kneeRightRotZ = 0.0f;

    public float hipLeftRotX = 0.0f;
    public float hipLeftRotY = 0.0f;
    public float hipLeftRotZ = 0.0f;

    public float kneeLeftRotX = 0.0f;
    public float kneeLeftRotY = 0.0f;
    public float kneeLeftRotZ = 0.0f;

    public float spineBaseRotX = 0.0f;
    public float spineBaseRotY = 0.0f;
    public float spineBaseRotZ = 0.0f;

    public float spineMidRotX = 0.0f;
    public float spineMidRotY = 0.0f;
    public float spineMidRotZ = 0.0f;

    public float spineShoulderRotX = 0.0f;
    public float spineShoulderRotY = 0.0f;
    public float spineShoulderRotZ = 0.0f;

    public float neckRotX = 0.0f;
    public float neckRotY = 0.0f;
    public float neckRotZ = 0.0f;


    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private Dictionary<JointType, Vector3> meshInitialJointLocations;
    private GameObject[] meshActiveJoints;


    private JointType[] jointTypes = new JointType[] {
        JointType.SpineShoulder, JointType.SpineMid, JointType.ShoulderLeft, JointType.ShoulderRight,
        JointType.Neck, JointType.Head, JointType.SpineBase,
        JointType.ElbowLeft, JointType.WristLeft, JointType.HandLeft, JointType.HandTipLeft, JointType.ThumbLeft,
        JointType.ElbowRight, JointType.WristRight, JointType.HandRight, JointType.HandTipRight, JointType.ThumbRight,
        JointType.HipLeft, JointType.KneeLeft, JointType.AnkleLeft, JointType.FootLeft,
        JointType.HipRight, JointType.KneeRight, JointType.AnkleRight, JointType.FootRight
    };

    private Dictionary<JointType, JointType> jointParents = new Dictionary<JointType, JointType>()
    {
        { JointType.FootLeft, JointType.AnkleLeft },
        { JointType.AnkleLeft, JointType.KneeLeft },
        { JointType.KneeLeft, JointType.HipLeft },
        { JointType.HipLeft, JointType.SpineBase },

        { JointType.FootRight, JointType.AnkleRight },
        { JointType.AnkleRight, JointType.KneeRight },
        { JointType.KneeRight, JointType.HipRight },
        { JointType.HipRight, JointType.SpineBase },

        { JointType.HandTipLeft, JointType.HandLeft },
        { JointType.ThumbLeft, JointType.WristLeft },
        { JointType.HandLeft, JointType.WristLeft },
        { JointType.WristLeft, JointType.ElbowLeft },
        { JointType.ElbowLeft, JointType.ShoulderLeft },
        { JointType.ShoulderLeft, JointType.SpineShoulder },

        { JointType.HandTipRight, JointType.HandRight },
        { JointType.ThumbRight, JointType.WristRight },
        { JointType.HandRight, JointType.WristRight },
        { JointType.WristRight, JointType.ElbowRight },
        { JointType.ElbowRight, JointType.ShoulderRight },
        { JointType.ShoulderRight, JointType.SpineShoulder },

        { JointType.SpineBase, JointType.SpineMid },
        { JointType.SpineMid, JointType.SpineShoulder },
        { JointType.Head, JointType.Neck },
        { JointType.Neck, JointType.SpineShoulder },
        { JointType.SpineShoulder, JointType.SpineShoulder },
    };

    void Start()
    {
        meshActiveJoints = new GameObject[] {
            Spine_Base, Spine_Mid, Neck, Head,
            Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,
            Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,
            Hip_Left, Knee_Left, Ankle_Left, Foot_Left,
            Hip_Right, Knee_Right, Ankle_Right, Foot_Right,
            Spine_Shoulder, Hand_Tip_Left, Thumb_Left, Hand_Tip_Right, Thumb_Right
        };

        meshInitialJointLocations = new Dictionary<JointType, Vector3> {
            {JointType.SpineShoulder, Spine_Shoulder.transform.position },
            {JointType.SpineMid, Spine_Mid.transform.position },
            {JointType.ShoulderLeft, Shoulder_Left.transform.position },
            {JointType.ShoulderRight, Shoulder_Right.transform.position },
            {JointType.Neck, Neck.transform.position },
            {JointType.Head, Head.transform.position},
            {JointType.SpineBase, Spine_Base.transform.position },

            {JointType.ElbowLeft, Elbow_Left.transform.position },
            {JointType.WristLeft, Wrist_Left.transform.position },
            {JointType.HandLeft, Hand_Left.transform.position },
            {JointType.HandTipLeft, Hand_Tip_Left.transform.position },
            {JointType.ThumbLeft, Thumb_Left.transform.position },

            {JointType.ElbowRight, Elbow_Right.transform.position },
            {JointType.WristRight, Wrist_Right.transform.position },
            {JointType.HandRight, Hand_Right.transform.position },
            {JointType.HandTipRight, Hand_Tip_Right.transform.position },
            {JointType.ThumbRight, Thumb_Right.transform.position },

            { JointType.HipLeft, Hip_Left.transform.position },
            {JointType.KneeLeft, Knee_Left.transform.position },
            {JointType.AnkleLeft, Ankle_Left.transform.position },
            {JointType.FootLeft, Foot_Left.transform.position },

            { JointType.HipRight, Hip_Right.transform.position },
            {JointType.KneeRight, Knee_Right.transform.position },
            {JointType.AnkleRight, Ankle_Right.transform.position },
            {JointType.FootRight, Foot_Right.transform.position }
        };

        //shoulderRInitDir = meshInitialJointLocations[JointType.ElbowRight] - meshInitialJointLocations[JointType.ShoulderRight];
        //shoulderRInitRot = Shoulder_Right.transform.rotation;
        //shoulderLInitDir = meshInitialJointLocations[JointType.ElbowLeft] - meshInitialJointLocations[JointType.ShoulderLeft];
        //shoulderLInitRot = Shoulder_Left.transform.rotation;

        //elbowRInitDir = meshInitialJointLocations[JointType.WristRight] - meshInitialJointLocations[JointType.ElbowRight];
        //elbowRInitRot = Elbow_Right.transform.rotation;
        //elbowLInitDir = meshInitialJointLocations[JointType.WristLeft] - meshInitialJointLocations[JointType.ElbowLeft];
        //elbowLInitRot = Elbow_Left.transform.rotation;

        //wristRInitDir = meshInitialJointLocations[JointType.HandRight] - meshInitialJointLocations[JointType.WristRight];
        //wristRInitRot = Wrist_Right.transform.rotation;
        //wristLInitDir = meshInitialJointLocations[JointType.HandLeft] - meshInitialJointLocations[JointType.WristLeft];
        //wristLInitRot = Wrist_Left.transform.rotation;

        //handRInitDir = meshInitialJointLocations[JointType.HandTipRight] - meshInitialJointLocations[JointType.HandRight];
        //handRInitRot = Hand_Right.transform.rotation;
        //handLInitDir = meshInitialJointLocations[JointType.HandTipLeft] - meshInitialJointLocations[JointType.HandLeft];
        //handLInitRot = Hand_Left.transform.rotation;


        for (int i = 0; i < meshActiveJoints.Length; i++)
        {
            GameObject jointController = new GameObject(meshActiveJoints[i].name + "_Controller");
            jointController.transform.parent = meshActiveJoints[i].transform.parent;
            jointController.transform.localPosition = meshActiveJoints[i].transform.localPosition;
            jointController.transform.localRotation = Quaternion.identity;
            jointController.transform.localScale = meshActiveJoints[i].transform.localScale;
            meshActiveJoints[i].transform.parent = jointController.transform;
            meshActiveJoints[i].transform.localPosition = Vector3.zero;
            meshActiveJoints[i].transform.localScale = Vector3.one;
            meshActiveJoints[i] = jointController;
        }
    }

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

        for (int idx = 0; idx < data.Length; idx++)
        {
            if (data[idx] == null)
            {
                continue;
            }

            if (data[idx].IsTracked  /*&& idx == modelIndex*/)
            {
                RefreshBodyObject(data[idx]);
            }
        }
    }

    private void ScaleBodyObject(Body body)
    {

        Vector3[] meshTempJointLocations = new Vector3[25];

        foreach (JointType jt in jointTypes)
        {

            if (jt.Equals(JointType.SpineShoulder))
            {
                meshActiveJoints[(int)jt].transform.position = GetVector3FromJoint(body.Joints[jt]);
                continue;
            }
            if (jt.Equals(JointType.Neck) || jt.Equals(JointType.ShoulderRight) || jt.Equals(JointType.ShoulderLeft) || jt.Equals(JointType.SpineMid))
            {
                meshTempJointLocations[(int)jt] = GetVector3FromJoint(body.Joints[jt]);
            }
            else
            {
                meshTempJointLocations[(int)jt] = GetVector3FromJoint(body.Joints[jt]) +
                    meshActiveJoints[(int)jointParents[jt]].transform.position - meshTempJointLocations[(int)jointParents[jt]];
            }

            Vector3 parenToChildVector = meshTempJointLocations[(int)jt] - meshActiveJoints[(int)jointParents[jt]].transform.position;

            meshActiveJoints[(int)jt].transform.position = meshActiveJoints[(int)jointParents[jt]].transform.position +
                parenToChildVector * ((meshInitialJointLocations[jt] - meshInitialJointLocations[jointParents[jt]]).magnitude / parenToChildVector.magnitude);
        }
    }

    private void RefreshBodyObject(Body body)
    {

        ScaleBodyObject(body);



        Vector3 quaternionEuler;

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.KneeRight]).eulerAngles;
        meshActiveJoints[(int)JointType.HipRight].transform.rotation = Quaternion.Euler(hipRightRotX, hipRightRotY, hipRightRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(quaternionEuler.x, Vector3.forward) *
            Quaternion.AngleAxis(-quaternionEuler.z, Vector3.right);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.AnkleRight]).eulerAngles;
        meshActiveJoints[(int)JointType.KneeRight].transform.rotation = Quaternion.Euler(kneeRightRotX, kneeRightRotY, kneeRightRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(quaternionEuler.x, Vector3.forward) *
            Quaternion.AngleAxis(-quaternionEuler.z, Vector3.right);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.KneeLeft]).eulerAngles;
        meshActiveJoints[(int)JointType.HipLeft].transform.rotation = Quaternion.Euler(hipLeftRotX, hipLeftRotY, hipLeftRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.forward) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.right);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.AnkleLeft]).eulerAngles;
        meshActiveJoints[(int)JointType.KneeLeft].transform.rotation = Quaternion.Euler(kneeLeftRotX, kneeLeftRotY, kneeLeftRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.forward) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.right);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.SpineMid]).eulerAngles;
        meshActiveJoints[(int)JointType.SpineBase].transform.rotation = Quaternion.Euler(spineBaseRotX, spineBaseRotY, spineBaseRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.z, Vector3.forward) *
            Quaternion.AngleAxis(quaternionEuler.x, Vector3.right);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.SpineShoulder]).eulerAngles;
        meshActiveJoints[(int)JointType.SpineMid].transform.rotation = Quaternion.Euler(spineMidRotX, spineMidRotY, spineMidRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.z, Vector3.forward) *
            Quaternion.AngleAxis(quaternionEuler.x, Vector3.right);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.Neck]).eulerAngles;
        meshActiveJoints[(int)JointType.SpineShoulder].transform.rotation = Quaternion.Euler(spineShoulderRotX, spineShoulderRotY, spineShoulderRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.z, Vector3.forward) *
            Quaternion.AngleAxis(quaternionEuler.x, Vector3.right);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.Neck]).eulerAngles;
        meshActiveJoints[(int)JointType.Neck].transform.rotation = Quaternion.Euler(neckRotX, neckRotY, neckRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.z, Vector3.forward) *
            Quaternion.AngleAxis(quaternionEuler.x, Vector3.right);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.ElbowRight]).eulerAngles;
        meshActiveJoints[(int)JointType.ShoulderRight].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(shoulderRightRotX, shoulderRightRotY, shoulderRightRotZ);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.ElbowLeft]).eulerAngles;
        meshActiveJoints[(int)JointType.ShoulderLeft].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(shoulderLeftRotX, shoulderLeftRotY, shoulderLeftRotZ);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.WristRight]).eulerAngles;
        meshActiveJoints[(int)JointType.ElbowRight].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(elbowRightRotX, elbowRightRotY, elbowRightRotZ);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.WristLeft]).eulerAngles;
        meshActiveJoints[(int)JointType.ElbowLeft].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(elbowLeftRotX, elbowLeftRotY, elbowLeftRotZ);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.HandRight]).eulerAngles;
        meshActiveJoints[(int)JointType.WristRight].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(wristRightRotX, wristRightRotY, wristRightRotZ);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.HandLeft]).eulerAngles;
        meshActiveJoints[(int)JointType.WristLeft].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(wristLeftRotX, wristLeftRotY, wristLeftRotZ);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.HandRight]).eulerAngles;
        meshActiveJoints[(int)JointType.HandRight].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(handRightRotX, handRightRotY, handRightRotZ);
        //Debug.Log("Hand R " + quaternionEuler);

        quaternionEuler = GetQuaternionFromJoint(body.JointOrientations[JointType.HandLeft]).eulerAngles;
        meshActiveJoints[(int)JointType.HandLeft].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(handLeftRotX, handLeftRotY, handLeftRotZ);
        //Debug.Log("Hand L" + quaternionEuler);

        //Vector3 wristRCurDir = meshActiveJoints[(int)JointType.HandRight].transform.GetChild(0).transform.position - meshActiveJoints[(int)JointType.WristRight].transform.GetChild(0).transform.position;
        //meshActiveJoints[(int)JointType.WristRight].transform.GetChild(0).transform.rotation = Quaternion.FromToRotation(wristRInitDir, wristRCurDir) * wristRInitRot;

        //Vector3 wristLCurDir = meshActiveJoints[(int)JointType.HandLeft].transform.GetChild(0).transform.position - meshActiveJoints[(int)JointType.WristLeft].transform.GetChild(0).transform.position;
        //meshActiveJoints[(int)JointType.WristLeft].transform.GetChild(0).transform.rotation = Quaternion.FromToRotation(wristLInitDir, wristLCurDir) * wristLInitRot;

        //Vector3 handRCurDir = meshActiveJoints[(int)JointType.HandTipRight].transform.GetChild(0).transform.position - meshActiveJoints[(int)JointType.HandRight].transform.GetChild(0).transform.position;
        //meshActiveJoints[(int)JointType.HandRight].transform.GetChild(0).transform.rotation = Quaternion.FromToRotation(handRInitDir, handRCurDir) * handRInitRot;

        //Vector3 handLCurDir = meshActiveJoints[(int)JointType.HandTipLeft].transform.GetChild(0).transform.position - meshActiveJoints[(int)JointType.HandLeft].transform.GetChild(0).transform.position;
        //meshActiveJoints[(int)JointType.HandLeft].transform.GetChild(0).transform.rotation = Quaternion.FromToRotation(handLInitDir, handLCurDir) * handLInitRot;

    }

    private static Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint, float scale = 5.0f)
    {
        return new Vector3(joint.Position.X * scale, joint.Position.Y * scale, joint.Position.Z * -scale);
    }

    private static Quaternion GetQuaternionFromJoint(Windows.Kinect.JointOrientation jointOrientation)
    {
        float ox = jointOrientation.Orientation.X;
        float oy = jointOrientation.Orientation.Y;
        float oz = jointOrientation.Orientation.Z;
        float ow = jointOrientation.Orientation.W;
        return new Quaternion(ox, oy, oz, ow);
    }

    public void EnableRendering()
    {
        GetComponent<Renderer>().enabled = true;
    }

    public void DisableRendering()
    {
        GetComponent<Renderer>().enabled = false;
    }

}

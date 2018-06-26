using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class AvatarModelController : MonoBehaviour 
{
    [Header("Manual Joint Rotations")]
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

    public float headRotX = 0.0f;
    public float headRotY = 0.0f;
    public float headRotZ = 0.0f;
    private Dictionary<JointType, Vector3> meshInitialJointLocations;
    private GameObject[] meshActiveJoints;

    public string ModelName { get; set; }

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

    void Awake () 
	{
        ModelName = this.transform.name;
        meshActiveJoints = new GameObject[Body.JointCount];
        meshInitialJointLocations = new Dictionary<JointType, Vector3>();
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            
            GameObject joint = this.transform.Find("Armature").Find(jt.ToString()).gameObject;
            meshActiveJoints[(int)jt] = joint;
            meshInitialJointLocations[jt] = joint.transform.position;
        }



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

        meshActiveJoints[(int)JointType.AnkleRight].transform.parent = meshActiveJoints[(int)JointType.KneeRight].transform.Find("KneeRight").transform;
        meshActiveJoints[(int)JointType.AnkleLeft].transform.parent = meshActiveJoints[(int)JointType.KneeLeft].transform.Find("KneeLeft").transform;

        meshActiveJoints[(int)JointType.FootRight].transform.parent = meshActiveJoints[(int)JointType.AnkleRight].transform.Find("AnkleRight").transform;
        meshActiveJoints[(int)JointType.FootLeft].transform.parent = meshActiveJoints[(int)JointType.AnkleLeft].transform.Find("AnkleLeft").transform;

        meshActiveJoints[(int)JointType.HandTipRight].transform.parent = meshActiveJoints[(int)JointType.HandRight].transform.Find("HandRight").transform;
        meshActiveJoints[(int)JointType.HandTipLeft].transform.parent = meshActiveJoints[(int)JointType.HandLeft].transform.Find("HandLeft").transform;

        meshActiveJoints[(int)JointType.ThumbRight].transform.parent = meshActiveJoints[(int)JointType.HandRight].transform.Find("HandRight").transform;
        meshActiveJoints[(int)JointType.ThumbLeft].transform.parent = meshActiveJoints[(int)JointType.HandLeft].transform.Find("HandLeft").transform;
    }

    public void RefreshModelData(Avatar.Body body)
    {

        RefreshModelPositions(body);
        
        Vector3 quaternionEuler;

        quaternionEuler = body.Joints[JointType.KneeRight].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.HipRight].transform.rotation = Quaternion.Euler(hipRightRotX, hipRightRotY, hipRightRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(quaternionEuler.x, Vector3.forward) *
            Quaternion.AngleAxis(-quaternionEuler.z, Vector3.right);

        quaternionEuler = body.Joints[JointType.AnkleRight].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.KneeRight].transform.rotation = Quaternion.Euler(kneeRightRotX, kneeRightRotY, kneeRightRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(quaternionEuler.x, Vector3.forward) *
            Quaternion.AngleAxis(-quaternionEuler.z, Vector3.right);

        quaternionEuler = body.Joints[JointType.KneeLeft].Rotation.eulerAngles; ;
        meshActiveJoints[(int)JointType.HipLeft].transform.rotation = Quaternion.Euler(hipLeftRotX, hipLeftRotY, hipLeftRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.forward) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.right);

        quaternionEuler = body.Joints[JointType.AnkleLeft].Rotation.eulerAngles; ;
        meshActiveJoints[(int)JointType.KneeLeft].transform.rotation = Quaternion.Euler(kneeLeftRotX, kneeLeftRotY, kneeLeftRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.forward) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.right);

        quaternionEuler = body.Joints[JointType.SpineMid].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.SpineBase].transform.rotation = Quaternion.Euler(spineBaseRotX, spineBaseRotY, spineBaseRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.z, Vector3.forward) *
            Quaternion.AngleAxis(quaternionEuler.x, Vector3.right);

        quaternionEuler = body.Joints[JointType.SpineShoulder].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.SpineMid].transform.rotation = Quaternion.Euler(spineMidRotX, spineMidRotY, spineMidRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.z, Vector3.forward) *
            Quaternion.AngleAxis(quaternionEuler.x, Vector3.right);

        quaternionEuler = body.Joints[JointType.Neck].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.SpineShoulder].transform.rotation = Quaternion.Euler(spineShoulderRotX, spineShoulderRotY, spineShoulderRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.z, Vector3.forward) *
            Quaternion.AngleAxis(quaternionEuler.x, Vector3.right);

        quaternionEuler = body.Joints[JointType.Head].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.Head].transform.rotation = Quaternion.Euler(headRotX, headRotY, headRotZ) *
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward);          

        quaternionEuler = body.Joints[JointType.ElbowRight].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.ShoulderRight].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(shoulderRightRotX, shoulderRightRotY, shoulderRightRotZ);

        quaternionEuler = body.Joints[JointType.ElbowLeft].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.ShoulderLeft].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(shoulderLeftRotX, shoulderLeftRotY, shoulderLeftRotZ);

        quaternionEuler = body.Joints[JointType.WristRight].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.ElbowRight].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(elbowRightRotX, elbowRightRotY, elbowRightRotZ);

        quaternionEuler = body.Joints[JointType.WristLeft].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.ElbowLeft].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(elbowLeftRotX, elbowLeftRotY, elbowLeftRotZ);

        quaternionEuler = body.Joints[JointType.HandRight].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.WristRight].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(wristRightRotX, wristRightRotY, wristRightRotZ);

        quaternionEuler = body.Joints[JointType.HandLeft].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.WristLeft].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(wristLeftRotX, wristLeftRotY, wristLeftRotZ);

        quaternionEuler = body.Joints[JointType.HandRight].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.HandRight].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(handRightRotX, handRightRotY, handRightRotZ);

        quaternionEuler = body.Joints[JointType.HandLeft].Rotation.eulerAngles;
        meshActiveJoints[(int)JointType.HandLeft].transform.rotation =
            Quaternion.identity *
            Quaternion.AngleAxis(-quaternionEuler.y, Vector3.up) *
            Quaternion.AngleAxis(-quaternionEuler.x, Vector3.right) *
            Quaternion.AngleAxis(quaternionEuler.z, Vector3.forward) *
            Quaternion.Euler(handLeftRotX, handLeftRotY, handLeftRotZ);
    }


    private void RefreshModelPositions(Avatar.Body body)
    {

        Vector3[] meshTempJointLocations = new Vector3[25];

        foreach (JointType jt in jointTypes) {

            if (jt.Equals(JointType.SpineShoulder)) {
                meshActiveJoints[(int)jt].transform.position = ScalePosition(body.Joints[jt].Position);
                continue;
            }
            if (jt.Equals(JointType.Neck) || jt.Equals(JointType.ShoulderRight) || jt.Equals(JointType.ShoulderLeft) || jt.Equals(JointType.SpineMid))
            {
                meshTempJointLocations[(int)jt] = ScalePosition(body.Joints[jt].Position);
            }
            else
            {
                meshTempJointLocations[(int)jt] = ScalePosition(body.Joints[jt].Position) + 
                    meshActiveJoints[(int)jointParents[jt]].transform.position - meshTempJointLocations[(int)jointParents[jt]];
            }

            Vector3 parenToChildVector = meshTempJointLocations[(int)jt] - meshActiveJoints[(int)jointParents[jt]].transform.position;

            meshActiveJoints[(int)jt].transform.position = meshActiveJoints[(int)jointParents[jt]].transform.position +
                parenToChildVector * ((meshInitialJointLocations[jt] - meshInitialJointLocations[jointParents[jt]]).magnitude / parenToChildVector.magnitude);
        } 
    }

    private Vector3 ScalePosition(Vector3 position, bool mirrored = true, float scale = 5.0f)
    {
        if (mirrored)
        {
            return new Vector3(position.x * scale, position.y * scale, position.z * -scale);
        }
        else
        {
            return new Vector3(position.x * scale, position.y * scale, position.z * scale);
        }
    }

    private static Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint, float scale = 5.0f)
    {
        return new Vector3(joint.Position.X * scale, joint.Position.Y * scale, joint.Position.Z * -scale);
    }

    private static Quaternion GetQuaternionFromJoint(JointOrientation jointOrientation) {
        float ox = jointOrientation.Orientation.X;
        float oy = jointOrientation.Orientation.Y;
        float oz = jointOrientation.Orientation.Z;
        float ow = jointOrientation.Orientation.W;
        return new Quaternion(ox, oy, oz, ow);
    }

    public void EnableRendering()
    {
        SetRendering(true);
    }

    public void DisableRendering()
    {
        SetRendering(false);
    }

    private void SetRendering(bool value)
    {
        this.transform.Find("Mesh").GetComponent<SkinnedMeshRenderer>().enabled = value;
    }

}

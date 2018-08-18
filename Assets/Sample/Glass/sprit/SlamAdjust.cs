using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SlamAdjust : MonoBehaviour
{
    public Text mLeftDistance;
    public Text mRightDistance;

    public Text mLeftPosition;
    public Text mRightPosition;

    public Text mLeftDifferenceValue;
    public Text mRightDifferenceValue;

    public Text mLeftDifferenceValue2;
    public Text mRightDifferenceValue2;

    public GameObject mHead;

    private Vector3 mLastDistance;
    private Vector3 mCurrentDistance;

    private static string mRecordPoseFilePath = "/sdcard/SlamAdjust/";
    private static string mRecordPoseFileName = "RecordPose.txt";

    private void Start()
    {
        ThreadPool.SetMinThreads(1, 1);
        ThreadPool.SetMaxThreads(5, 5);
        if (File.Exists(mRecordPoseFilePath + mRecordPoseFileName)) {
            File.Delete(mRecordPoseFilePath + mRecordPoseFileName);
        }
    }

    void Update()
    {
        var currentPosition = transform.position;
        Ray ray = new Ray(currentPosition, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            var point = hit.point;
            var distance = Vector3.Distance(currentPosition, point);
            mLeftDistance.text = distance.ToString("F4");
            mRightDistance.text = distance.ToString("F4");
        }
        else
        {
            mLeftDistance.text = "∞";
            mRightDistance.text = "∞";
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            mCurrentDistance = transform.position;
            var distance = Vector3.Distance(mCurrentDistance, mLastDistance).ToString("F4");
            mLastDistance = mCurrentDistance;
            mLeftDifferenceValue.text = distance;
            mRightDifferenceValue.text = distance;
            Pose6Dof headPose = new Pose6Dof();
            headPose.position = mHead.transform.position;
            headPose.rotation = mHead.transform.rotation;
            ThreadPool.QueueUserWorkItem(new WaitCallback(write6Dof), headPose);
        }
    }

    private void OnGUI()
    {
        var position = "x:" + transform.position.x.ToString("F4") + "\ny:" + transform.position.y.ToString("F4") + "\nz:" + transform.position.z.ToString("F4");
        mLeftPosition.text = position;
        mRightPosition.text = position;

        var distance = Vector3.Distance(transform.position, mLastDistance).ToString("F4");
        mLeftDifferenceValue2.text = distance;
        mRightDifferenceValue2.text = distance;

    }

    private static void write6Dof(object pose) {
        if (!Directory.Exists(mRecordPoseFilePath))
        {
            Directory.CreateDirectory(mRecordPoseFilePath);
        }

        if (!File.Exists(mRecordPoseFilePath+ mRecordPoseFileName))
        {
            FileStream fs = new FileStream(mRecordPoseFilePath + mRecordPoseFileName, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            fs.Seek(0, SeekOrigin.End);
            sw.WriteLine(((Pose6Dof)pose).ToString());//开始写入值
            sw.Close();
            fs.Close();
        }
        else
        {
            FileStream fs = new FileStream(mRecordPoseFilePath + mRecordPoseFileName, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            fs.Seek(0, SeekOrigin.End);
            sw.WriteLine(((Pose6Dof)pose).ToString());//开始写入值
            sw.Close();
            fs.Close();
        }
    }
}

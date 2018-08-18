using UnityEngine;

public class CheckerManager : MonoBehaviour
{

    public Transform mChecker;

    private float mRotation = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void adjustLocalRotationX(float rotation)
    {
        var lastRotation = mChecker.transform.localRotation.eulerAngles;
        mChecker.transform.localRotation = Quaternion.Euler(-rotation, lastRotation.y, lastRotation.z);
    }

    public void adjustLocalRotationY(float rotation)
    {
        var lastRotation = mChecker.transform.localRotation.eulerAngles;
        mChecker.transform.localRotation = Quaternion.Euler(lastRotation.x, rotation, lastRotation.z);
    }

    public void adjustLocalRotationZ(float rotation)
    {
        var lastRotation = mChecker.transform.localRotation.eulerAngles;
        mChecker.transform.localRotation = Quaternion.Euler(lastRotation.x, lastRotation.y, -rotation);
    }

    public void adjustLocalPositionX(float position)
    {
        var lastPosition = mChecker.transform.localPosition;
        mChecker.transform.localPosition = new Vector3(-position, lastPosition.y, lastPosition.z);
    }

    public void adjustLocalPositionY(float position)
    {
        var lastPosition = mChecker.transform.localPosition;
        mChecker.transform.localPosition = new Vector3(lastPosition.x, position, lastPosition.z);
    }

    public void adjustLocalPositionZ(float position)
    {
        var lastPosition = mChecker.transform.localPosition;
        mChecker.transform.localPosition = new Vector3(lastPosition.x, lastPosition.y, -position);
    }

    public void resetLocalPosition()
    {
        mChecker.transform.localPosition = Vector3.zero;
    }

    public void resetLocalRotation()
    {
        mChecker.transform.localRotation = Quaternion.identity;
    }
}

using UnityEngine;

public class Pose6Dof
{
    //toString 格式是欧拉角还是四元数
    bool isQuaternion = false;

    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;

    public string ToString()
    {
        if (isQuaternion)
        {
            return "position x:" + position.x + " y:" + position.y + " z:" + position.z
          + " rotationQuaternion x:" + rotation.x + " y:" + rotation.y + " z:" + rotation.z + " w:" + rotation.z;
        }
        else
        {
            return "position x:" + position.x + " y:" + position.y + " z:" + position.z
          + " rotationEuler x:" + rotation.eulerAngles.x + " y:" + rotation.eulerAngles.y + " z:" + rotation.eulerAngles.z;
        }

    }

}

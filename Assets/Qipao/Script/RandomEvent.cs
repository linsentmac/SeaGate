using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEvent : MonoBehaviour {
    private List<GameObject> qipaoList = new List<GameObject>();
    public bool delayDispaly = false;//是否延迟显示
    public bool changePos = false;//是否每隔一段时间随机改变位置

    // Use this for initialization
    void Start() {
        //先全部隐藏
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
            qipaoList.Add(child.gameObject);
        }

        //本次要显示的泡泡个数
        int sum = UnityEngine.Random.Range(4, 8);

        //得到要显示的泡泡的下标集合
        int[] arr = getRandoms(sum, 0, 9);

        //显示泡泡，根据泡泡数量延迟不同的秒数再显示
        StartCoroutine(DelayToInvokeDo(() =>
        {
            showPaopao(arr);
            changPosition();
        }, delayDispaly ? sum % 4 * 2 : 0));
    }

    // Update is called once per frame
    void Update() {

    }

    //从最小值与最大值之间获取sum个不重复的随机数
    public static int[] getRandoms(int sum, int min, int max) {
        int[] arr = new int[sum];
        int j = 0;
        //表示键和值对的集合。
        Hashtable hashTable = new Hashtable();
        while (hashTable.Count < sum) {
            //返回一个min到max之间的随机数
            int nValue = UnityEngine.Random.Range(min, max);
            // 是否包含特定值
            if (!hashTable.ContainsValue(nValue)) {
                //把键和值添加到hashtable
                hashTable.Add(nValue, nValue);
                arr[j] = nValue;
                j++;
            }
        }

        return arr;
    }

    //显示泡泡
    private void showPaopao(int[] arr) {
        for (int i = 0; i < arr.Length; i++) {
            qipaoList[arr[i]].SetActive(true);
        }
    }

    //每隔10s随机改变位置
    private void changPosition() {
        if (changePos) {
            StartCoroutine(DelayToInvokeDo(() =>
            {
                float posX = transform.position.x;
                float posZ = transform.position.z;
                float offX = UnityEngine.Random.Range(-1, 2);
                float offZ = UnityEngine.Random.Range(-1, 2);

                //控制泡泡的边界
                while (posX + offX >= 20 / 5 || posX + offX <= -20 / 5) {
                    offX = UnityEngine.Random.Range(-1, 2);
                }
                while (posZ + offZ >= 45 / 5 || posZ + offZ <= 15 / 5) {
                    offZ = UnityEngine.Random.Range(-1, 2);
                }

                //变换位置
                transform.position = new Vector3(posX + offX, transform.position.y, posZ + offZ);
                //变换角度
                transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + UnityEngine.Random.Range(25, 45), transform.rotation.eulerAngles.z);

                //递归
                changPosition();
            }, 10));
        }
    }

    public static IEnumerator DelayToInvokeDo(Action action, float delaySeconds) {
        yield return new WaitForSeconds(delaySeconds);
        action();
    }

}

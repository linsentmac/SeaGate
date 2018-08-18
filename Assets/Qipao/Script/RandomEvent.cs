using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEvent : MonoBehaviour {
    private List<GameObject> qipaoList = new List<GameObject>();

    // Use this for initialization
    void Start() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
            qipaoList.Add(child.gameObject);
        }

        //本次要显示的泡泡个数
        int sum = Random.Range(5, 10);

        //得到要显示的泡泡的下标集合
        int[] arr = getRandoms(sum, 0, 9);

        //显示泡泡
        showPaopao(arr);
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
            int nValue = Random.Range(min, max);
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

}

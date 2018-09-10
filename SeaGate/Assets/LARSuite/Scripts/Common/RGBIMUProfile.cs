using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class RGBIMUProfile  {
    //RGBIMU的外参
    class dateList
    {
        public List<double > Data;

    }
   private static TextAsset jsonsText;

   private  static  dateList date;


    private List<double> dtes;
    public static List <double >RGBIMUDdtesFun()
    {
        List<double> RGBIMUDdtes =new List<double> ();
        //jsonsText = Resources.Load("/persist/sensors/rgbtoimu.txt") as TextAsset;
        //  date = JsonMapper.ToObject<dateList>(jsonsText.text);
        
        string path = "/persist/sensors/rgbtoimu.txt";
        byte[] b = new byte[1024 * 1024];
        FileStream fileRead = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
        int r = fileRead.Read(b, 0, b.Length);

        // 新增了这两行代码
        fileRead.Close();
        fileRead.Dispose();

        string contents = Encoding.UTF8.GetString(b, 0, r);
        date = JsonMapper.ToObject<dateList>(contents);


        for (int i = 0; i < date.Data.Count; i++)
        {
          
            RGBIMUDdtes.Add(date.Data[i]);
        }

        return RGBIMUDdtes;
    }

 
}

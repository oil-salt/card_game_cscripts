using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;   //反射
using System.IO;    //读写
using System.Text;  


//---------------Mix in---------------------
public interface iDied
{
        
}
public static class ExtendDied
{
    public static void tryDie<T>(this T example) where T : iDied
    {
        Debug.Log("屎啊");
    }
}
//-----------------------------------------

public class MainCamera : MonoBehaviour,iDied {

    class ModelTest
    {
        public string name;
        public int age;
        public string[] interests;
        public string morethings;
        public string toString()
        {
            string s= name + "\n" + age.ToString() + "\n" + morethings + "\n";
            foreach (string inter in interests) s = s + inter + "\n";
            return s;
        }
    }

    // Use this for initialization
    void Start () {
        MainClasses.Character myself = new MainClasses.Character();
        myself.setName("hhh").setDescribe("你们的爸爸")
            .setOriginHp(2);
        //用函数名调用函数的例子，注意到这个Invoke必须要对实例才起作用
        Type type = typeof(MainClasses.Character);
        int speed_ori = 2;
        object[] para = new object[] { speed_ori };
        type.GetMethod("setOriginSpeed").Invoke(myself, para);
        //
        myself.creat();
        Debug.Log(myself.info());
        this.tryDie();

        //------------Json字符串创建对象------------------      
        //string jsontext = File.ReadAllText("/Unity Projects/Boardgame-type17/Assets/Data/Test.json", Encoding.UTF8);
        //Debug.Log(jsontext);
        //ModelTest obj = JsonUtility.FromJson<ModelTest>(jsontext);
        //Debug.Log(obj.toString());
        //-----------------------------------------------
        MainClasses.Global_abilities g = new MainClasses.Global_abilities();
        MainClasses.Skill skill = g.creatSkillfromConfig("火球术", myself);
        Debug.Log(skill.info());
        skill.save();   //保存为json
        //---------------------------------------------
        List<string> ss = new List<string>();
        ss.Add("abc");
        ss.Add("def");
        ss.Add("def");
        ss.Add("ghi");
        for (int i = 0; i < ss.Count; i++)
            Debug.Log(ss[i]);
        Debug.Log("--------------");
        ss.Remove("def");
        for (int i = 0; i < ss.Count; i++)
            Debug.Log(ss[i]);
        ss.Remove("nothing");
        Debug.Log("--------------");
        List<string> re_ss = ss;
        while (re_ss.Count > 0)
            ss.Remove(re_ss[0]);
        Debug.Log(ss.Count);
        Debug.Log("--------------");
        System.Random ra = new System.Random();
        int roll = ra.Next(11) + 2;
        Debug.Log(roll);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

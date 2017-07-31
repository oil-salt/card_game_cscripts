using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;   //反射

public class MainCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MainClasses.character myself = new MainClasses.character();
        myself.setName("hhh").setDescribe("你们的爸爸")
            .setOriginHp(2);
        //用函数名调用函数的例子，注意到这个Invoke必须要对实例才起作用
        Type type = typeof(MainClasses.character);
        int speed_ori = 2;
        object[] para = new object[] { speed_ori };
        type.GetMethod("setOriginSpeed").Invoke(myself, para);
        //
        myself.Creat();
        Debug.Log(myself.info());
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

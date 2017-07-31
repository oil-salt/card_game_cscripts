using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainClasses : MonoBehaviour {

    public class character
    {
        private string name;
        private string describe;
        private int hp_ori, hp_cur_max, hp_cur;
        private int speed_ori, speed_cur;
        public character()
        {
            //初始化
        
        }
        public character setName(string s)
        {
            name = s;
            return this;
        }
        public character setDescribe(string s)
        {
            describe = s;
            return this;
        }
        public character setOriginHp(int i)
        {
            hp_ori = i;
            return this;
        }
        public character setOriginSpeed(int i)
        {
            speed_ori = i;
            return this;
        }
        public character Creat()
        {
            //扫描牌组组，并初始化数值
            hp_cur_max = hp_ori;
            hp_cur = hp_cur_max;
            speed_cur = speed_ori;
            return this;
        }
        public string info()
        {
            string ans="";
            ans = "名字：" + name + "\n"
                + "介绍：" + describe + "\n"
                + "血量：" + hp_cur + "/" + hp_cur_max + "\n"
                + "速度：" + speed_cur + "\n";
            //Debug.Log("hey");
            return ans;
        }
        

    }

}

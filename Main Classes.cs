using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;   //反射

public class MainClasses : MonoBehaviour {
    public class Global_abilities
    {
        private Hashtable name2describe = new Hashtable();
        public void addDescribe(string name,string describe)
        {
            name2describe.Add(name, describe);
        }
        public string return_Describe_of_Ability(string name)
        {
            string ans=(string)name2describe[name];
            return ans;
        }
    }
    public class Buff
    {
        private string name;
        private string describe;
        private Character belongsto;
        private Global_abilities abilities;
        public Buff()
        {
            //do nothing
        }
        public Buff setName(string s)
        {
            name = s;
            return this;
        }
        public Buff setDescribe(string s)
        {
            describe = s;
            return this;
        }
        public Buff setBelongsTo(Character c)
        {
            belongsto=c;
            return this;
        }
        public Buff setGlobalAbilities(Global_abilities g)
        {
            abilities=g;
            return this;
        }
        public Buff init()
        {
            //为在Global中能以名字查到Describe
            abilities.addDescribe(name,describe);
            return this;
        }
        //
        public void takeEffect()
        {
            Type type=typeof(MainClasses.Global_abilities);
            object[] para=new object[]{belongsto};  //buff类型的参数，只有buff所属角色一个
            type.GetMethod(name).Invoke(abilities,para);
        }
    }
    public class Character
    {
        private string name;
        private string describe;
        private int hp_ori, hp_cur_max, hp_cur;
        private int speed_ori, speed_cur;
        List<Buff> buffs = new List<Buff>();
        private Global_abilities abilities;

        public Character()
        {    
            //初始化
            //do nothing
        }
        public Character setName(string s)
        {
            name = s;
            return this;
        }
        public Character setDescribe(string s)
        {
            describe = s;
            return this;
        }
        public Character setOriginHp(int i)
        {
            hp_ori = i;
            return this;
        }
        public Character setOriginSpeed(int i)
        {
            speed_ori = i;
            return this;
        }
        public Character addNewBuff(string s)
        {
            Buff new_buff = new Buff();
            new_buff.setName(s).setDescribe(abilities.return_Describe_of_Ability(s))
                .setBelongsTo(this).setGlobalAbilities(abilities);
            buffs.Add(new_buff);
            return this;
        }
        public Character setGlobalAbilities(Global_abilities g)
        {
            abilities = g;
            return this;
        }
        public Character creat()
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

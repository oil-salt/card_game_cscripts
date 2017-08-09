using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;   //反射
using System.IO;    //读写
using System.Text;

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
        public Skill creatSkillfromConfig(string name_Skill,Character c)
        {
            string jsontext = File.ReadAllText("/Unity Projects/Boardgame-type17/Assets/Data/Skill/" + name_Skill + ".json", Encoding.UTF8);           
            Skill skill_json = JsonUtility.FromJson<Skill>(jsontext);
            skill_json.setGlobalAbilities(this).setSkillCaster(c);            
            return skill_json;
        }
        //-------------------------单体技能-------------------------------------
        //----------------------------------------------------------------------
        //-------------------------AOE技能--------------------------------------
        //----------------------------------------------------------------------
        //-------------------------状态Buff-------------------------------------
        //----------------------------------------------------------------------
    }
    public class Buff
    {
        private string name;
        private string describe;
        private Character belongsto;
        private Global_abilities abilities;
        private string return_Message;
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
        public string takeEffect()
        {
            return_Message="";
            Type type=typeof(MainClasses.Global_abilities);
            object[] para=new object[]{belongsto};  //buff类型的参数，只有buff所属角色一个
            try
            {
                type.GetMethod(name).Invoke(abilities,para);
            }
            catch
            {
                return_Message="No such Buff";
            }
            return return_Message;
        }
    }
    public class Skill
    {
        public string name;
        public string describe;
        private Character caster;
        private Global_abilities abilities;
        public string skill_Type;  //AOE, or Single
        private string return_Message;
        public Skill()
        {
            //do nothing
        }
        public Skill setName(string s)
        {
            name = s;
            return this;
        }
        public Skill setDescribe(string s)
        {
            describe = s;
            return this;
        }
        public Skill setSkillCaster(Character c)
        {
            caster=c;
            return this;
        }
        public Skill setGlobalAbilities(Global_abilities g)
        {
            abilities=g;
            return this;
        }
        public Skill setSkillType(string s)
        {
            skill_Type=s;
            return this;
        }
        public Skill init()
        {
            //为在Global中能以名字查到Describe
            abilities.addDescribe(name,describe);
            return this;
        }
        //
        public string skill_Single(Character target)
        {
            return_Message="";
            Type type=typeof(MainClasses.Global_abilities);
            object[] para=new object[]{caster,target,this};  //单体技能类型的参数，只有施法者和目标，以及自身用来接收返回
            try
            {
                type.GetMethod(name).Invoke(abilities,para);
            }
            catch
            {
                return_Message="No such Skill";
            }
            return return_Message;
        }
        public string skill_AOE(Character[] multi_target)
        {
            return_Message="";
            Type type=typeof(MainClasses.Global_abilities);
            object[] para=new object[]{caster,multi_target,this};    //AOE技能的参数，包含了施法者，目标数组，以及自身用来接收返回
            try
            {
                type.GetMethod(name).Invoke(abilities,para);
            }
            catch
            {
                return_Message="No such Skill";
            }
            return return_Message;
        }
        public string info()
        {
            string ans = "";
            ans = "名称：" + name + "\n"
                + "描述：" + describe + "\n"
                + "类型：" + skill_Type + "\n";
            return ans;
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
        public Character addNewBuff(string buffName)
        {
            Buff new_buff = new Buff();
            new_buff.setName(buffName).setDescribe(abilities.return_Describe_of_Ability(buffName))
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

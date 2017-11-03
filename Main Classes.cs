using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;   //反射
using System.IO;    //读写
using System.Text;

public class MainClasses : MonoBehaviour
{
    private class fit_Skill
    {
        public string name;
        public string describe;
        public string skill_Type;
        public int range_max, range_min;
        public List<string> require_Cost;
    }
    public class Cards
    {
        public string name;
        public string describe;
    }
    public class Global_abilities
    {
        //-----------------------全局变量---------------------------------------
        List<Character> all_characters = new List<Character>();
        //======================================================================
        private Hashtable name2describe = new Hashtable();
        public void addDescribe(string name, string describe)
        {
            name2describe.Add(name, describe);
        }
        public string return_Describe_of_Ability(string name)
        {
            string ans = (string)name2describe[name];
            return ans;
        }
        public Skill creatSkillfromConfig(string name_Skill, Character c)
        {
            string jsontext = File.ReadAllText(Application.dataPath + "/Data/Skill/" + name_Skill + ".json", Encoding.UTF8);
            fit_Skill skill_json = JsonUtility.FromJson<fit_Skill>(jsontext);
            Skill ans = new Skill();
            ans.setName(skill_json.name)
                .setDescribe(skill_json.describe)
                .setSkillType(skill_json.skill_Type)
                .setGlobalAbilities(this).setSkillCaster(c)
                .setRange(skill_json.range_max, skill_json.range_min)
                .setRequireCost(skill_json.require_Cost);
            return ans;
        }
        //-----------------------各种获取---------------------------------------
        public List<Character> get_same_position(Character standard)
        {
            List<Character> ans = new List<Character>();
            all_characters.ForEach(delegate (Character c)
            {
                if (c.get_battle_position() == standard.get_battle_position() && c.get_team_tag() == standard.get_team_tag())
                    ans.Add(c);
            });
            return ans;
        }
        //----------------------------------------------------------------------
        //-----------------------各种检定---------------------------------------
        public bool check_require_cost(List<Cards> cost, Skill skill)
        {
            bool ans = false;
            List<string> require_cost = skill.getRequireCost();
            string tag;
            if (cost.Count >= require_cost.Count)
                if (cost.Count > 1)
                    for (int i = 0; i < cost.Count; i++)
                    {
                        if (cost[i].name == "攻击") tag = cost[i].name;
                        else if (cost[i].name == "移动") tag = cost[i].name;
                        else tag = "普通";
                        require_cost.Remove(tag);
                    }
            if (require_cost.Count == 0)
            {
                //消耗已经满足，将消耗的牌移除掉
                skill.getCaster().consume_Cards(cost);
                ans = true;
            }
            return ans;
        }
        public bool check_range_single(Character speller, Character target, int max, int min)
        {
            bool ans = false;
            int range = speller.get_battle_position() + target.get_battle_position() + 1;
            if (range >= min && range <= max)
                ans = true;
            return ans;
        }     
        public bool roll_DC(int dc)
        {
            System.Random ra = new System.Random();
            int roll = ra.Next(11) + 2;   //返回是一个小于，并且大于等于0的整数
            bool ans = false;
            if (roll >= dc) ans = true;
            return ans;
        }
        //----------------------------------------------------------------------
        //-------------------------单体技能-------------------------------------
        public void 火球术(Character speller, Character target, List<Cards> cost, Skill skill)
        {
            //先检查是否满足消耗
            bool cost_check = check_require_cost(cost, skill);
            bool range_check = check_range_single(speller, target, skill.get_range_max(), skill.get_range_min());
            //再检查距离
            if (cost_check && range_check)
            {
                //此处开始自定义技能效果
                //·1钝击伤害 0DC
                //·1高温伤害 4DC
                if (roll_DC(2))
                    target.take_钝击伤害(1);
                if (roll_DC(6))
                    target.take_火焰伤害(1);                
            }
            else
            {
                //返回消耗不足
            }
        }
        public void 类火球术(Character target_self)
        {
            if (roll_DC(2))
                target_self.take_钝击伤害(1);
            if (roll_DC(6))
                target_self.take_火焰伤害(1);
        }
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
            belongsto = c;
            return this;
        }
        public Buff setGlobalAbilities(Global_abilities g)
        {
            abilities = g;
            return this;
        }
        public Buff init()
        {
            //为在Global中能以名字查到Describe
            abilities.addDescribe(name, describe);
            return this;
        }
        //
        public string takeEffect()
        {
            return_Message = "";
            Type type = typeof(MainClasses.Global_abilities);
            object[] para = new object[] { belongsto };  //buff类型的参数，只有buff所属角色一个
            try
            {
                type.GetMethod(name).Invoke(abilities, para);
            }
            catch
            {
                return_Message = "No such Buff";
            }
            return return_Message;
        }
    }
    public class Skill
    {
        private string name;
        private string describe;
        private Character caster;
        private Global_abilities abilities;
        private string skill_Type;  //AOE, or Single
        private List<string> require_Cost;
        private List<Cards> extra_Cost;
        private int range_max, range_min;
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
        public Skill setRange(int max, int min)
        {
            this.range_max = max;
            this.range_min = min;
            return this;
        }
        public int get_range_max()
        {
            return this.range_max;
        }
        public int get_range_min()
        {
            return this.range_min;
        }
        public Skill setSkillCaster(Character c)
        {
            caster = c;
            return this;
        }
        public Character getCaster()
        {
            return caster;
        }
        public Skill setGlobalAbilities(Global_abilities g)
        {
            abilities = g;
            return this;
        }
        public Skill setSkillType(string s)
        {
            skill_Type = s;
            return this;
        }
        public Skill setRequireCost(List<string> ss)
        {
            require_Cost = ss;
            return this;
        }
        public List<string> getRequireCost()
        {
            return require_Cost;
        }
        public Skill init()
        {
            //为在Global中能以名字查到Describe
            abilities.addDescribe(name, describe);
            return this;
        }
        //
        public string skill_Single(Character target, List<Cards> cost)
        {
            return_Message = "";
            Type type = typeof(MainClasses.Global_abilities);
            object[] para = new object[] { caster, target, cost, this };  //单体技能类型的参数，只有施法者和目标，以及自身用来接收返回
            try
            {
                type.GetMethod(name).Invoke(abilities, para);
            }
            catch
            {
                return_Message = "No such Skill";
            }
            return return_Message;
        }
        public string skill_AOE(Character[] multi_target, List<Cards> cost)
        {
            return_Message = "";
            Type type = typeof(MainClasses.Global_abilities);
            object[] para = new object[] { caster, multi_target, cost, this };    //AOE技能的参数，包含了施法者，目标数组，以及自身用来接收返回
            try
            {
                type.GetMethod(name).Invoke(abilities, para);
            }
            catch
            {
                return_Message = "No such Skill";
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
        public void save()
        {
            fit_Skill jSkill = new fit_Skill();
            jSkill.name = name;
            jSkill.describe = describe;
            jSkill.skill_Type = skill_Type;
            jSkill.require_Cost = require_Cost;
            jSkill.range_max = range_max;
            jSkill.range_min = range_min;
            string jString = JsonUtility.ToJson(jSkill, true);
            string path = Path.Combine(Application.dataPath + "/Data/Skill/", name + ".json");
            File.WriteAllText(path, jString);
        }
    }
    public class Character
    {
        private string name;
        private string describe;
        private int hp_ori, hp_cur_max, hp_cur;
        private int speed_ori, speed_cur;
        private int battle_position;
        private string team_tag;
        private List<Cards> cards_on_hand;
        public Cards Environment;
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
        public Character set_team_tag(string s)
        {
            this.team_tag = s;
            return this;
        }
        //------------------------------------------------
        public int get_battle_position()
        {
            return this.battle_position;
        }
        public string get_team_tag()
        {
            return this.team_tag;
        }
        //================================================
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
        public Character set_Environment(string env)
        {
            string jsontext = File.ReadAllText(Application.dataPath + "/Data/Environment/" + env + ".json", Encoding.UTF8);
            Cards card_json = JsonUtility.FromJson<Cards>(jsontext);
            Environment = card_json;
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
        public void consume_Cards(List<Cards> l)
        {
            while (l.Count > 0)
                cards_on_hand.Remove(l[0]);
        }
        public string info()
        {
            string ans = "";
            ans = "名字：" + name + "\n"
                + "介绍：" + describe + "\n"
                + "血量：" + hp_cur + "/" + hp_cur_max + "\n"
                + "速度：" + speed_cur + "\n";
            //Debug.Log("hey");
            return ans;
        }
        //----------------各种属性伤害效果-----------------
        public void take_钝击伤害(int amount)
        {
            hp_cur -= amount;
            if (Environment.name == "冰面")
            {
                set_Environment("击倒");
            }
            check_dead();
        }
        public void take_火焰伤害(int amount)
        {
            hp_cur -= amount;
            if (Environment.name=="毒")
            {
                set_Environment("普通");
                abilities.类火球术(this);
            }
            if (Environment.name == "油")
            {
                set_Environment("黑烟");
                abilities.类火球术(this);
            }
        }
        //----------------各种检查-------------------------
        public void check_dead()
        {
            if (hp_cur<0)
            {
                //挂了
            }
        }
    }

    public class Simplify
    {
        public class basic_info
        {
            public string name;
            public string info;
        }
        public class Card
        {
            public basic_info info;
            public Character belong;
            public void set_info(basic_info inf)
            {
                this.info = inf;
            }
            public string toString()
            {
                return "名称：" + info.name + "\n" +
                    "描述：" + info.info;
            }
            public string name()
            {
                return info.name;
            }
        }
        public class Character
        {
            public basic_info info;
            public int hp, current_hp;  //血量
            public int spd;             //速度
            public global glo;                 //全局变量与函数
            public List<Card> hand_cards;      //手牌
            public Card ground_card;           //地形牌
            public List<Card> abandon_cards;   //弃牌
            public List<Card> pakage_cards;    //牌组
            public List<Card> buff_cards;      //状态栏
            //加入牌组
            public void add_cards(string name, string to_where)
            {
                Card new_card = glo.creatSkillfromConfig(name);
                if (to_where == "pakage")
                    pakage_cards.Add(new_card);
                else if (to_where == "hand")
                    hand_cards.Add(new_card);
                else if (to_where == "abandon")
                    abandon_cards.Add(new_card);
                else if (to_where == "buff")
                    buff_cards.Add(new_card);
                else
                    Debug.Log("不存在这个类型");
            }
            //改变地形
            public void change_ground(string name)
            {
                Card new_card = glo.creatSkillfromConfig(name);
                ground_card = new_card;
            }
            //移除卡
            public void remove_card(Card card, string from_where)
            {
                if (from_where == "pakage")
                    pakage_cards.Remove(card);
                else if (from_where == "hand")
                    hand_cards.Remove(card);
                else if (from_where == "abandon")
                    abandon_cards.Remove(card);
                else if (from_where == "buff")
                    buff_cards.Remove(card);
                else
                    Debug.Log("不存在这个类型");
            }
            public void take_damage(int d)
            {
                current_hp -= d;
            }
            public string toString()
            {
                string ans = "";
                ans = "名称：" + info.name + "\n" +
                    "血量：" + current_hp + "/" + hp + "\n" +
                    "速度：" + spd + "\n" +
                    "持有手牌：\n";
                int count = hand_cards.Count;
                if (count == 0)
                    ans += "无\n";
                else for (int i = 0; i < count; i++)
                        ans += "·" + hand_cards[i].name() + "\n";
                ans += "描述：" + info.info;
                return ans;
            }
        }
        public class global
        {
            private class temp_Character
            {
                public string name, info;
                public int hp, cur_hp, spd;
                public string hand, ground, abandon, package, buff;
            }
            public bool roll_DC(int dc)
            {
                System.Random ra = new System.Random();
                int roll=ra.Next(11)+2;   //返回是一个小于，并且大于等于0的整数
                bool ans = false;
                if (roll > dc) ans = true;
                return ans;
            }

            public void dc_with_string(int dc,string str)
            {
                string s = "*"+dc.ToString()+"DC ";
                if (roll_DC(dc))
                    s = s + "成功！ ";
                else
                    s = s + "失败！ ";
                s= s + str;
                Debug.Log(s);
            }

            public Card creatSkillfromConfig(string name_Skill)
            {
                string jsontext = File.ReadAllText(Application.dataPath + "/Simple_Data/Skill/" + name_Skill + ".json", Encoding.UTF8);
                basic_info card_json = JsonUtility.FromJson<basic_info>(jsontext);
                Card ans = new Card();
                ans.set_info(card_json);
                return ans;
            }
            public Character load_character_from_save(string id)
            {
                string json_Character = File.ReadAllText(Application.dataPath + "/Simple_Data/Character/" + id + ".json", Encoding.UTF8);
                temp_Character tmp = JsonUtility.FromJson<temp_Character>(json_Character);
                Character cha = new Character();
                cha.info = new basic_info();
                cha.info.name = tmp.name;
                cha.info.info = tmp.info;
                return cha;
            }
        }
    }

}

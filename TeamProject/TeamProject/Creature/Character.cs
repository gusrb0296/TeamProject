﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TeamProject
{
    public class Character : Creature
    {
        public string Job { get; set; }
        public float HpMax => DefaultHpMax + hpMaxModifier;
        public float Damage => DefaultDamage + damageModifier;
        public float Defense => DefaultDefense + defenseModifier;
        public float MpMax => DefaultMpMax + mpMaxModifier;
        public float Critical => DefaultCritical + criticalModifier;
        public float Avoid => DefaultAvoid + avoidModifier;

        public int NextLevelExp;
        public int TotalExp;

        public Skill PlayerSkill;

        public int Gold { get; set; }
        public override float Hp
        {
            get => hp;
            set
            {
                if (value <= 0) hp = 0;
                else if (value >= HpMax) hp = HpMax;
                else hp = value;
            }
        }
        protected float mp;
        public float Mp
        {
            get => mp;
            set
            {
                if (value <= 0) mp = 0;
                else if (value >= MpMax) mp = MpMax;
                else mp = value;
            }
        }
        public Inventory Inventory { get; set; }
        public Equipment Equipment { get; set; }

        public float hpMaxModifier;
        public float damageModifier;
        public float defenseModifier;
        public float mpMaxModifier;
        public float criticalModifier;
        public float avoidModifier;

        //public Character() { }
        public Character(string name, string job, int level, float damage, float defense, float hp, float mp, int gold, float critical, float avoid, Skill playerSkill)
        {
            Name = name;
            Job = job;
            Level = level;
            DefaultDamage = damage;
            DefaultDefense = defense;
            DefaultHpMax = hp;
            DefaultMpMax = mp;
            DefaultCritical = critical;
            DefaultAvoid = avoid;
            Gold = gold;

            Inventory = new Inventory(this);
            Equipment = new Equipment();
            NextLevelExp = 100;
            TotalExp = 0;
            Hp = HpMax;
            Mp = MpMax;
            PlayerSkill = playerSkill;
        }

        public override void Attack(Creature creature, int line)
        {
            int printWidthPos = Console.WindowWidth / 2;
            bool isCritical = false;
            for (int i = 0; i < 5; i++)
            {
                Renderer.Print(line + i, new string(' ', printWidthPos - 1), false, 0, printWidthPos);
            }
            if (RandomChance(creature.DefaultAvoid)) // 상대방이 회피 했을때
            {
                Renderer.Print(line++, $"{creature.Name}이(가) 회피했습니다!", false, textdelay, printWidthPos);
            }
            else // 공격에 성공 했을 때
            {
                float damage = DefaultDamage;
                // 일정 확률로 치명타 적용
                if (RandomChance(DefaultCritical))
                {
                    isCritical = true;
                    damage *= 1.5f; // 기본 데미지의 1.5배
                }

                int finalDamage = Math.Clamp((int)damage - (int)DefaultDefense / 2, 0, (int)Hp);
                string battleText = $"{creature.Name}에게 {finalDamage}의 데미지를 입혔습니다!";
                if (isCritical) battleText = "치명타 발생! " + battleText;
                Renderer.Print(line++, battleText, false, textdelay, printWidthPos);
                creature.OnDamaged(finalDamage);
            }
        }

        public void Skill(Creature creature, ref int line, float damage)
        {
            if (creature.IsDead())
                return;
            int printWidthPos = Console.WindowWidth / 2;

            //for (int i = line; i <= 11; i++)
            //{
            //    //Renderer.Print(i, new string(' ', printWidthPos - 1), false, 0, printWidthPos);
            //    Renderer.ClearLine(i, printWidthPos - 3, printWidthPos);
            //}

            int finalDamage = Math.Clamp((int)damage - (int)DefaultDefense / 2, 0, (int)Hp);

            string battleText = $"{creature.Name}에게 {finalDamage}의 데미지를 입혔습니다!";
            Renderer.Print(line++, battleText, false, textdelay, printWidthPos);
            creature.OnDamaged(finalDamage);

        }

        public override void OnDamaged(int damage)
        {
            Hp -= damage;
            Managers.Game.SaveGame();
        }

        public void ChangeMana(int value)
        {
            Mp += value;
            Managers.Game.SaveGame();
        }

        //확률에 따라 발생하는 메서드
        private bool RandomChance(float probability)
        {
            Random random = new Random();
            float randomValue = (float)random.NextDouble(); // 0.0에서 1.0 사이의 난수

            return randomValue < probability;
        }

        public override bool IsDead()
        {
            if (hp <= 0) return true;
            return false;
        }

        public void ChangeGold(int gold)
        {
            Gold += gold;
            Managers.Game.SaveGame();
        }

        public void ChangeExp(int expAmount)
        {
            int levelsToAdvance = 0;
            TotalExp += expAmount;
            while (TotalExp >= NextLevelExp)
            {
                TotalExp -= NextLevelExp;
                levelsToAdvance++;
                NextLevelExp += 50;
            }
            LevelUp(levelsToAdvance);
            Managers.Game.SaveGame();
        }

        public void LevelUp(int levelsToAdvance)
        {
            if (levelsToAdvance == 0)
                return;
            Level += levelsToAdvance;
            // 임시 -> 레벨업당 공1, 방어0.5 증가
            DefaultDamage += 1.0f * levelsToAdvance;
            DefaultDefense += 0.5f * levelsToAdvance;

            // 출력
            Renderer.Print(Console.WindowHeight - 7, $"레벨 {Level - levelsToAdvance} -> {Level}");
            Renderer.Print(Console.WindowHeight - 6, $"공격력 {Damage - 1.0f * levelsToAdvance} -> {Damage}");
            Renderer.Print(Console.WindowHeight - 5, $"방어력 {Defense - 0.5f * levelsToAdvance} -> {Defense}");
            Managers.Game.SaveGame();
        }

    }
}

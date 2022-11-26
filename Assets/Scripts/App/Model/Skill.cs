using System;
using TandC.RunIfYouWantToLive.Common;


namespace TandC.RunIfYouWantToLive
{
    public class Skill
    {
        public int SkillID { get; private set; }
        public int CurrentLevel { get; private set; }
        public SkillsData SkillData { get; private set; }
        public int MaxLevel { get; set; }
        public Action<Skill> SkillActionEvent;
        public Enumerators.SkillType SkillType { get; private set; }
        public Enumerators.SkillUseType SkillUseType { get; private set; }
        public float Value { get; private set; }
        public Skill(SkillsData data)
        {
            SkillData = data;
            CurrentLevel = 0;
            Value = data.Value;
            MaxLevel = data.MaxLevel;
            SkillType = data.type;
            SkillUseType = data.useType;
        }
        private void LevelUp() 
        {
            CurrentLevel++;
        }
        public bool CanUpThisSkill() 
        {
            return CurrentLevel < MaxLevel;
        }

        public void Update() 
        {

        }

        public void Action() 
        {
            LevelUp();
            SkillActionEvent?.Invoke(this);
            SkillUseType = Enumerators.SkillUseType.Additional;
        }
    }
}
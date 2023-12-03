using UnityEngine;
using System;
using TandC.RunIfYouWantToLive.Common;

namespace TandC.RunIfYouWantToLive
{
    [CreateAssetMenu(fileName = "GameplayData", menuName = "TandC/Game/GameplayData", order = 3)]
    public class GameplayData : ScriptableObject
    {
        public Enemies[] enemies;
        public PlayerWeaponData[] weaponData;
        public EnemyWeaponData[] enemyWeaponData;
        public PlayerData playerData;
        public PlayerBulletData[] bulletData;
        public EnemyBulletData[] enemyBulletData;
        public ItemData[] itemDatas;
        public SkillsData[] skillData;
        public MiniBoss[] miniBosses;
        public Enumerators.SkillType[] StartenSkills;
        public Enumerators.SkillType[] StartenSkills2;
        public DropChanceData DropChance;
        public BonusTypes[] bonusTypes;
        public Phase[] gamePhases;
        public Materials[] materials;

        public BonusTypes GetBonusByType(Enumerators.BonusType type)
        {
            foreach (var item in bonusTypes)
            {
                if (item.type == type)
                    return item;
            }

            return null;
        }

        public Materials GetMaterialByType(Enumerators.MaterialTypes type)
        {
            foreach (var item in materials)
            {
                if (item.type == type)
                    return item;
            }

            return null;
        }

        public SkillsData GetSkillByType(Enumerators.SkillType skillType)
        {
            foreach (var item in skillData)
            {
                if (item.type == skillType)
                    return item;
            }

            return null;
        }
        public Enemies GetEnemiesByType(Enumerators.EnemyType type) 
        {
            foreach (var item in enemies)
            {
                if (item.type == type)
                    return item;
            }

            return null;
        }
        public MiniBoss GetMiniBossByPhaseId(int phaseId) 
        {
            foreach(var item in miniBosses) 
            {
                foreach(var bossPhases in item.PhaseID) 
                {
                    if(phaseId == bossPhases) 
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        public Phase GetPhaseById(int id) 
        {
            foreach (var item in gamePhases)
            {
                if (item.PhaseId == id)
                    return item;
            }

            return null;
        }
        public PlayerWeaponData GetWeaponByType(Enumerators.WeaponType weaponType)
        {
            foreach (var item in weaponData)
            {
                if (item.type == weaponType)
                    return item;
            }

            return null;
        }

        public EnemyWeaponData GetEnemyWeaponByType(Enumerators.EnemyType enemyType)
        {
            foreach (var item in enemyWeaponData)
            {
                if (item.type == enemyType)
                    return item;
            }

            return null;
        }

        public PlayerBulletData GetBulletByType(Enumerators.WeaponType weaponType)
        {
            foreach (var item in bulletData)
            {
                if (item.type == weaponType)
                    return item;
            }

            return null;
        }

        public EnemyBulletData GetEnemyBulletByType(Enumerators.EnemyType enemyType)
        {
            foreach (var item in enemyBulletData)
            {
                if (item.type == enemyType)
                    return item;
            }

            return null;
        }

        public SkillsData GetSkillByID(int id)
        {
            foreach (var item in skillData)
            {
                if (item.id == id)
                    return item;
            }

            return null;
        }

        public ItemData GetItemDataByType(Enumerators.ItemType itemType) 
        {
            foreach (var item in itemDatas)
            {
                if (item.type == itemType)
                    return item;
            }

            return null;
        }
    }

    [Serializable]
    public class BonusTypes
    {
        public Enumerators.BonusType type;
        public Sprite sprite;
    }


    [Serializable]
    public class PlayerData
    {
        public int rotateSpeed;
        public int startedLevel;
        public int startNeedXp;
        public float StartDashTimeRecover;
        public int StartDashDamage;
        public float StartMaskRecoverTime;
        public float StartMaskActiveTime;
        public float StartRocketRecoverTime;
        public int StartRocketCount;
        public int StartRocketDamage;
        public int StartHealthCountRestoreByTime;
        public float StartHealthRestoreTime;
        public float StartLaserShotSize;
        public float StartLaserShotTime;
        public int BombDamage;
        public float StartRocketBlowSize;
        public float StartDashTime;
        public float StartDodgePower;
        public float StartDodgeRecoverTimer;
        public DroneData DroneData;
        public GameObject playerPrefab;
        public Enumerators.WeaponType StartWeaponType;
    }
    [Serializable]
    public class DropChanceData 
    {
        public int StandartShotChance;
        public int LaserShotChance;
        public int RocketBlowChance;
        public int BombBlowChance;
        public int DashChance;
        public int DronChance;
    }
    [Serializable]
    public class DroneData 
    {
        public GameObject prefab;
        public float StartDroneSpeed;
        public int StartDroneDamage;
    }
    public class BulletData
    {
        public int BulletSpeed;
        public float bulletLifeTime;
        public int bulletLife;
        public GameObject ButlletObject;
    }
    [Serializable]
    public class PlayerBulletData : BulletData
    {
        public Enumerators.WeaponType type;
        public bool IsNotCustomise;
        public Enumerators.CustomisationType CustomisationType;
    }

    [Serializable]
    public class EnemyBulletData : BulletData
    {
        public Enumerators.EnemyType type;
    }

    [Serializable]
    public class MiniBoss
    {
        public Enemies enemyData;
        public int[] PhaseID;
    }
    [Serializable]
    public class Phase
    {
        public int PhaseId;
        public float spawnTime;
        public EnemySpawnData[] enemyInPhase;
       // public int[] EnemyCount;
        public float timeBeforePhase;
        public bool IsRandomEnemySpawn;
    }
    [Serializable]
    public class EnemySpawnData 
    {
        public Enumerators.EnemyType enemyType;
        public Enumerators.SpawnType spawnType;
        public int EnemyCount;
    }

    [Serializable]
    public class Enemies
    {
        public int enemyId;
        public float health;
        public int damage;
        public int experience;
        public float movementSpeed;
        public Enumerators.EnemyMovementType movementType;
        public Enumerators.EnemyActionType actionType;
        public Enumerators.EnemyType type;
        public GameObject enemyPrefab;
        public float lifeTime;
    }

    [Serializable]
    public class Materials
    {
        public Enumerators.MaterialTypes type;
        public Material material;
    }

    [Serializable]
    public class ItemData
    {
        public int itemId;
        public int itemValueMin;
        public int itemValueMax;
        public float weight;
        public GameObject prefab;
        public Enumerators.ItemType type;
        public bool isForBoss;
        [TextArea(5, 10)]
        public string description;
    }

    public class WeaponData
    {
        public string weaponName;
        public float baseDamage;
        public float shootDeley;
    }

    [Serializable]
    public class PlayerWeaponData : WeaponData
    {
        public Enumerators.WeaponType type;
    }

    [Serializable]
    public class EnemyWeaponData : WeaponData
    {
        public Enumerators.EnemyType type;
    }

    [Serializable]
    public class SkillsData
    {
        public uint id;
        public string name;
        public string nameForDev;
        public Sprite sprite;
        public int MaxLevel;
        public Skill Skill;
        public float Value = 0;
        public Enumerators.SkillType type;
        public Enumerators.SkillUseType useType;
        public string description;
        public bool isIncrease;

        public bool isProcent;
        public float procentIncrease;
    }
}

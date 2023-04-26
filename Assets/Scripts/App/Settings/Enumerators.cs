namespace TandC.RunIfYouWantToLive.Common
{
    public class Enumerators
    {
        public enum AppState
        {
            NONE,
            APP_INIT_LOADING,
            MAIN_MENU,
        }

        public enum ButtonState
        {
            ACTIVE,
            DEFAULT
        }

        public enum BonusType
        {
            Damage,
            Freeze,
            BlowUp
        }

        public enum ItemType 
        {
            SmallXp,
            MeduimXp,
            BigXp,
            Ammo,
            Medecine,
            FrozenBomb,
            RocketBox,
            Chest,
            Bomb,
            Magnet,
            SmallMoney
        }

        public enum ActiveButtonType 
        {
            MaskButton,
            LaserButton,
            RocketButton,
            DashButton,
        }

        public enum SceneType
        {
            MAIN_MENU,
        }

        public enum SoundType : int
        {
            //  CLICK,
            //  OTHER,
            //   BACKGROUND,
        }

        public enum NotificationType
        {
            LOG,
            ERROR,
            WARNING,

            MESSAGE
        }

        public enum SpreadsheetDataType
        {
            Localization
        }

        public enum Language
        {
            Unknown,

            English,
            Russian,
            Ukrainian
        }

        public enum ScreenOrientationMode
        {
            PORTRAIT,
            LANDSCAPE
        }

        public enum CacheDataType
        {
            USER_LOCAL_DATA,
            USER_RECORDS_DATA
        }

        public enum NotificationButtonState
        {
            ACTIVE,
            INACTIVE
        }

        public enum UpgradeType 
        {
            Health,
            Armor,
            Speed,
            DamageMultiplier,
            CriticalChance,
            CriticalDamageMultiplier,
            RecoverTimerMultiplier,
            MoneyMultiplier,
            PickUpRadius,
            StarterSkill,
        }

        public enum CustomisationType
        {
            Player = 0,
            DefaultBullet = 1,
            AutoBullet = 2,
            MiniGunBullet = 3,
            RocketBullet = 4,
            Drones = 5,
        }

        public enum EnemyMovementType
        {
            Undefined,
            DefaultMove,
            SawMove,
            DistanceMove,
            MoveInPoint,
        }
        public enum EnemyActionType
        {
            Undefined,
            DefaultAction,
            WeaponAction,
        }
        public enum SpawnType 
        {
            Random,
            Circle,
            SpawnFrontPlayer,
            EnemySpawnPosition_0,
            EnemySpawnPosition_1,
            EnemySpawnPosition_2,
            EnemySpawnPosition_3,
            EnemySpawnPosition_4,
            EnemySpawnPosition_5,
            EnemySpawnPosition_6,
            EnemySpawnPosition_7,
            EnemySpawnPosition_8,
            EnemySpawnPosition_9,
            EnemySpawnPosition_10,
            EnemySpawnPosition_11,
            UpperPosition,
            DownPosition,
            LeftPosition,
            RightPosition,

        }

        public enum MaterialTypes
        {
            DefaultEnemyMaterial,
            FlashEnemyMaterial
        }

        public enum EnemyType 
        {
            StandartSquare,
            StandartPentagon,
            Star,
            Mine,
            Saw,
            PiciesFull,
            PiciesHalf,
            PiciesSmall,
            MiniBoss,
            SmallSquare,
            ShootingRoundedBall,
            Impulse,
            ImpulseSaw,
        }
        public enum WeaponType
        {
            Undefined,
            Standart,
            RocketLauncer,
            AutoGun,
            LaserGun,
            Minigun,
            EnergyGun,
            LightningGun,
        }

        public enum SkillUseType 
        {
            Active,
            Passive,
            Additional,
        }

        public enum SkillType
        {
            MaxHealthIncrease,
            MovementSpeedIncrease,

            Shield,
            ShieldRecoverTime,
            ShieldHealthIncrease,

            Armor,

            ShotAfterShot,
            DoubleShot,

            BlowMina,
            BlowMinaDamage,

            BulletSpeed,
            RecoverTimerDecrease,
            DamageIncrease,

            Drone,
            IncreaseDroneSpeed,

            CriticalChanceIncrease,
            CriticalDamageMultilpier,

            AutoGun,
            AutoGunShotCount,

            Dash,
            DashDistanceIncrease,

            Mask,
            MaskActiveTimeIncrease,

            Rocket,
            RocketMaxCountUpgrade,
            RocketExplosionSizeIncrese,

            HealthRestore,
            HealthRestoreTimeDecrease,
            HealthRestoreCountIncrese,

            XpMultiplierIncrese,

            LaserGun,
            LaserGunSizeIncrese,

            Minigun,
            MinigunShotIncrease,
            MinigunSizeIncrease,
            MinigunSizeDecrease,

            EnergyGun,
            EnergyGunShotCount,
            EnergyGunFasterHit,

            LightningGun,
            LightningGunChainCount,

            PickUpRadiusIncrease,
        }
    }
}
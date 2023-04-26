using System;
using System.Collections.Generic;
using TandC.RunIfYouWantToLive.Common;

namespace TandC.RunIfYouWantToLive
{
    public class UserLocalData
    {
        public Enumerators.Language appLanguage;
        public bool IstutorialComplete;
        public int MoneyCount;
        public bool IsStaticJoyStick;
        public float MusicValue;
        public float SoundValue;

        public List<AviableProducts> AviableCustomization;

        public Dictionary<Enumerators.UpgradeType, int> PlayerCharacteristicsData;

        public UserLocalData()
        {
            Reset();
        }

        public void Reset()
        {
            appLanguage = Enumerators.Language.Unknown;
            IstutorialComplete = false;
            MoneyCount = 0;
            IsStaticJoyStick = false;
            MusicValue = 0.5f;
            SoundValue = 0.5f;

            AviableCustomization = new List<AviableProducts>();

            PlayerCharacteristicsData = new Dictionary<Enumerators.UpgradeType, int>();
        }

        public void InitCustomisation()
        {

            if(AviableCustomization.Count == 0) 
            {
                foreach (var type in Enum.GetValues(typeof(Enumerators.CustomisationType)))
                {
                    var item = new AviableProducts((int)(Enumerators.CustomisationType)type);
                    AviableCustomization.Add(item);

                    if (item.AviableId.Count == 0)
                    {
                        item.FirstCreate();
                    }
                }
            }
            else 
            {
                foreach (var type in Enum.GetValues(typeof(Enumerators.CustomisationType)))
                {
                    bool skip = false;
                    if (AviableCustomization.Count != 0)
                    {
                        foreach (var customisation in AviableCustomization)
                        {
                            if (customisation.CustomisationType == (Enumerators.CustomisationType)type)
                            {
                                skip = true;
                            }
                        }
                    }
                    if (skip)
                    {
                        continue;
                    }

                    var item = new AviableProducts((int)(Enumerators.CustomisationType)type);
                    AviableCustomization.Add(item);

                    if (item.AviableId.Count == 0)
                    {
                        item.FirstCreate();
                    }
                }

            }
            
        }

        public AviableProducts GetProducts(Enumerators.CustomisationType type) 
        {
            foreach (var item in AviableCustomization)
            {
                if (item.CustomisationType == type)
                {
                    return item;
                }
            }
            return AviableCustomization[0];
        }

        public int GetSelectedProduct(Enumerators.CustomisationType type) 
        {
            foreach(var item in AviableCustomization) 
            {
                if(item.CustomisationType == type) 
                {
                    return item.SelectedId;
                }
            }
            return 0;
        }

        public void SetCharecteristicData() 
        {

            foreach (var type in Enum.GetValues(typeof(Enumerators.UpgradeType)))
            {
                PlayerCharacteristicsData.Add((Enumerators.UpgradeType)type, 0);
            }
        }
        public class AviableProducts 
        {
            public int Id { get; private set; }
            public int SelectedId { get; set; }
            public List<int> AviableId { get; private set; }
            public Enumerators.CustomisationType CustomisationType { get; private set; }

            public AviableProducts(int id) 
            {
                Id = id;
                SelectedId = 0;
                AviableId = new List<int>();
                CustomisationType = (Enumerators.CustomisationType)Id;
            }
            public void FirstCreate() 
            {
                AviableId.Add(0);
            }
        }
    }
}
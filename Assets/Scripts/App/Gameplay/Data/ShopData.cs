using UnityEngine;
using System;
using TandC.RunIfYouWantToLive.Common;
using System.Collections.Generic;

namespace TandC.RunIfYouWantToLive
{
    [CreateAssetMenu(fileName = "GameplayData", menuName = "TandC/Game/ShopData", order = 3)]
    public class ShopData : ScriptableObject
    {
        public UpgradeData[] UpgradeDataList;
        public List<ShopProductsData> ShopDataList;

        public ShopProductsData GetProductsByType(Enumerators.CustomisationType type)
        {
            foreach (var item in ShopDataList)
            {
                if (item.CustomisationType == type)
                    return item;
            }

            return ShopDataList[0];
        }

        public UpgradeData GetUpgradeByType(Enumerators.UpgradeType type)
        {
            foreach (var item in UpgradeDataList)
            {
                if (item.UpgradeType == type)
                    return item;
            }

            return null;
        }

        public float GetUpgradeValueByType(Enumerators.UpgradeType type)
        {
            foreach (var item in UpgradeDataList)
            {
                if (item.UpgradeType == type)
                    return item.GetUpgradeValue();
            }
            return 0f;
        }

        public int GetSumOfUpgradeLevel() 
        {
            int sum = 0;

            foreach (var item in UpgradeDataList)
            {
                sum += item.CurrentLevel;
            }
            return sum;
        }

        

        [Serializable]
        public class UpgradeData
        {
            public List<float> UpgradeList;
            public Sprite UpgradeSprite;
            public string Title;
            public string DescriptionKey;
            [TextArea(5, 10)]
            public string DescriptionForDev;
            public List<int> Cost;
            public Enumerators.UpgradeType UpgradeType;
            public bool IsProcent;

            public int CurrentLevel;

            public float GetUpgradeValue() 
            {
                return UpgradeList[CurrentLevel];
            }

            public int GetCostValue()
            {
                return Cost[CurrentLevel+1];
            }

            public float GetProcentFromUpgrade()
            {
                float procent = 0;

                float startValue = UpgradeList[0];
                if(startValue == 0) 
                {
                    startValue = 1;
                }
                float maxValue = 0;
                if (CurrentLevel+1 >= UpgradeList.Count)
                {
                    maxValue = UpgradeList[UpgradeList.Count - 1];
                }
                else 
                {
                    maxValue = UpgradeList[CurrentLevel+1];
                }
                procent = ((maxValue / startValue) - 1) * 100;

                if(procent < 0) 
                {
                    procent *= -1;
                }
                if (IsProcent) 
                {
                    procent = GetUpgradeValue();
                }
                return procent;
            }
        }
        [Serializable]
        public class ProductData 
        {
            public int Id;
            public Sprite Sprite;
            public int CostValue;
            public bool IsHideForShop;
            public GameObject Prefab;
        }

        [Serializable]
        public class ShopProductsData
        {
            public Enumerators.CustomisationType CustomisationType;
            public ProductData[] Products;

            public ProductData GetProductById(int id)
            {
                foreach (var item in Products)
                {
                    if (item.Id == id)
                        return item;
                }

                return Products[0];
            }
        }
    }
}

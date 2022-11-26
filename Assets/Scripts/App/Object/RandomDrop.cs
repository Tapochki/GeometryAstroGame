using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TandC.RunIfYouWantToLive.Common.Enumerators;

namespace TandC.RunIfYouWantToLive
{
    public class RandomDrop
    {
        private List<ItemData> _itemsDrop;

        private List<ItemData> _bossDrop;

        private List<ChestRandomItem> _randomChest = new List<ChestRandomItem>() 
        { 
            new ChestRandomItem() { id = 1, weight = 100f},
            new ChestRandomItem() { id = 2, weight = 75f},
            new ChestRandomItem() { id = 3, weight = 40f},
            new ChestRandomItem() { id = 4, weight = 15f},
            new ChestRandomItem() { id = 5, weight = 5f}
        };

        private float _totalWeithChest;

        private float _totalWeight;

        private float _totalBoos;

        public RandomDrop(GameplayData data)
        {
            _itemsDrop = new List<ItemData>();

            _bossDrop = new List<ItemData>();
            foreach (var item in data.itemDatas) 
            {
                if (item.isForBoss) 
                {
                    _bossDrop.Add(item);
                }
                else 
                {
                    _itemsDrop.Add(item);
                }
            }
            GetTotalWeight();

        }

        private void GetTotalWeight()
        {
            foreach (var item in _itemsDrop)
            {
                _totalWeight += item.weight;
            }
            foreach (var item in _bossDrop)
            {
                _totalBoos += item.weight;
            }
            foreach (var item in _randomChest)
            {
                _totalWeithChest += item.weight;
            }
        }

        public int GetChestDrop()
        {
            float roll = UnityEngine.Random.Range(0f, _totalWeithChest);

            foreach (var item in _randomChest)
            {
                if (item.weight >= roll)
                    return item.id;

                roll -= item.weight;
            }

            throw new System.Exception("Reward generation exaption!");
        }

        public ItemData GetBossDrop()
        {
            float roll = UnityEngine.Random.Range(0f, _totalBoos);

            foreach (var item in _bossDrop)
            {
                if (item.weight >= roll)
                    return item;

                roll -= item.weight;
            }

            throw new System.Exception("Reward generation exaption!");
        }

        public ItemData GetDrop()
        {
            float roll = UnityEngine.Random.Range(0f, _totalWeight);
            foreach (var item in _itemsDrop)
            {
                if (item.weight >= roll) 
                {
                    return item;
                }


                roll -= item.weight;
            }

            throw new System.Exception("Reward generation exaption!");
        }

        private class ChestRandomItem 
        {
            public int id;
            public float weight;
        }
    }
}
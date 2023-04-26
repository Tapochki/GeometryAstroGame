using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TandC.RunIfYouWantToLive.Helpers
{
    public static class InternalTools
    {
        private static string LINE_BREAK = "%line%";
        public static void ShuffleList<T>(this IList<T> list)
        {
            System.Random rnd = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static int GetRandomIndexer() 
        {
            int[] indexes = new int[] {-1, 1};

            return indexes[UnityEngine.Random.Range(0, indexes.Length+1)];
        }

        public static float GetRandomNumberFloat(float firstNumber, float secondNumber) 
        {
            return UnityEngine.Random.Range(firstNumber, secondNumber + 1);
        }

        public static int GetRandomNumberInteger(int firstNumber, int secondNumber)
        {
            return UnityEngine.Random.Range(firstNumber, secondNumber + 1);
        }

        public static string ReplaceLineBreaks(string data)
        {
            if (data == null)
                return "";

            return data.Replace(LINE_BREAK, "\n");
        }

        public static float GetFloatSquaredNumber(float num, int n)
        {
            float num_n = 1;
            for (int i = 0; i < n; i++)
            {
                num_n *= num;
            }
            return num_n;
        }

        /// <summary>
        /// Multiplier must be 1.07 - 1.15
        /// </summary>
        public static float GetIncrementalFloatValue(float basicValue, float multiplier, int ownedCount)
        {
            return basicValue * GetFloatSquaredNumber(multiplier, ownedCount);
        }

        public static int GetRandomListIndex<T>(this IList<T> list) 
        {
            return UnityEngine.Random.Range(0, list.Count);
        }

        public static T GetRandomListElement<T>(this IList<T> list)
        {
            var listelement = list[GetRandomListIndex(list)];
            return listelement;
        }

        public static List<T> GetRandomElements<T>(this IList<T> list, int count)
        {
            List<T> shuffledList = new List<T>(count);
            shuffledList.AddRange(list);

            if (list.Count <= count)
                return shuffledList;

            ShuffleList(shuffledList);
            return shuffledList.GetRange(0, count);
        }

        public static T GetEnumFromString<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
        public static void FixVerticalLayoutGroupFitting(UnityEngine.Object value)
        {
            VerticalLayoutGroup group = null;

            if (value is VerticalLayoutGroup)
                group = value as VerticalLayoutGroup;
            else if (value is GameObject)
                group = (value as GameObject).GetComponent<VerticalLayoutGroup>();
            else if (value is Transform)
                group = (value as Transform).GetComponent<VerticalLayoutGroup>();


            if (group == null)
                return;

            group.enabled = false;
            Canvas.ForceUpdateCanvases();
            group.SetLayoutVertical();
            group.CalculateLayoutInputVertical();
            group.enabled = true;
        }

        public static Rect GetScreenCoordinates(RectTransform uiElement, GameObject canvas)
        {
            RectTransform canvasTransf = canvas.GetComponent<RectTransform>();

            Vector2 canvasSize = new Vector2(canvasTransf.rect.width, canvasTransf.rect.height);
            float koefX = Screen.width / canvasSize.x;
            float koefY = Screen.height / canvasSize.y;
            Vector2 position = Vector2.Scale(uiElement.anchorMin, canvasSize);
            float directionX = uiElement.pivot.x * -1;
            float directionY = uiElement.pivot.y * -1;

            var result = new Rect();
            result.width = uiElement.sizeDelta.x * koefX;
            result.height = uiElement.sizeDelta.y * koefX;
            result.x = position.x * koefX + uiElement.anchoredPosition.x * koefX + result.width * directionX;
            result.y = position.y * koefY + uiElement.anchoredPosition.y * koefX + result.height * directionY;
            return result;
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }


        // InternalTools.CallPhoneNumber("+############");
        public static void CallPhoneNumber(string phone)
        {
            Application.OpenURL("tel://" + phone);
        }

        public static List<T> ParseCSV<T>(string data)
        {
            CSVMap myMap = new CSVMap();
            myMap.defineColumns(typeof(T));
            ArrayList itemList = myMap.loadCsvFromString(data);
            return itemList.Cast<T>().ToList();
        }

        public static List<T> ParseCSVFromResources<T>(string path)
        {
            CSVMap myMap = new CSVMap();
            myMap.defineColumns(typeof(T));
            ArrayList itemList = myMap.loadCsvFromFile(path);
            return itemList.Cast<T>().ToList();
        }

        public static void ExportCSV<T>(List<T> genericList, string finalPath)
        {
            var sb = new StringBuilder();
            var header = string.Empty;
            var info = typeof(T).GetFields();

            if (!File.Exists(finalPath))
            {
                var file = File.Create(finalPath);
                file.Close();
                foreach (var prop in info)
                {
                    header += prop.Name + "; ";
                }
                header = header.Substring(0, header.Length - 2);
                sb.AppendLine(header);
                TextWriter sw = new StreamWriter(finalPath, true);
                sw.Write(sb.ToString());
                sw.Close();
            }

            foreach (var obj in genericList)
            {
                sb = new StringBuilder();
                var line = string.Empty;
                foreach (var prop in info)
                {
                    line += prop.GetValue(obj) + "; ";
                }
                line = line.Substring(0, line.Length - 2);
                sb.AppendLine(line);
                TextWriter sw = new StreamWriter(finalPath, true);
                sw.Write(sb.ToString());
                sw.Close();
            }
        }
    }
}
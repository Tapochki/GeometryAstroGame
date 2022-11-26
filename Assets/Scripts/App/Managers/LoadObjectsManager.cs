using System.IO;
using UnityEngine;

namespace TandC.RunIfYouWantToLive
{
    public class LoadObjectsManager : IService, ILoadObjectsManager
    {
        private bool _loadFromResources = true;

        public void Deinit()
        {
          
        }

        public void Dispose()
        {
          
        }

        public void Init()
        {

        }

        public void Update()
        {
            
        }


        public T GetObjectByPath<T>(string path) where T : UnityEngine.Object
        {
            if(_loadFromResources)
                return LoadFromResources<T>(path);
            else
                return LoadFromResources<T>(path); // ToDo change into other load type
        }

        public string GetTextByPath(string path)
        {
            return File.ReadAllText(path);
        }

        public void SetTextByPath(string path, string data)
        {
            File.WriteAllText(path, data);
        }

        private T LoadFromResources<T>(string path) where T : UnityEngine.Object
        {
            return Resources.Load<T>(path);
        }
    }
}
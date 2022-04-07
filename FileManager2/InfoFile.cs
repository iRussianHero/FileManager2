using Newtonsoft.Json;
using System;

namespace FileManager2
{
    public class InfoFile
    {
        public string Method { get; set; }
        public string Name { get; set; }
        public int Length { get; set; }
        public byte[] Data { get; set; }

        public string GetJson()
        {
            if (Name == null || Data.Length == 0 || Length == 0)
            {
                throw new Exception("Файл отсутствует");
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}

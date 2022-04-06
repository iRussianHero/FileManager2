using Newtonsoft.Json;
using System;

namespace TcpServer
{
    public class InfoFile
    {
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

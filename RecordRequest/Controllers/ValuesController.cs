using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace RecordRequest.Controllers
{
    public class ValueController : ApiController
    {
        [HttpPost, Route("api/record")]
        public void Record([FromBody] Data data)
        {
            if (string.IsNullOrEmpty(data.ip)) return;
            var path = ConfigurationManager.AppSettings["FoldPath"];
            var max = int.Parse(ConfigurationManager.AppSettings["Max"]);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var files = new DirectoryInfo(path).GetFiles().OrderBy(x => x.LastWriteTime).ToList();
            var fileName = DateTime.Today.ToString("yy-MM-dd").VerifyFileName(files.Select(x => x.Name.Replace(".txt", "")).ToList());
            var fs = new FileStream($@"{path}\{fileName}.txt", FileMode.Create);
            var sw = new StreamWriter(fs);
            sw.Write(data.ip);
            sw.Flush();
            sw.Close();
            fs.Close();
            while (files.Count() > max)
                files.First().Delete();
        }
    }

    public class Data
    {
        public string ip { get; set; }
    }

    public static class Help
    {
        public static string VerifyFileName(this string name, List<string> files)
        {
            if (files == null || files.All(x => x != name))
                return name;
            for (var i = 1; ; i++)
                if (files.All(x => x != $"{name} ({i.ToString()})"))
                    return $"{name} ({i.ToString()})";
        }
    }
}
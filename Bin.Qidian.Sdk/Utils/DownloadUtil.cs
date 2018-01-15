using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Bin.Qidian.Sdk.Utils
{
    /// <summary>
    /// 下载工具类
    /// </summary>
    public class DownloadUtil
    {
        public const string TITLE_BEGIN = "<h3 class=\"j_chapterName\">";
        public const string TITLE_END = "</h3>";
        public const string CONTENT_BEGIN = "<div class=\"read-content j_readContent\">";
        public const string CONTENT_END = "</div>";
        public const string NEXTCHAPTER_BEGIN = "j_chapterNext\" href=\"";
        public const string NEXTCHAPTER_END = "\"";

        /// <summary>
        /// 获取HTML页面内容
        /// </summary>
        /// <param name="url">HTML URL</param>
        /// <returns></returns>
        public static string GetHtmlContent(string url)
        {
#if DEBUG
            var hch = new HttpClientHandler();
            hch.Proxy = new WebProxy("127.0.0.1", 8888);
            var client = new HttpClient(hch);
#else
            var client = new HttpClient();
#endif
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:50.0) Gecko/20160101 Firefox/53.0");
            var response = client.GetAsync(url);
            return response.Result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 下载章节
        /// </summary>
        /// <param name="url">章节URL</param>
        /// <param name="path">下载目录</param>
        public static void DownloadChapter(string url, string path)
        {
            LogOut($"开始下载HTML... - {url}");
            var html = GetHtmlContent(url);
            if (string.IsNullOrEmpty(html))
                return;

            LogOut("开始解析HTML...");

            string title = null, content;
            var index = html.IndexOf(TITLE_BEGIN);
            if (index > 0)
            {
                var index2 = html.IndexOf(TITLE_END, index);
                if (index2 > 0)
                {
                    title = html.Substring(index + TITLE_BEGIN.Length, index2 - index - TITLE_BEGIN.Length);
                    title = title.Replace("/", "_");
                    title = title.Replace(":", "_");
                    LogOut("解析HTML... - 标题 - 成功");
                }
            }
            else
                index = 0;

            index = html.IndexOf(CONTENT_BEGIN, index);
            if (index > 0)
            {
                var index2 = html.IndexOf(CONTENT_END, index);
                if (index2 > 0)
                {
                    content = ContentProcessing(html.Substring(index + CONTENT_BEGIN.Length, index2 - index - CONTENT_BEGIN.Length));
                    LogOut("解析HTML... - 内容 - 成功");
                    if (string.IsNullOrEmpty(title))
                        title = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + new Random().Next(9999);
                    WriteFile($"{path}\\{title}.txt", content);
                    WriteToOneFile($"{path}\\AllInOne.txt", title, content);
                    LogOut($"写入文件 - {title} - 成功");
                }
            }
            else
                index = 0;

            index = html.IndexOf(NEXTCHAPTER_BEGIN);
            if (index > 0)
            {
                var index2 = html.IndexOf(NEXTCHAPTER_END, index + NEXTCHAPTER_BEGIN.Length);
                if (index2 > 0)
                {
                    content = html.Substring(index + NEXTCHAPTER_BEGIN.Length, index2 - index - NEXTCHAPTER_BEGIN.Length);
                    if (content.Contains("lastpage"))
                        return;
                    LogOut("准备下载下一章 ================");
                    DownloadChapter($"https:{content}", path);//下载下一章
                }
            }            
        }

        public static string ContentProcessing(string content)
        {
            content = content.Trim();
            content = content.Replace("<p>", "\r\n");
            return content;
        }

        public static void WriteFile(string path, string text)
        {
            var file = path;

            using(var fs = new FileStream(file, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(text);
            }
        }

        public static void WriteToOneFile(string path, string title, string text)
        {
            var file = path;

            using (var fs = new FileStream(file, FileMode.Append))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(title);
                sw.Write("\r\n\r\n");
                sw.Write(text);
                sw.Write("\r\n");
            }
        }

        public static void LogOut(string text)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}： {text}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Planes.Tools
{
    public class MD5Tool
    {
        public static string Encrypt(string input)
        {
            return Encrypt(input, new UTF8Encoding());
        }
        public static string Encrypt(string input, Encoding encode)
        {
            System.Security.Cryptography.MD5 mD = new MD5CryptoServiceProvider();
            byte[] array = mD.ComputeHash(encode.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder(32);
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }
    }

    public class FileTool
    {
        public static string Save(HttpPostedFileBase file,string path)
        {
            string fileName = "default.png";
            if (file != null)
            {
                Random rand = new Random();
                fileName = MD5Tool.Encrypt(DateTime.Now.ToString("y-M-d H-m-s.fff") + rand.Next()) + Path.GetExtension(file.FileName);
                file.SaveAs(Path.Combine(HttpContext.Current.Server.MapPath("~/" + path), fileName));
            }
            return path + "/" + fileName;
        }

        public static void Delete(string urlPath)
        {
            var path = HttpContext.Current.Server.MapPath("~/" + urlPath);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void Delete(string[] urls)
        {
            foreach (var s in urls)
                Delete(s);
        }
    }

}
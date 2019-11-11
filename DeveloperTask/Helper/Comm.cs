using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using System.Collections.Specialized;
using System.Web.Security;

namespace Helper {
    public class Comm {
        private static readonly string EncryptionKey = "/@Dyx*.";


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptionString"></param>
        /// <returns></returns>
        public static string Decryption(string encryptionString) {
            if (string.IsNullOrEmpty(encryptionString)) {
                return string.Empty;
            }
            var bytes = Convert.FromBase64String(encryptionString);
            return Encoding.UTF8.GetString(MachineKey.Unprotect(bytes, EncryptionKey));
        }

        /// <summary>
        /// 正则验证手机号码格式
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <returns></returns>
        public static bool ValidateMobile(string mobile) {
            if (string.IsNullOrEmpty(mobile))
                return false;
            return Regex.IsMatch(mobile, @"^(13|14|15|16|18|19|17)\d{9}$");
        }
        /// <summary>
        /// 正则验证邮箱格式
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <returns></returns>
        public static bool ValidateEmail(string email) {
            if (string.IsNullOrEmpty(email))
                return false;
            return Regex.IsMatch(email, "^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
        }

        /// <summary>
        /// 生成随机8位数
        /// </summary>
        /// <returns></returns>
        public string CreateShopID() {
            Random r = new Random();
            string temMsg = string.Empty;
            for (int i = 0; i < 8; i++) {
                temMsg += r.Next(0, 9);
            }
            temMsg = temMsg.PadLeft(8, '1');
            return temMsg.ToString();
        }

        /// <summary>
        /// 后台请求api接口返回json对象
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string HttpPost(string url, object obj) {
            System.Net.Http.HttpContent content = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = new System.Net.Http.HttpClient().PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;
            return response;
        }

        public static string HttpGet(string url) {
            var response = new System.Net.Http.HttpClient().GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            return response;
        }

        public static string GuidTo16String() {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

    }

    public static class MyExtensions {
        /// <summary>
        /// 过滤掉实体类值为空的字段，不更新到数据库
        /// </summary>
        public static void FilterNullable(this System.Data.Entity.Infrastructure.DbEntityEntry entry) {
            FilterFieldValueNull(entry);
        }

        /// <summary>
        /// 过滤掉实体类值为空的字段，不更新到数据库
        /// </summary>
        public static void FilterFieldValueNull(this System.Data.Entity.Infrastructure.DbEntityEntry entry) {
            object entity = entry.Entity;
            entry.State = System.Data.Entity.EntityState.Modified;
            Type t = entity.GetType();
            System.Reflection.PropertyInfo[] fields = t.GetProperties();
            foreach (System.Reflection.PropertyInfo item in fields) {
                if (item.GetValue(entity) == null) {
                    entry.Property(item.Name).IsModified = false;
                    continue;
                }
            }
        }

        /// <summary>
        /// 排除值为空的字段，不更新到数据库
        /// </summary>
        public static void ExcludeFieldValueNull(this System.Data.Entity.Infrastructure.DbEntityEntry entry) {
            object entity = entry.Entity;
            entry.State = System.Data.Entity.EntityState.Modified;
            Type t = entity.GetType();
            System.Reflection.PropertyInfo[] fields = t.GetProperties();
            foreach (System.Reflection.PropertyInfo item in fields) {
                if (item.GetValue(entity) == null) {
                    entry.Property(item.Name).IsModified = false;
                    continue;
                }
            }
        }

        //public static void FilterNullable<T>(this T entity, NameValueCollection collection) {

        //    //Models.Test test = new Models.Test();
        //    //db.Test.Attach(test);
        //    //var entry = db.Entry(test);

        //    Type t = entity.GetType();
        //    System.Reflection.PropertyInfo[] fields = t.GetProperties();
        //    foreach (System.Reflection.PropertyInfo item in fields) {
        //        if (collection.AllKeys.Contains(item.Name)) {
        //            dynamic value = Convert.ChangeType(collection[item.Name], item.PropertyType);
        //            item.SetValue(entity, value);
        //            item.CanWrite
        //            entry.Property(item.Name).IsModified = true;
        //            continue;
        //        }
        //    }

        //}

        /// <summary>
		/// 获取时间差
		/// </summary>
		/// <returns></returns>
        public static string TimeStamp(this DateTime date) {
            TimeSpan ts = date - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static int ToInt32<T>(this T num) {
            try {
                return Convert.ToInt32(num);
            } catch (Exception) {
                return -1;
            }
        }

        public static decimal ToDecimal<T>(this T num) {
            try {
                return Convert.ToDecimal(num);
            } catch (Exception) {
                return 0;
            }
        }

        /// <summary>
        /// 对象转JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T list) where T : new() {
            var res = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            return res;
        }

        /// <summary>
        /// 对象转JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToJsonResult<T>(this T result, string errorMessage = "", string tag = "") where T : new() {
            var res = Newtonsoft.Json.JsonConvert.SerializeObject(new { result = Convert.ToBoolean(result) ? 1 : 0, message = errorMessage, tag = tag });
            return res;
        }

        /// <summary>
		/// 返回MD5加密格式
		/// </summary>
		/// <param name="sourceSting"></param>
		/// <returns></returns>
		public static string GetMd5(this string sourceSting) {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(sourceSting));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        #region  加密 解密
        readonly static string keys = "DA39A3EE5E6B4B0D3255BFEF95601890AFD80709";
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encodes(string data) {
            byte[] byKey = System.Text.Encoding.Default.GetBytes(keys.Substring(5, 8));
            byte[] byIV = System.Text.Encoding.Default.GetBytes(keys.Substring(5, 8));

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            cst.Write(System.Text.Encoding.Default.GetBytes(data), 0, System.Text.Encoding.Default.GetByteCount(data));
            //sw.Write(data);   
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Decodes(string data) {
            //把密钥转成二进制数组
            byte[] byKey = System.Text.Encoding.Default.GetBytes(keys.Substring(5, 8));
            byte[] byIV = System.Text.Encoding.Default.GetBytes(keys.Substring(5, 8));

            byte[] byEnc;
            try {
                //base64解码
                byEnc = Convert.FromBase64String(data);
            } catch {
                return null;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            byte[] tmp = new byte[ms.Length];
            cst.Read(tmp, 0, tmp.Length);
            string result = System.Text.Encoding.Default.GetString(tmp);

            return result.Replace("\0", "");
        }


        #endregion
    }
}
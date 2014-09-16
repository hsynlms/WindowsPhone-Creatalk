using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Creatalk.classes
{
    public class Oauth
    {
        public static string sApiAddress = General.forum_domain + "mobiquo/mobiquo.php";

        public static string ParseXML(string element_name, string result)
        {
            try
            {
                var temp = XElement.Parse(result).Descendants("params").Elements("param").Elements("value").Elements("struct").Elements("member").Where(w => w.Element("name").ToString().Contains(element_name));
                return temp.Elements("value").First().Value;
            }
            catch
            {
                return "";
            }
        }

        public static string ParseXML2(IEnumerable<XElement> path, string element_name)
        {
            try
            {
                var temp = path.Where(w => w.Element("name").ToString().Contains(element_name));
                return temp.Elements("value").First().Value;
            }
            catch
            {
                return "";
            }
        }

        public static string GetParams()
        {
            string genParams = "";

            if (General.general_list != null && General.general_list.Count > 0)
                foreach (var param in General.general_list)
                {
                    if (param == null) continue;

                    if (param.GetType().Equals(typeof(string)))
                        if (param.ToString().IndexOf("<array>") >= 0)
                            genParams += string.Format(@"<param><value>{0}</value></param>", param);
                        else
                            if (param.ToString().IndexOf("base64|") >= 0)
                                genParams += string.Format(@"<param><value><base64>{0}</base64></value></param>", param.ToString().Split('|')[1]);
                            else
                                genParams += string.Format(@"<param><value><string>{0}</string></value></param>", param);
                    else if (param.GetType().Equals(typeof(bool)))
                        genParams += string.Format(@"<param><value><boolean>{0}</boolean></value></param>", (bool)param ? 1 : 0);
                    else if (param.GetType().Equals(typeof(double)))
                        genParams += string.Format(@"<param><value><double>{0}</double></value></param>", param);
                    else if (param.GetType().Equals(typeof(int)) || param.GetType().Equals(typeof(short)))
                        genParams += string.Format(@"<param><value><int>{0}</int></value></param>", param);
                    else if (param.GetType().Equals(typeof(DateTime)))
                        genParams += string.Format(@"<param><value><dateTime.iso8601>{0:yyyy}{0:MM}{0:dd}T{0:hh}:{0:mm}:{0:ss}</dateTime.iso8601></value></param>", param);
                }

            return genParams;
        }

        public static void XmlRpcExecMethod(string methodName, UploadStringCompletedEventHandler completed)
        {
            try
            {
                WebClient req = new WebClient();

                string genParams = GetParams();

                //MessageBox.Show(genParams);

                string command = @"<?xml version=""1.0""?><methodCall><methodName>" + methodName + @"</methodName><params>" + genParams + @"</params></methodCall>";
                byte[] bytes = Encoding.Unicode.GetBytes(command);

                req.Headers[HttpRequestHeader.ContentLength] = bytes.Length.ToString();
                req.Headers[HttpRequestHeader.ContentType] = "text/xml";
                req.Headers[HttpRequestHeader.AcceptEncoding] = "text";
                req.Headers[HttpRequestHeader.Accept] = "*/*";
                req.Headers[HttpRequestHeader.Connection] = "keep-alive";

                if (App.Session_LoadSession("xf_session").Replace(" ", "") != "")  //hdp.
                    req.Headers[HttpRequestHeader.Cookie] = "xf_session=" + App.Session_LoadSession("xf_session");  //hdp.

                req.UploadStringAsync(new Uri(Oauth.sApiAddress, UriKind.Absolute), "POST", command);
                req.UploadStringCompleted += completed;
            }
            catch (Exception ex)
            {
                //BugSenseHandler.Instance.SendExceptionAsync(ex);

                MessageBox.Show("Hata oluştu : exec-method");
            }
        }
    }
}

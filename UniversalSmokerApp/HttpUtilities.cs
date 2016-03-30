using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Net;

// For trace
using System.Diagnostics;

namespace UniversalSmokerApp
{
    class HttpUtilities
    {
        static int buffSize = 1024;
        static int stringBuilderSize = 4096 * 4;

        public static async System.Threading.Tasks.Task<string> HttpGet(string RequestUrl)
        {
            string Doc = null;

            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(RequestUrl);
                req.ContentType = "text/html";
                req.Method = "GET";
                //req.ProtocolVersion = System.Net.HttpVersion.Version10;
                //req.KeepAlive = false;
                //req.ServicePoint.Expect100Continue = false; 

                WebResponse res = await req.GetResponseAsync();
                if (req.HaveResponse && (res.GetResponseStream() != null))
                {
                    // Sometimes the stream reader 'stalls' when we try to tokenize while reading 
                    //  the stream and only returns the partial result, so need to read the entire
                    //  response stream before passing it to the JSON parser
                    //TextReader str = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                    //jDoc = new JSONParser.JSONDoc(str);

                    // read the entire response buffer into a string
                    if (res.ContentLength > 0)
                    {
                        // the easy way, we know how much we need to read
                        byte[] buff;
                        buff = new byte[res.ContentLength];
                        int cnt = 0;
                        while (cnt < (int)res.ContentLength)
                        {
                            int bytesLeft = (int)res.ContentLength - cnt;
                            cnt += res.GetResponseStream().Read(buff, cnt, bytesLeft);
                        }

                        Doc = Encoding.UTF8.GetString(buff);
                    }
                    else
                    {
                        // we didn't get a length, so read the response in chunks and dynamically build the string
                        // the easy way, we know how much we need to read
                        byte[] buff = new byte[buffSize];

                        StringBuilder sb = new StringBuilder(stringBuilderSize);
                        int bytesRead = 1; // trick it first time through
                        while (bytesRead > 0)
                        {
                            bytesRead = res.GetResponseStream().Read(buff, 0, buffSize);
                            if (bytesRead > 0)
                            {
                                sb.Append(Encoding.UTF8.GetString(buff, 0, bytesRead));
                            }
                        }
                        Doc = sb.ToString();
                    }
                }
                //res.Dispose();

                return Doc;
            }
            catch (WebException ex)
            {
                if ((ex.Response != null) && (ex.Response.GetResponseStream() != null))
                {
                    if (ex.Response.ContentLength > 0)
                    {
                        // the easy way, we know how much we need to read
                        byte[] buff;
                        buff = new byte[ex.Response.ContentLength];
                        int cnt = 0;
                        while (cnt < (int)ex.Response.ContentLength)
                        {
                            int bytesLeft = (int)ex.Response.ContentLength - cnt;
                            cnt += ex.Response.GetResponseStream().Read(buff, cnt, bytesLeft);
                        }

                        string str = Encoding.UTF8.GetString(buff);

                        throw new Exception(str, ex);
                    }
                    else
                    {
                        TextReader str = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8);
                        Doc = str.ReadToEnd();
                        ex.Response.Dispose();

                        throw new Exception(Doc.ToString(), ex);
                    }
                }
                //Trace.WriteLine(string.Format("Exception in SwarmServer.HttpPost: {0}", ex.Message));
                //Trace.WriteLine(ex.StackTrace);

                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(string.Format("Exception in SwarmServer.HttpPost: {0}", ex.Message));
                //Trace.WriteLine(ex.StackTrace);

                throw;
            }
        }

        public static async System.Threading.Tasks.Task<string> HttpPost(string RequestUrl, string RequestContent)
        {
            string Doc = null;
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(RequestUrl);

            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";

            try
            {
                if (RequestContent != null)
                {
                    byte[] bytedata = Encoding.UTF8.GetBytes(RequestContent);
                    //req.ContentLength = bytedata.Length;
                    //req.GetRequestStream().Write(bytedata, 0, bytedata.Length);
                    Doc = "this shit was commented out";
                }
                WebResponse res = await req.GetResponseAsync();

                if (req.HaveResponse && (res != null) && (res.GetResponseStream() != null))
                {
                    if (res.ContentLength > 0)
                    {
                        // the easy way, we know how much we need to read
                        byte[] buff;
                        buff = new byte[res.ContentLength];
                        int cnt = 0;
                        while (cnt < (int)res.ContentLength)
                        {
                            int bytesLeft = (int)res.ContentLength - cnt;
                            cnt += res.GetResponseStream().Read(buff, cnt, bytesLeft);
                        }

                        Doc = Encoding.UTF8.GetString(buff);
                    }
                    else
                    {
                        TextReader str = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                        Doc = str.ReadToEnd();
                    }
                }
                res.Dispose();

                return Doc;
            }
            catch (WebException ex)
            {
                if ((ex.Response != null) && (ex.Response.GetResponseStream() != null))
                {
                    if (ex.Response.ContentLength > 0)
                    {
                        // the easy way, we know how much we need to read
                        byte[] buff;
                        buff = new byte[ex.Response.ContentLength];
                        int cnt = 0;
                        while (cnt < (int)ex.Response.ContentLength)
                        {
                            int bytesLeft = (int)ex.Response.ContentLength - cnt;
                            cnt += ex.Response.GetResponseStream().Read(buff, cnt, bytesLeft);
                        }

                        Doc = Encoding.UTF8.GetString(buff);

                        throw new Exception(Doc, ex);
                    }
                    else
                    {
                        TextReader str = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8);
                        Doc = str.ReadToEnd();
                        ex.Response.Dispose();

                        throw new Exception(Doc);
                    }
                }
                //Trace.WriteLine(string.Format("Exception in SwarmServer.HttpPost: {0}", ex.Message));
                //Trace.WriteLine(ex.StackTrace);

                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(string.Format("Exception in SwarmServer.HttpPost: {0}", ex.Message));
                //Trace.WriteLine(ex.StackTrace);

                throw;
            }
        }
    }
}

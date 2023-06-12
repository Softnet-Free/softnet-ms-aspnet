using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RetUrl
/// </summary>
public class UrlBuider
{
    public UrlBuider(string retUrl)
	{
        if (string.IsNullOrEmpty(retUrl))
            m_retUrl = null;
        else
            m_retUrl = retUrl;
	}

    public UrlBuider()
    {
        m_retUrl = null;
    }

    string m_retUrl;

    public string getLoopUrl(string currentUrl)
    {
        if (m_retUrl != null)
            return string.Format("{0}&ret={1}", currentUrl, HttpUtility.UrlEncode(m_retUrl));
        else
            return currentUrl;
    }

    public bool hasBackUrl()
    {
        return m_retUrl != null;
    }

    public string getBackUrl()
    {
        if (m_retUrl == null)
            return null;        
        string[] urlArray = m_retUrl.Split('|');
        if (urlArray.Length == 1)
            return urlArray[0];
        else if (urlArray.Length == 2)
            return string.Format("{0}&ret={1}", urlArray[0], HttpUtility.UrlEncode(urlArray[1]));
        else
        {
            string prevUrl = urlArray[1];
            for (int i = 2; i < urlArray.Length; i++)
                prevUrl = prevUrl + "|" + urlArray[i];
            return string.Format("{0}&ret={1}", urlArray[0], HttpUtility.UrlEncode(prevUrl)); 
        }
    }

    public string getNextUrl(string nextUrl, string currentUrl)
    {
        if (m_retUrl == null)
            return string.Format("{0}&ret={1}", nextUrl, HttpUtility.UrlEncode(currentUrl));
        else
        {
            string[] urlArray = m_retUrl.Split('|');
            List<string> urlList = new List<string>(urlArray.Length);
            foreach(string url in urlArray)
                if (url.Equals(currentUrl) == false)
                    urlList.Add(url);
            if (urlList.Count >= 1)
            {
                string retUrl = currentUrl;
                foreach(string url in urlList)
                    retUrl = retUrl + "|" + url;
                return string.Format("{0}&ret={1}", nextUrl, HttpUtility.UrlEncode(retUrl));
            }
            else
                return string.Format("{0}&ret={1}", nextUrl, HttpUtility.UrlEncode(currentUrl));
        }
    }
}
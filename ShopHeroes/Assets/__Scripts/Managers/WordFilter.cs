using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cfgb
{
    public int id;
    public string forbiden_words;
}
public class WordFilter : SingletonMono<WordFilter>
{
    private List<String> s_filters = new List<string>();

    public void InitCSV()
    {
        var scArray = CSVParser.GetConfigsFromCache<cfgb>("forbidenWords", CSVParser.STRING_SPLIT);
        foreach (var sc in scArray)
        {
            s_filters.Add(sc.forbiden_words);
        }
        //加载文件
        // string forbidencsv = CSVParser.GetCSV("forbidenWords");
        // string[] lines = null;
        // if (forbidencsv.IndexOf(CSVParser.WIN_LINEFEED) >= 0)
        // {
        //     lines = forbidencsv.Split(CSVParser.RNSTR_SPLIT, System.StringSplitOptions.None);
        // }
        // else
        // {
        //     lines = forbidencsv.Split(CSVParser.NLINE_SPLIT, System.StringSplitOptions.None);
        // }
        // s_filters = new string[lines.Length - 3];
        // for (int i = 3; i < lines.Length; i++)
        // {
        //     string[] values = lines[i].Split(',');
        //     if (values.Length > 1)
        //     {
        //         s_filters[i - 3] = values[1];
        //     }
        // }
    }



    /// <summary>
    /// 初始化s_filters之后调用filter函数
    /// </summary>
    /// <param name="content">欲过滤的内容</param>
    /// <param name="result_str">执行过滤之后的内容</param>
    /// <param name="filter_deep">检测深度，即s_filters数组中的每个词中的插入几个字以内会被过滤掉，例：检测深度为2，s_filters中有个词是中国，那么“中国”、“中*国”，“中**国”都会被过滤掉（*是任意字）。</param>
    /// <param name="check_only">是否只检测而不执行过滤操作</param>
    /// <param name="bTrim">过滤之前是否要去掉头尾的空字符</param>
    /// <param name="replace_str">将检测到的敏感字替换成的字符</param>

    public bool filter(string content, out string result_str, int filter_deep = 1, bool check_only = false, bool bTrim = false, string replace_str = "*")
    {
        string result = content;
        if (bTrim)
        {
            result = result.Trim();
        }
        result_str = result;

        if (s_filters == null)
        {
            return false;
        }

        bool check = false;
        foreach (string str in s_filters)
        {
            if (str == null) continue;
            string s = str.Replace(replace_str, "");
            if (s.Length == 0)
            {
                continue;
            }

            bool bFiltered = true;
            while (bFiltered)
            {
                int result_index_start = -1;
                int result_index_end = -1;
                int idx = 0;
                while (idx < s.Length)
                {
                    string one_s = s.Substring(idx, 1);
                    if (one_s == replace_str)
                    {
                        continue;
                    }
                    if (result_index_end + 1 >= result.Length)
                    {
                        bFiltered = false;
                        break;
                    }
                    int new_index = result.IndexOf(one_s, result_index_end + 1, StringComparison.OrdinalIgnoreCase);
                    if (new_index == -1)
                    {
                        bFiltered = false;
                        break;
                    }
                    if (idx > 0 && new_index - result_index_end > filter_deep + 1)
                    {
                        bFiltered = false;
                        break;
                    }
                    result_index_end = new_index;

                    if (result_index_start == -1)
                    {
                        result_index_start = new_index;
                    }
                    idx++;
                }

                if (bFiltered)
                {
                    if (check_only)
                    {
                        return true;
                    }
                    check = true;
                    string result_left = result.Substring(0, result_index_start);
                    for (int i = result_index_start; i <= result_index_end; i++)
                    {
                        result_left += replace_str;
                    }
                    string result_right = result.Substring(result_index_end + 1);
                    result = result_left + result_right;
                }
            }
        }
        result_str = result;
        return check;
    }

}

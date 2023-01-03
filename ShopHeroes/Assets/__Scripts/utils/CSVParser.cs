using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using DictSS = System.Collections.Generic.Dictionary<string, string>;
using System.Diagnostics;
using System.IO;

public static class CSVParser
{
    public readonly static char[] NLINE_SPLIT = new char[] { '\n' };
    public readonly static char[] RNLINE_SPLIT = new char[] { '\r', '\n' };
    public readonly static string[] RNSTR_SPLIT = new string[] { "\r\n" };
    public readonly static string WIN_LINEFEED = "\r\n";
    public readonly static char[] FIELD_SPLIT = new char[] { ","[0] };
    public readonly static char[] STRING_SPLIT = new char[] { ':' };
    public readonly static char[] SEMICOLON_SPLIT = new char[] { ';' };
    readonly static System.Type[] ArraySplitType = new System.Type[1] { typeof(string) };
    static Dictionary<string, string> ConfigCaches;

    public static void SetConfigCaches(DictSS caches)
    {
#if !UNITY_EDITOR
        ConfigCaches = caches;
#endif
    }

    public static void ClearCaches()
    {
        if (ConfigCaches != null)
            ConfigCaches.Clear();
    }
    public static List<T> GetConfigsFromCache<T>(string csvName, char[] splitMark, int skipLine = 3) where T : new()
    {
        string csv = GetCSV(csvName);
        if (csv == null)
        {
            Logger.error("CSV cache not found : " + csvName);
            return null;
        }
        List<T> list = null;
        try
        {
            list = ParseConfigs<T>(csv, splitMark, skipLine);
        }
        catch (System.Exception ex)
        {
            Logger.logException(ex);
        }
        if (list == null)
        {
            Logger.error("CSV parse error : " + csvName);
            return null;
        }
#if !UNITY_EDITOR
        //ConfigCaches.Remove(csvName);
#endif
        return list;
    }

    public static string GetCSV(string csvName)
    {
        string csv = null;
#if UNITY_EDITOR
        csv = FileUtils.loadTxtFile("Assets/Configs/" + csvName + ".csv");
        // csv = CsvCfgCatalogMgr.inst.GetCsvbyName(csvName);
        //csv = AssetCache.GetCSVText(csvName);
#else
        csv = CsvCfgCatalogMgr.inst.GetCsvbyName(csvName);
#endif
        return csv;
    }

    public static List<T> ParseConfigs<T>(string csv, char[] splitMark, int skipLine = 3) where T : new()
    {
        var contentList = DeserializeWithFieldName(csv, skipLine);
        List<T> tlist = DeserializeByType<T>(contentList, splitMark);
        return tlist;
    }

    public static List<string[]> Deserialize(string csv, int skipline = 3)
    {
        string[] lines = csv.Split(NLINE_SPLIT);
        List<string[]> contentList = new List<string[]>(lines.Length - skipline);
        for (int i = skipline; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] fields = line.Split(FIELD_SPLIT);
            contentList.Add(fields);
        }
        return contentList;
    }

    public static List<DictSS> DeserializeWithFieldName(string csv, int skipline = 3)
    {
        string[] lines = null;
        if (csv.IndexOf(WIN_LINEFEED) >= 0)
        {
            lines = csv.Split(RNSTR_SPLIT, System.StringSplitOptions.None);
        }
        else
        {
            lines = csv.Split(NLINE_SPLIT, System.StringSplitOptions.None);
        }

        //List<string[]> contentList = new List<string[]>(lines.Length - skipline);
        List<DictSS> parsedContent = new List<DictSS>();
        var dict = new DictSS(lines.Length - skipline);
        string[] nameList = lines[0].Split(FIELD_SPLIT);
        for (int i = skipline; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrEmpty(line))
                continue;
            string[] fields = line.Split(FIELD_SPLIT);
            var lineDict = new DictSS(nameList.Length);
            var len = (nameList.Length < fields.Length ? nameList.Length : fields.Length);
            for (int j = 0; j < len; j++)
            {
                string n = nameList[j];
                if (string.IsNullOrEmpty(n))
                    continue;
                if (n.Contains("\r"))
                    continue;
                lineDict.Add(nameList[j], fields[j]);
            }
            parsedContent.Add(lineDict);
        }
        //return contentList;
        return parsedContent;
    }

    public static List<T> DeserializeByType<T>(List<DictSS> csvList, char[] splitMark) where T : new()
    {
        System.Type t = typeof(T);
        var fields = t.GetFields();
        List<T> list = new List<T>(csvList.Count);
        foreach (var kv in csvList)
        {
            T val = System.Activator.CreateInstance<T>();
            foreach (var fi in fields)
            {
                var ftype = fi.FieldType;
                string content = null;
                kv.TryGetValue(fi.Name, out content);
                if (content == null)
                {
#if UNITY_EDITOR
                    Logger.warning("DeserializeByType name not found:" + fi.Name);
#endif
                    continue;
                }
                if (string.IsNullOrEmpty(content))
                    continue;
#if UNITY_EDITOR
                try
                {
#endif
                    setValueByType(ftype, val, fi, content, splitMark);

#if UNITY_EDITOR
                }
                catch (System.Exception ex)
                {
                    Logger.error("parse error field:" + fi.Name + "  type:" + ftype.Name + " val:" + content);
                    Logger.logException(ex);
                }
#endif
            }
            list.Add(val);
        }
        return list;
    }

    static void setValueByType(System.Type type, System.Object obj, FieldInfo fi, string content, char[] splitMark)
    {
        if (type == IntType)
        {
            fi.SetValue(obj, int.Parse(content));

        }
        else if (type == UIntType)
        {
            fi.SetValue(obj, uint.Parse(content));

        }
        else if (type == FloatType)
        {
            fi.SetValue(obj, float.Parse(content));
        }
        else if (type == StringType)
        {
            fi.SetValue(obj, content);
        }
        else if (type == UShortType)
        {
            fi.SetValue(obj, ushort.Parse(content));
        }
        else if (type == ByteType)
        {
            fi.SetValue(obj, byte.Parse(content));
        }
        else if (type == BoolType)
        {
            fi.SetValue(obj, "1".Equals(content) ? true : false);
        }
        else if (type == DoubleType)
        {
            fi.SetValue(obj, double.Parse(content));

        }
        else if (type == LongType)
        {
            fi.SetValue(obj, long.Parse(content));
        }
        else if (type.BaseType == EnumType)
        {
            var enumObj = System.Enum.Parse(type, content);
            fi.SetValue(obj, enumObj);
        }
        else if (type == StringArrayType)
        {
            var slist = content.Split(splitMark, System.StringSplitOptions.None);
            fi.SetValue(obj, slist);
        }
        else if (type == IntArrayType)
        {
            parseValueTypeArray<int>(IntType, content, obj, fi, splitMark);
        }
        else if (type == UIntArrayType)
        {
            parseValueTypeArray<uint>(UIntType, content, obj, fi, splitMark);
        }
        else if (type == UShortArrayType)
        {
            parseValueTypeArray<ushort>(UShortType, content, obj, fi, splitMark);
        }
        else if (type == ByteArrayType)
        {
            parseValueTypeArray<byte>(ByteType, content, obj, fi, splitMark);
        }
        else if (type == FloatArrayType)
        {
            parseValueTypeArray<float>(FloatType, content, obj, fi, splitMark);
        }
        else if (type == DoubleArrayType)
        {
            parseValueTypeArray<double>(DoubleType, content, obj, fi, splitMark);
        }
    }

    static System.Object[] ParamsArr = new object[1];
    static void parseValueTypeArray<S>(System.Type fieldType, string content, System.Object obj, System.Reflection.FieldInfo fi, char[] splitMark, bool jumpEmpty = true) where S : struct
    {
        System.Type tp = fieldType;//typeof(S);
        string ct = content;
        if (ct.Equals(string.Empty, System.StringComparison.Ordinal))
            return;
        string[] slist = ct.Split(splitMark, System.StringSplitOptions.None);
        List<S> vlist = new List<S>();
        System.Object[] parr = ParamsArr;//new object[1];
        for (int i = 0; i < slist.Length; i++)
        {
            if (jumpEmpty && string.IsNullOrEmpty(slist[i]))
                continue;
            MethodInfo mi = tp.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, ArraySplitType, null);
            parr[0] = slist[i];
            vlist.Add((S)mi.Invoke(null, parr));
        }
        fi.SetValue(obj, vlist.ToArray());
    }

    public static System.Type IntType = typeof(int);
    public static System.Type UIntType = typeof(uint);
    public static System.Type UShortType = typeof(ushort);
    public static System.Type ByteType = typeof(byte);
    public static System.Type FloatType = typeof(float);
    public static System.Type BoolType = typeof(bool);
    public static System.Type StringType = typeof(string);
    public static System.Type DoubleType = typeof(double);
    public static System.Type EnumType = typeof(System.Enum);
    public static System.Type IntArrayType = typeof(int[]);
    public static System.Type UIntArrayType = typeof(uint[]);
    public static System.Type StringArrayType = typeof(string[]);
    public static System.Type FloatArrayType = typeof(float[]);
    public static System.Type UShortArrayType = typeof(ushort[]);
    public static System.Type ByteArrayType = typeof(byte[]);
    public static System.Type DoubleArrayType = typeof(double[]);
    public static System.Type LongType = typeof(long);

    public static void InitTypes()
    {

    }

    /*public static string[,] Deserialize(string csvText, int skipline = 3) {
        string[] array = csvText.Split(new char[]
        {
            "\n"[0]
        });
        int num = 0;
        for (int i = skipline; i < array.Length; i++) {
            List<string> list = SplitCsvLine(array[i]);
            num = (num <= list.Count ? list.Count : num);//Mathf.Max(num, list.Count);
        }
        string[,] array2 = new string[num + 1, array.Length + 1];
        for (int j = skipline; j < array.Length; j++) {
            List<string> list2 = SplitCsvLine(array[j]);
            for (int k = 0; k < list2.Count; k++) {
                array2[k, j] = list2[k];
                array2[k, j] = array2[k, j].Replace("\"\"", "\"");
            }
        }
        return array2;
    }
    public static List<string> SplitCsvLine(string line) {
        MatchList.Clear();
        MatchCollection matchCollection = Regex.Matches(line, "(((?<x>(?=[,\\r\\n]+))|\"(?<x>([^\"]|\"\")+)\"|(?<x>[^,\\r\\n]+)),?)", RegexOptions.ExplicitCapture);
        for (int i = 0; i < matchCollection.Count; i++) {
            Match match = matchCollection[i];
            MatchList.Add(match.Groups[1].Value);
        }
        return MatchList;
    }*/
}
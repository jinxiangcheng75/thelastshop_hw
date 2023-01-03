using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseConfigManager<TIndex, TItem, TManager> : TSingletonHotfix<TManager>, IConfigManager where TManager : new() where TItem : new()
{
    protected Dictionary<TIndex, TItem> mDict;
    protected List<TItem> mList;
    protected virtual string Csv_name { get; }
    public void InitCSVConfig()
    {
        var scArray = CSVParser.GetConfigsFromCache<TItem>(Csv_name, CSVParser.STRING_SPLIT);
        mList = scArray;
        mDict = new Dictionary<TIndex, TItem>();
        processConfigs(scArray);
    }

    public void ReLoadCSVConfig()
    {
        if (mDict != null)
            mDict.Clear();
        InitCSVConfig();
    }

    protected virtual void processConfigs(List<TItem> list) { }

    public virtual TItem getConfig(TIndex id)
    {
        TItem cfg = default;
        if (!mDict.TryGetValue(id, out cfg))
        {
            Logger.error(Csv_name + " Config not found id:" + id);
        }
        return cfg;
    }

    public virtual IEnumerable<TItem> getList()
    {
        for (int i = 0; i < mList.Count; i++)
        {
            var cfg = mList[i];
            yield return cfg;
        }
    }
}

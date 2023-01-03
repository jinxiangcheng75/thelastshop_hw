using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TimeUtils
{
    //格林尼治起始时间
    public static DateTime TIME_START_UTC = TimeZone.CurrentTimeZone.ToUniversalTime(new DateTime(1970, 1, 1));
    public static DateTime TIME_START = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
    public const int HOUR_SECS = 3600;
    public const int MIN_SECS = 60;
    public const int DAY_SECS = HOUR_SECS * 24;
    public const int FOUR_HOURS = HOUR_SECS * 4;
    public const int SIX_HOURS = HOUR_SECS * 6;
    public const int EIGHT_HOURS = HOUR_SECS * 8;
    public const int SIXTEEN_HOURS = HOUR_SECS * 16;
    public const int TEN_H_SECS = HOUR_SECS * 10;
    public static int ZONE_OFFSET = (int)((DateTime.Now - DateTime.UtcNow).TotalSeconds) / HOUR_SECS;

    //
    public static int GetNowSeconds()
    {
        return (int)((System.DateTime.Now - TIME_START).TotalSeconds);
    }
    //

    
    public static void PrintTimezones()
    {
        string ss = string.Empty;
        var tzis = TimeZoneInfo.GetSystemTimeZones();
        for (int i = 0; i < tzis.Count; i++)
        {
            TimeZoneInfo tzi = tzis[i];
            ss += tzi.DisplayName + "\n";
        }
        Logger.log("PrintTimezones:" + ss);
    }

    public static int GetTodayStartSecs()
    {
        System.DateTime dtNow = System.DateTime.Now;
        int hour = dtNow.Hour;
        int mins = dtNow.Minute;
        int secs = dtNow.Second;
        int nowSecs = (int)(dtNow - TimeUtils.TIME_START).TotalSeconds;
        return nowSecs - hour * HOUR_SECS - mins * 60 - secs;
    }

    public static int GetTodayEndLeftSecs()
    {
        System.DateTime dtNow = System.DateTime.Now;
        int hour = dtNow.Hour;
        int mins = dtNow.Minute;
        int secs = dtNow.Second;
        return DAY_SECS - hour * HOUR_SECS - mins * 60 - secs;
    }

    public static bool checkIsSameDay(int savedTime)
    {
        DateTime dt = DateTime.Now;
        int nowMilli = (int)((dt - TIME_START).TotalSeconds);
        nowMilli -= dt.Hour * HOUR_SECS + dt.Minute * 60 + dt.Second;

        int span = savedTime - nowMilli;
        if (span > 0 && span < DAY_SECS)
        {
            //in same day
            return true;
        }
        return false;
    }

    public static DateTime getDateTimeByTimeZoneOffset(int zoneOffset, double secStamp)
    {
        int zoneSpan = (ZONE_OFFSET - zoneOffset) * HOUR_SECS;
        DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        dt = dt.AddSeconds(secStamp + zoneSpan);
        return dt;
    }

    public static DateTime getDateTimeBySecs(double secs)
    {
        DateTime st = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        st = st.AddSeconds(secs);
        return st;
    }

    public static string getDateTimeStr()
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);
    }

    public static string GetTime_yyyyMMdd_HHmm()
    {
        return System.DateTime.Now.ToString("yyyyMMdd_HHmm", System.Globalization.DateTimeFormatInfo.InvariantInfo);
    }

    public static string GetTime_yyyyMMdd()
    {
        return System.DateTime.Now.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
    }

    public static string GetTime_HHmmss()
    {
        return System.DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// 时间转字符串 xx天xx时  ,, xx时xx分  ,,  xx分xx秒  ,, xx秒
    /// </summary>
    /// <param name="secs">总秒数</param>
    /// <param name="zeroShow">第二时间梯度为0时是否显示</param>
    /// <returns></returns>
    public static string timeSpanStrip(int secs, bool zeroShow = true)
    {
        if (secs >= DAY_SECS)
        {
            int days = secs / DAY_SECS;
            int hours = (secs % DAY_SECS) / HOUR_SECS;

            return days + LanguageManager.inst.GetValueByKey("天") + (zeroShow ? hours + LanguageManager.inst.GetValueByKey("时") : hours == 0 ? "" : hours + LanguageManager.inst.GetValueByKey("时"));
        }
        else if (secs < DAY_SECS && secs >= HOUR_SECS)
        {
            int hours = secs / HOUR_SECS;
            int mins = (secs % HOUR_SECS) / MIN_SECS;

            return hours + LanguageManager.inst.GetValueByKey("时") + (zeroShow ? mins + LanguageManager.inst.GetValueByKey("分") : mins == 0 ? "" : mins + LanguageManager.inst.GetValueByKey("分"));
        }
        else if (secs < HOUR_SECS && secs >= MIN_SECS)
        {
            int mins = secs / MIN_SECS;
            int msecs = secs % MIN_SECS;

            return mins + LanguageManager.inst.GetValueByKey("分") + (zeroShow ? msecs + LanguageManager.inst.GetValueByKey("秒") : msecs == 0 ? "" : msecs + LanguageManager.inst.GetValueByKey("秒"));
        }
        else
        {
            if (secs < 1) secs = 1;
            return secs + LanguageManager.inst.GetValueByKey("秒");
        }
    }


    /// <summary>
    /// 去尾 xx天 ,, xx时 ,, xx分 ,, xx秒
    /// </summary>
    public static string timeTruncate2Str(int secs)
    {
        if (secs >= DAY_SECS)
        {
            return string.Format("{0}", secs / DAY_SECS) + LanguageManager.inst.GetValueByKey("天");
        }
        else if (secs < DAY_SECS && secs >= HOUR_SECS)
        {
            return string.Format("{0}", secs / HOUR_SECS) + LanguageManager.inst.GetValueByKey("时");
        }
        else if (secs < HOUR_SECS && secs >= MIN_SECS)
        {
            return string.Format("{0}", secs / MIN_SECS) + LanguageManager.inst.GetValueByKey("分");
        }
        else
        {
            if (secs < 1) secs = 1;
            return string.Format("{0}", secs) + LanguageManager.inst.GetValueByKey("秒");
        }
    }

    public static string timeSpan2Str(int secs)
    {
        if (secs < DAY_SECS && secs >= HOUR_SECS)
        {
            return string.Format("{0:D2}:{1:D2}", secs / HOUR_SECS, (secs % HOUR_SECS) / MIN_SECS);
        }
        else if (secs < HOUR_SECS && secs >= MIN_SECS)
        {
            return string.Format("{0:D2}:{1:D2}", secs / MIN_SECS, (secs % HOUR_SECS) % MIN_SECS);
        }
        else
        {
            if (secs < 1) secs = 1;
            return string.Format("{0:D2}", secs);
        }

        //return string.Format("{0}:{1}:{2}", secs / HOUR_SECS, (secs % HOUR_SECS) / MIN_SECS, (secs % HOUR_SECS) % MIN_SECS);
    }

    public static string timeSpan3Str(int secs)
    {
        if (secs < 1) secs = 1;
        return string.Format("{0:D2}:{1:D2}:{2:D2}", secs / HOUR_SECS, (secs % HOUR_SECS) / MIN_SECS, (secs % HOUR_SECS) % MIN_SECS);
    }

    public static string timeSpan4Str(int secs)
    {
        if (secs >= DAY_SECS)
        {
            int days = secs / DAY_SECS;
            int hours = (secs % DAY_SECS) / HOUR_SECS;
            int mins = (secs % HOUR_SECS) / MIN_SECS;
            int msecs = secs % MIN_SECS;

            return days + LanguageManager.inst.GetValueByKey("天") + hours + LanguageManager.inst.GetValueByKey("时") + mins + LanguageManager.inst.GetValueByKey("分") + msecs + LanguageManager.inst.GetValueByKey("秒");
        }
        else if (secs < DAY_SECS && secs >= HOUR_SECS)
        {
            int hours = secs / HOUR_SECS;
            int mins = (secs % HOUR_SECS) / MIN_SECS;
            int msecs = secs % MIN_SECS;

            return hours + LanguageManager.inst.GetValueByKey("时") + mins + LanguageManager.inst.GetValueByKey("分") + msecs + LanguageManager.inst.GetValueByKey("秒");
        }
        else if (secs < HOUR_SECS && secs >= MIN_SECS)
        {
            int mins = secs / MIN_SECS;
            int msecs = secs % MIN_SECS;

            return mins + LanguageManager.inst.GetValueByKey("分") + msecs + LanguageManager.inst.GetValueByKey("秒");
        }
        else
        {
            if (secs < 1) secs = 1;
            return secs + LanguageManager.inst.GetValueByKey("秒");
        }
    }

    public static string pasttimeSpanStrip(int secs)
    {
        if (secs >= DAY_SECS)
        {
            return $"{secs / DAY_SECS}" + LanguageManager.inst.GetValueByKey("天前");
        }
        else if (secs >= HOUR_SECS)
        {
            return $"{secs / HOUR_SECS}" + LanguageManager.inst.GetValueByKey("时前");
        }
        else if (secs >= MIN_SECS)
        {
            return $"{secs / MIN_SECS}" + LanguageManager.inst.GetValueByKey("分前");
        }
        else
        {
            if (secs < 1) secs = 1;
            return $"{secs}" + LanguageManager.inst.GetValueByKey("秒前"); ;
        }
    }
    public static string timeSpan(int secs)
    {
        if (secs < MIN_SECS)
        {
            return "1\'";
        }
        else if (secs < HOUR_SECS)
        {
            return string.Format("{0}\'", secs / MIN_SECS);
        }
        else
        {
            return string.Format("{0}\"{1}\'", secs / HOUR_SECS, (secs % HOUR_SECS) / MIN_SECS);
        }
    }

    //时间转角度
    public static float timeSpanAngle(int secs)
    {
        return (1.0f - (float)secs % (DAY_SECS / 2) / (DAY_SECS / 2)) * 360f;
    }
}

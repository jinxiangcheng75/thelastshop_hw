using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmailDataProxy : TSingletonHotfix<EmailDataProxy>, IDataModelProx
{

    Dictionary<int, EmailData> emaildDataDic;

    public bool needShowRedPoint
    {
        get
        {
            return GetAllEmailDatas().FindIndex(t => t.state == (int)EMailStatus.Unread || t.state == (int)EMailStatus.Unclaimed) != -1;
        }
    }


    public int sendFeedbackTimingTime;

    public bool canSendFeedback
    {
        get
        {
            return sendFeedbackTimingTime <= GameTimer.inst.serverNow;
        }
    }

    public void Init()
    {
        emaildDataDic = new Dictionary<int, EmailData>();
    }
    public void UpdateEmailData(OneMail info)
    {
        if (emaildDataDic.ContainsKey(info.mailId))
        {
            emaildDataDic[info.mailId].SetInfo(info);
        }
        else
        {
            emaildDataDic[info.mailId] = new EmailData(info);
        }
    }

    public EmailData GetEmailByID(int id)
    {
        if (emaildDataDic.ContainsKey(id))
        {
            return emaildDataDic[id];
        }

        return null;
    }

    public bool DelEmailById(int id)
    {
        return emaildDataDic.Remove(id);
    }

    public List<EmailData> GetAllEmailDatas()
    {
        var list = emaildDataDic.Values.ToList().FindAll(t => t.state != (int)EMailStatus.Deleted);
        list.Sort((a, b) =>
        {
            if (a.state == (int)EMailStatus.Unread || a.state == (int)EMailStatus.Unclaimed)
            {
                if (b.state == (int)EMailStatus.Unread || b.state == (int)EMailStatus.Unclaimed)
                {
                    return -a.dateTime.CompareTo(b.dateTime);
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (b.state == (int)EMailStatus.Unread || b.state == (int)EMailStatus.Unclaimed)
                {
                    return 1;
                }
                else
                {
                    return -a.dateTime.CompareTo(b.dateTime);
                }
            }
        });

        return list;
    }

    public void Clear()
    {
        if (emaildDataDic != null) emaildDataDic.Clear();
        //emaildDataDic = null;
    }

}

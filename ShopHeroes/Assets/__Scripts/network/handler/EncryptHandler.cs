using UnityEngine;
using System.Collections;
using System.Text;

public interface IEncryptHandler {
    System.Object handleDecryption (kEncryptType type, System.Object data);
    System.Object handleEncryption (kEncryptType type, System.Object data);
}

public class EncryptHandlerFactory : AbstractHandlerFactory<IEncryptHandler, kEncryptType> {
    protected override int getTypeIndex (kEncryptType type) {
        return (int)type;
    }

    protected override int getTypeNum () {
        return (int)kEncryptType.Base64 + 1;
    }
}

public class EncryptHandler : IEncryptHandler{
    
    public System.Object handleDecryption (kEncryptType type, System.Object data) {
        switch(type) {
        case kEncryptType.None:
            return data;
        case kEncryptType.Des:
            return decrypt(data as byte[]);
        case kEncryptType.Base64:
            return decryptBase64Mod(data as string);
        }
        return data;
    }

    string decrypt (string s) {
        return s;
    }

    string decryptBase64Mod (string s) {
        char cc = s[0];
        int num = (int)(cc - 97);
        string ss = s.Substring(1, num) + s.Substring(num + 2);
        byte[] bts = null;
        try {
            bts = System.Convert.FromBase64String(ss);
        } catch(System.Exception e) {
            Debug.LogError(e);
        }
        if(bts == null)
            return null;

        string de = Encoding.UTF8.GetString(bts);
        return de;
    }

    byte[] decrypt(byte[] bytes) {
        return bytes;
    }

    public System.Object handleEncryption (kEncryptType type, System.Object data) {
        switch(type) {
        case kEncryptType.None:
        return data;
        case kEncryptType.Des:
        return encrypt(data as byte[]);
        case kEncryptType.Base64:
        return encryptBase64(data);
        }
        return data;
    }

    string encrypt (string s) {
        return s;
    }

    string encryptBase64 (System.Object data) {
        string enc = string.Empty;
        byte[] bts = null;
        if(data is string) {
            bts = Encoding.UTF8.GetBytes(data as string);
        } else {
            bts = data as byte[];
        }
        enc = System.Convert.ToBase64String(bts);

//        return enc;

        char[] charArr = enc.ToCharArray();

        System.Random rd = new System.Random();
        int num = rd.Next(7) + 1;// 1-7
        StringBuilder sb = new StringBuilder();
        sb.Append((char)(num + 97));
        int idx = 0;
        foreach (char c in charArr)
        {
            ++idx;
            if (idx == num + 1)
            {
                sb.Append((char)(num + 97));
            }
            sb.Append(c);
        }
        return sb.ToString();
    }

    byte[] encrypt (byte[] bytes) {
        return bytes;
    }
}

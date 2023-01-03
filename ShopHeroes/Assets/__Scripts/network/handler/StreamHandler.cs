using System.Collections;
using System.IO;

public class StreamHandlerFactory : AbstractHandlerFactory<IStreamHandler, kStreamType> {
    protected override int getTypeIndex (kStreamType type) {
        return (int)type;
    }

    protected override int getTypeNum () {
        return (int)kStreamType.Binary + 1;
    }
}

public interface IStreamHandler {
    System.Object handle (Stream s);
}

public class StringStreamHandler : IStreamHandler {
    public System.Object handle (Stream s) {
        string content = StatelessHandle(s);
        return content;
    }

    public static string StatelessHandle (Stream s) {
        string content = null;
        using(StreamReader sr = new StreamReader(s)) {
            content = sr.ReadToEnd();
            sr.Close();
        }
        return content;
    }
}

public class BinaryStreamHandler : IStreamHandler {
    public object handle (Stream s) {
        return StatelessHandle(s);
    }

    public static object StatelessHandle(Stream s) {
        int len = (int)s.Length;
        byte[] bts = new byte[len];
        s.Read(bts, 0, len);
        return bts;
    }
}
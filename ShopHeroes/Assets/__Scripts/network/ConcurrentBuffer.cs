using System.IO;
using System.Text;
using System.Threading;
public class ConcurrentBuffer
{
    const int BUFFER_SIZE = 1024 * 512;//512k
    ReaderWriterLockSlim mLock;
    public byte[] mBuffer;
    private int mCursor;

    public ConcurrentBuffer()
    {
        mLock = new ReaderWriterLockSlim();
        mBuffer = new byte[BUFFER_SIZE];
        mCursor = 0;
    }

    public int write(string s, int startIdx, int len)
    {
        int writeLen = 0;
        mLock.EnterWriteLock();
        try
        {
            if (mCursor + len >= BUFFER_SIZE)
            {
                mCursor = 0;//reset to start
            }
            writeLen = Encoding.UTF8.GetBytes(s, startIdx, len, mBuffer, mCursor);
            mCursor += writeLen;
        }
        catch (System.Exception e)
        {
            Logger.log(e.Message);
        }
        finally
        {
            mLock.ExitWriteLock();
        }
        return writeLen;
    }

    public void write(byte[] bytes, int startIdx, int len)
    {
        mLock.EnterWriteLock();
        try
        {

            if (mCursor + len >= BUFFER_SIZE)
            {
                mCursor = 0;//reset to start
            }
            System.Buffer.BlockCopy(bytes, startIdx, mBuffer, mCursor, len);

        }
        catch (System.Exception)
        {

            throw;
        }
        finally
        {
            mLock.ExitWriteLock();
        }
    }

    public void read(Stream s, int len)
    {
        mLock.EnterReadLock();

        if (mCursor + len > BUFFER_SIZE)
        {
            throw new System.Exception("[ConcurrentBuffer] read exceed : " + (mCursor + len));
        }
        try
        {
            s.Write(mBuffer, mCursor, len);
        }
        catch (System.Exception)
        {

            throw;
        }
        finally
        {
            mLock.ExitReadLock();
        }
    }

    void log(string msg)
    {
        Logger.log(msg);
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Guineu.Core
{
    // TODO : NO : useless, use FileInfo instead - set the internal bool to specify fileaccess and attributes
    // TODO (but not by me) : implement the SET CONSOLE for DISPLAY, LIST
    // check the compact version
    // check unicode stuff
    enum fErrorCode
    {
        // 2 File not found
        // 4 Too many files open (out of file handles)
        // 5 Access denied
        // 6 Invalid file handle given
        // 8 Out of memory
        // 25 Seek error (can't seek before the start of a file)
        // 29 Disk full
        // 31 Error opening file

        NoError = 0,
        FilenotFound = 2,
        TooManyFiles = 4,
        AccessDenied = 5,
        InvalidFileHandle = 6,
        OutOfMemory = 8,
        SeekError = 25,
        DiskFull = 29,
        ErrorOpeningFile = 31
    }

    public partial class LowLevelFile : IDisposable
    {
        private LowLevelFileMngr Parent;
        private FileStream m_fs;
        private int m_guineuHandle;
        private string m_fullname = string.Empty;
//        private bool m_Read;
//        private bool m_Write;
//        private bool m_Buffered;
//        private bool m_Shared;
        private string m_apiHandle = string.Empty; // let us see what to do with that thinggie, as it is useless with .net ...

        //--------------------------------------------
        internal void SetError(fErrorCode error)
        {
            if (this.Parent == null)
                return;
            if (error == fErrorCode.NoError)
                this.Parent.Ferror(0);

            this.Parent.Ferror((int)error);

        }
        //--------------------------------------------
        internal LowLevelFile(LowLevelFileMngr parent)
        {
            this.Parent = parent;
        }
        //--------------------------------------------
        private string GetGuineuHandleForStatus()
        {
            return m_guineuHandle.ToString();
        }
        //--------------------------------------------
        internal int GetGuiNeuHandle()
        {
            return m_guineuHandle;
        }
        //--------------------------------------------
        private string GetAPIHandle()
        {
            return m_apiHandle.ToString();
        }
        //--------------------------------------------
        private string GetPath()
        {
            return m_fullname.ToString();
        }
        //--------------------------------------------
        private string GetFileMode()
        {
            string cRetVal = string.Empty;
//            if (fi.Attributes == FileAttributes.)
//                cRetVal += "Buffered : yes\t";
//            else
//                cRetVal += "Buffered : no\t";
            if (m_fs.CanRead)
                cRetVal += "Read : yes\t";
            else
                cRetVal += "Read : no\t";
            if (m_fs.CanWrite)
                cRetVal += "Write : yes";
            else
                cRetVal += "Write : no";
            return cRetVal;
        }

        //------------------------------------------
        internal bool Feof()
        {
            // true if eof
            try
            {
                if (!checkHandle())
                    return true;

                return m_fs.Position >= m_fs.Length;
            }
            catch (ArgumentOutOfRangeException)// aore)
            {
                SetError(fErrorCode.SeekError);
                return true; // lets say that we need to say to user that feof is true...
            }
            catch (Exception)
            {
                SetError(fErrorCode.AccessDenied);
                return true; // lets say that we need to say to user that feof is true...
            }
        }
        //------------------------------------------
        internal void FFlush(bool force)
        {
            //if (force)
            // dunno
            try
            {
                m_fs.Flush();
                SetError(fErrorCode.NoError);
            }
            catch (IOException)// IOe)
            {
                SetError(fErrorCode.AccessDenied);
            }
            catch (ObjectDisposedException)// ode)
            {
                throw new ErrorException(ErrorCodes.InvalidArgument); // in fact an internal error that couldn't happend
            }
        }

        //------------------------------------------
        internal long FchSize(long newSize)
        {
            try
            {
                m_fs.SetLength(newSize);
                SetError(fErrorCode.NoError);
                return m_fs.Length;
            }
            // -1 if error, curSize if ok
            catch (ArgumentOutOfRangeException)// aore)
            {
                SetError(fErrorCode.SeekError);
                return -1;
            }
            catch (Exception)
            {
                SetError(fErrorCode.AccessDenied);
                return -1;
            }
        }

        //------------------------------------------
        internal string Fgets(long nbytes)
        {
            string cRetVal = string.Empty;
            char[] charValue;
            try
            {
                if (!checkHandle())
                    return cRetVal;


                if (m_fs.CanRead)
                {
                	var pos = m_fs.Position;
                    byte[] aBytes = new byte[nbytes];
                    long n = (long)m_fs.Read(aBytes, 0, (int)nbytes);
                    long i;

                    for (i = 0; i < n; i++)
                    {
                        if (aBytes[i] == '\n' || aBytes[i] == '\r')
                        {
                            n = i;
                            break;
                        }
                    }

                	m_fs.Position = pos + n + 1;

                    if (GuineuInstance.UseUnicode)
                    {
                        StringBuilder sb = new StringBuilder();
                        for(i=0;i<n;i++)
                            sb.Append((char)aBytes[i]);
                        cRetVal = sb.ToString();
                    }
                    else
                    {
                        charValue = GuineuInstance.CurrentCp.GetChars(aBytes, 0, (int)i);
                        cRetVal = new string(charValue);
                    }
                }
                else
                {
                    SetError(fErrorCode.AccessDenied);
                }
            }
            catch (Exception)// e)
            {
                SetError(fErrorCode.AccessDenied);
            }
            return cRetVal;

            // needs to be rewritten usiong filestream...
            // read nbytes unless a EOL is read
            // if nbytes==0, read til the EOL or EOF
            /*
        string cRetVal=string.Empty;
        if (m_fs.CanRead)
        {
            StreamReader sr = new StreamReader(m_fs);
            cRetVal = sr.ReadLine();
            //sr.Dispose(); it would close the file !!!

            if (nbytes > 0 && cRetVal.Length > nbytes)
                cRetVal.Substring(0, nbytes);
        }
        return cRetVal;
 */
        }
        //------------------------------------------
        internal string Fread(long nbytes)
        {
            string cRetVal = string.Empty;
            try
            {
                if (!checkHandle())
                    return cRetVal;

                if (m_fs.CanRead)
                {
                    byte[] aBytes = new byte[nbytes];
                    long n = (long)m_fs.Read(aBytes, 0, (int)nbytes);
                    long i;
/* TODO : test the beginning of the file to see if it's a Unicode something :
BOM are :
   EF BB BF UTF-8 
   FF FE UTF-16, little endian 
   FE FF UTF-16, big endian 
   FF FE 00 00 UTF-32, little endian 
   00 00 FE FF UTF-32, big-endian 
*/

                    if (GuineuInstance.UseUnicode)
                    {
                        StringBuilder sb = new StringBuilder();
                        for( i=0;i<n;i++)
                            sb.Append((char)aBytes[i]);
                        cRetVal = sb.ToString();
                    }
                    else
                    {
                        char[] charValue = GuineuInstance.CurrentCp.GetChars(aBytes, 0, (int)n);
                        cRetVal = charValue.ToString();
                    }
                }
                else
                {
                    SetError(fErrorCode.AccessDenied);
                }
            }
            catch (Exception)// e)
            {
                SetError(fErrorCode.AccessDenied);
            }
            return cRetVal;
        }

        //------------------------------------------
        internal long Fwrite(string expression, long carToWrite)
        {
            string cExpr;
            long i;
            try
            {
                long oldPos = m_fs.Position;
                long curPos = oldPos;

                long nCarWritten = carToWrite;

                if (!checkHandle())
                    return 0;

                if (nCarWritten > 0)
                    cExpr = expression.Substring(0, (int)(nCarWritten - 1));
                else
                    cExpr = expression.ToString();

                nCarWritten = cExpr.Length;

                if (m_fs.CanWrite)
                {
                    if (GuineuInstance.UseUnicode)
                    {
                        //Convert the string into an array of bytes -- that's the hard part i can't find anywhere a speeder converter
                        byte[] aBytes = new byte[nCarWritten];
                        for (i = 0; i < nCarWritten; i++)
                        {
                            aBytes[i] = (byte)cExpr[(int)i];
                        }

                        //Call the write method of the FileStream
                        m_fs.Write(aBytes, 0, (int)nCarWritten);
                    }
                    else
                    {
                        byte[] values;
                        values = GuineuInstance.CurrentCp.GetBytes(cExpr);
                        m_fs.Write(values, 0, values.Length);
                    }

                }
                else
                {
                    SetError(fErrorCode.AccessDenied);
                    return 0;
                }

                curPos = m_fs.Position;
                return curPos - oldPos;
            }
            catch (Exception)// e)
            {
                SetError(fErrorCode.AccessDenied);
            }
            return 0;
        }

        //------------------------------------------
        internal long Fseek(long nBytesMoved, int relativePosition)
        {
            try
            {
                SeekOrigin so;
                if (!checkHandle())
                    return 0;

                switch (relativePosition)
                {
                    case 0:
                        so = SeekOrigin.Begin;
                        break;
                    case 1:
                        so = SeekOrigin.Current;
                        break;
                    case 2:
                        so = SeekOrigin.End;
                        break;

                    default:
                        throw new ErrorException(ErrorCodes.InvalidArgument);

                }
                m_fs.Seek(nBytesMoved, so);
                return m_fs.Position;
            }
            catch (Exception)//e)
            {
                SetError(fErrorCode.SeekError);
                return 0;
            }
        }
        //-------------------------------------------
        internal string DisplayStatus()
        {
            return this.GetPath().ToString() + "\t" + this.GetGuineuHandleForStatus().ToString() + "\t" + this.GetAPIHandle().ToString() + "\t" + this.GetFileMode().ToString() + "\n";
        }
        //-------------------------------------------
        internal long Fsize()
        {
            try
            {
                if (!checkHandle())
                    return -1;

                return m_fs.Length;
            }
            catch (Exception)// e)
            {
                SetError(fErrorCode.AccessDenied);
                return -1;
            }
        }
        //-------------------------------------------
        internal DateTime FDate()
        {
            DateTime dRetVal ;
						String name = GuineuInstance.FileMgr.MakePath(m_fullname.ToString());
            try
            {
                if (File.Exists(name))
                {
                    FileInfo fi = new FileInfo(name);
                    dRetVal = fi.LastWriteTime;
                    return dRetVal;
                }
                else
                {
                    SetError(fErrorCode.FilenotFound);
                    throw new ErrorException(ErrorCodes.FileNotFound);
                }
            }
            catch (Exception)// e)
            {
                SetError(fErrorCode.AccessDenied);
                throw new ErrorException(ErrorCodes.InvalidArgument);
            }
        }
        //-------------------------------------------
        internal string FTime()
        {
            string cRetVal = string.Empty;
						String name = GuineuInstance.FileMgr.MakePath(m_fullname.ToString());
						try
            {
                //Check if it exists
                if (System.IO.File.Exists(name))
                {
                    //Create the FileInfo object
                    FileInfo fi = new FileInfo(name);

                    //Call the LastAccessTime to get the last read/write/copy time
                    cRetVal = fi.LastWriteTime.ToShortTimeString();
                }
                else
                    SetError(fErrorCode.FilenotFound);

                return cRetVal.ToString();
            }
            catch (Exception)
            {
                SetError(fErrorCode.AccessDenied);
                return cRetVal.ToString();
            }
        }
				private bool disposed = false;

				public void Dispose()
				{
					Dispose(true);
					GC.SuppressFinalize(this);
				}

				protected virtual void Dispose(bool disposing)
				{
					if (!this.disposed)
						if (disposing)
						{
							this.m_fs.Close();
						}
					disposed = true;
				}
	    
		}

    //---------------------------------------------------------
    public class LowLevelFileMngr : List<LowLevelFile>
    {
        internal int m_lastLowLevelError;
        //---------------------------------------------------------
        public int Ferror()
        {
            return m_lastLowLevelError;
        }
        //---------------------------------------------------------
        public void Ferror(int lastError)
        {
            m_lastLowLevelError = lastError;
        }
        //---------------------------------------------------------
        internal int GetNewGuineuHandle(LowLevelFile newItem)
        {
            this.Add(newItem);
            return this.IndexOf(newItem) + 1;
        }
        //---------------------------------------------------------
        public void DisplayFilesStatus() // (Output chanel)
        {    // eg : LIST STATUS
            foreach (LowLevelFile fi in this)
            {
                fi.DisplayStatus();
                // chanel.write( fi.DisplayStatus());
            }
        }
        //---------------------------------------------------------
        public void CloseFiles()
        {    // eg : CLOSE ALL, QUIT ...
            foreach (LowLevelFile fi in this)
            {
                fi.FClose();
                this.Remove(fi);
            }
        }

        //---------------------------------------------------------
        public void Dispose()
        {
            this.CloseFiles();
        }

        //---------------------------------------------------------
        private LowLevelFile GetItem(int guineuHandle)
        {
            foreach (LowLevelFile fi in this)
            {
                if (fi.GetGuiNeuHandle() == guineuHandle)
                    return fi;
            }

            this.Ferror(6); // invalid handle

            return null;
            // TODO : learn C# <lol> dunno how that bloody list works...
            /*   // let the collection do it itself
            LowLevelFile[] fi=new LowLevelFile[1];
            try
            {
                fi[0] = this.GetRange(guineuHandle-1, 1);
                return fi[0];
            }
            catch (Exception)
            {
                return null;
            }
             */
        }
        //---------------------
        public int FCreate(string path, int nFlags)
        {
            LowLevelFile fi = new LowLevelFile(this);
            if (fi != null)
            {
                return fi.FCreate(path, nFlags);
            }
            return -1;
        }
        //---------------------
        public int FOpen(string path, int nFlags)
        {
            LowLevelFile fi = new LowLevelFile(this);
            if (fi != null)
            {
                return fi.FOpen(path, nFlags);
            }
            return -1;
        }
        //---------------------
        public bool FClose(int guineuHandle)
        {
            LowLevelFile fi = GetItem(guineuHandle);
            if (fi != null)
            {
                fi.FClose();
            }
            return true;
        }
        //---------------------
        public DateTime FDate(int guineuHandle)
        {
            LowLevelFile fi = GetItem(guineuHandle);
            return fi.FDate();
        }

        //---------------------
        public bool FFlush(int guineuHandle, bool force)
        {
            LowLevelFile fi = GetItem(guineuHandle);
            if (fi != null)
            {
                fi.FFlush(force);
            }
            return true;
        }
        //---------------------
        public bool FEof(int guineuHandle)
        {
            LowLevelFile fi = GetItem(guineuHandle);
            if (fi != null)
            {
                return fi.Feof();
            }
            return true;
        }
        //---------------------
        public string Fgets(int guineuHandle, long nbytes)
        {
            string cRetVal = string.Empty;
            LowLevelFile fi = GetItem(guineuHandle);
            if (fi != null)
            {
                cRetVal = fi.Fgets(nbytes);
            }
            return cRetVal;
        }
        //---------------------
        public string Fread(int guineuHandle, long nbytes)
        {
            string cRetVal = string.Empty;
            LowLevelFile fi = GetItem(guineuHandle);
            if (fi != null)
            {
                cRetVal = fi.Fread(nbytes);
            }
            return cRetVal;
        }
        //---------------------
        public long FChSize(int guineuHandle, long newSize)
        {
            LowLevelFile fi = GetItem(guineuHandle);
            if (fi != null)
            {
                return fi.FchSize(newSize);
            }
            return -1;
        }
        //---------------------
        public long Fputs(int guineuHandle, string expression, long carToWrite)
        {
            LowLevelFile fi = GetItem(guineuHandle);
            if (fi != null)
            {
                return fi.Fputs(expression, carToWrite);
            }
            return 0;
        }
        //---------------------
        public long Fwrite(int guineuHandle, string expression, long carToWrite)
        {
            LowLevelFile fi = GetItem(guineuHandle);
            if (fi != null)
            {
                return fi.Fwrite(expression, carToWrite);
            }
            return 0;
        }
        //---------------------
        public long Fseek(int guineuHandle, long nBytesMoved, int relativePosition)
        {
            LowLevelFile fi = GetItem(guineuHandle);
            if (fi != null)
            {
                return fi.Fseek(nBytesMoved, relativePosition);
            }
            return 0;
        }
    }
}

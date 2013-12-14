using System;
using System.IO;

namespace Guineu.Core
{
    public partial class LowLevelFile
    {
        bool checkHandle()
        {
            return true;
        }
        //------------------------------------------
        internal long Fputs(string expression, long carToWrite)
        {
            try
            {
                long oldPos = m_fs.Position;
                long curPos = oldPos;
                int nCarWritten = (int)carToWrite;

                if (!checkHandle())
                    return 0;

                if (carToWrite.CompareTo(0) > 0)
                {
                    string cExpr = expression.ToString();

                    if (nCarWritten.CompareTo(cExpr.Length) < 0)
                        cExpr = cExpr.Substring(1, nCarWritten);

                    nCarWritten = cExpr.Length;

                    if (cExpr.Length > 0)
                        Fwrite(expression, nCarWritten);
                }

                Fwrite("\r", 1);

                curPos = m_fs.Position;
                return curPos - oldPos;
            }
            catch (Exception)// e)
            {
                SetError(fErrorCode.AccessDenied);
                return 0;
            }
        }

        //------------------------------------------
        public void FClose()
        {
            if (m_fs == null)
                return;

            m_fs.Flush();
            //                m_fs.SafeFileHandle.SetHandleAsInvalid();
            SetError(fErrorCode.NoError);

            m_fs.Close();
            //m_fs.Dispose();
            m_fs = null;
        }

        //------------------------------------------
        internal int FOpen(string filename, int nAttr)
        {
            try
            {
                m_guineuHandle = -1;
                m_fullname = Path.GetFullPath(filename);

                if (!File.Exists(m_fullname.ToString()))
                {
                    SetError(fErrorCode.FilenotFound);
                    return -1;
                }

                switch (nAttr)
                {
                    case 0: // Read-Only, Buffered ???
                        m_fs = new FileStream(m_fullname.ToString(), FileMode.Open, FileAccess.Read);
                        break;
                    case 1: // Write-Only, Buffered
                        m_fs = new FileStream(m_fullname.ToString(), FileMode.Open, FileAccess.Write);
                        break;
                    case 2: // Read-Write, Buffered
                        m_fs = new FileStream(m_fullname.ToString(), FileMode.Open, FileAccess.ReadWrite);
                        break;
                    case 10:// ReadOnly
                        m_fs = new FileStream(m_fullname.ToString(), FileMode.Open, FileAccess.Read);
                        break;
                    case 11:// WriteOnly
                        m_fs = new FileStream(m_fullname.ToString(), FileMode.Open, FileAccess.Write);
                        break;
                    case 12:// ReadWrite
                        m_fs = new FileStream(m_fullname.ToString(), FileMode.Open, FileAccess.ReadWrite);
                        break;
                    default:// exception
                        throw new ErrorException(ErrorCodes.InvalidArgument);
                    //                    break;
                }

                if (!checkHandle())
                    SetError(fErrorCode.InvalidFileHandle);
                else
                    SetError(fErrorCode.NoError);

                m_guineuHandle = Parent.GetNewGuineuHandle(this);
                m_apiHandle = m_fs.ToString();
            }

            catch (Exception)
            {
                SetError(fErrorCode.AccessDenied); // seems to be the default FERROR of VFP
            }

            return m_guineuHandle;
        }

        //--------------------------------------------
        internal int FCreate(string filename, int nFileAttribute)
        {
            m_guineuHandle = -1;

            try
            {
                // return the Guineu handle and set internals properties
                m_fullname = Path.GetFullPath(filename);
                if(nFileAttribute==0)
                    m_fs = new FileStream(filename.ToString(), FileMode.Create, FileAccess.ReadWrite);
                else
                    m_fs = new FileStream(filename.ToString(), FileMode.Create);

                m_guineuHandle = Parent.GetNewGuineuHandle(this);
                
                if(!checkHandle())
                    SetError(fErrorCode.InvalidFileHandle);
                else
                    SetError(fErrorCode.NoError);

                m_apiHandle = m_fs.ToString();
            }

            catch (Exception)
            {
                SetError(fErrorCode.AccessDenied); // seems to be the default FERROR of VFP
            }

            return m_guineuHandle;
        }
    }

}

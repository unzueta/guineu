using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;

namespace Guineu.Core
{
    public partial class LowLevelFile : IDisposable
    {
        //------------------------------------------
        private bool checkHandle()
        {
            if (m_fs.SafeFileHandle.IsInvalid)
            {
                SetError(fErrorCode.InvalidFileHandle);
                return false;
            }
            else
                SetError(fErrorCode.NoError);
            return true;
        }

        //------------------------------------------
        internal void FClose()
        {
            if (m_fs == null)
                return;

            if (!m_fs.SafeFileHandle.IsInvalid)
            {
                m_fs.Flush();
                //                m_fs.SafeFileHandle.SetHandleAsInvalid();
                SetError(fErrorCode.NoError);
            }
            else
                SetError(fErrorCode.InvalidFileHandle);
            m_fs.Close();
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
                    case 0: // Read-Only, Buffered
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

                if (m_fs.SafeFileHandle.IsInvalid)
                    SetError(fErrorCode.InvalidFileHandle);
                else
                    SetError(fErrorCode.NoError);

                m_guineuHandle = Parent.GetNewGuineuHandle(this);
                m_apiHandle = m_fs.SafeFileHandle.DangerousGetHandle().ToString();
            }
            catch (FileLoadException)// fle)
            {
                SetError(fErrorCode.TooManyFiles);
            }
            catch (PathTooLongException)// pe)
            {
                SetError(fErrorCode.FilenotFound);
            }
            catch (FileNotFoundException)// fe)
            {
                SetError(fErrorCode.FilenotFound);
            }
            catch (DirectoryNotFoundException)// de)
            {
                SetError(fErrorCode.FilenotFound);
            }
            catch (UnauthorizedAccessException)// ue)
            {
                SetError(fErrorCode.AccessDenied);
            }
            catch (IOException)
            {
                SetError(fErrorCode.AccessDenied);
            }
            catch (ArgumentException)
            {
                SetError(fErrorCode.AccessDenied);
            }
            catch (System.Security.SecurityException)
            {
                SetError(fErrorCode.AccessDenied);
            }
            catch (Exception)
            {
                SetError(fErrorCode.AccessDenied); // seems to be the default FERROR of VFP
            }

            //            if (m_fs.SafeFileHandle.IsInvalid)
            //                return -1;

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
                m_fs = new FileStream(filename.ToString(), FileMode.Create);
                m_fs.Close();

                switch (nFileAttribute)
                {
                    case 0: // read/write
                        File.SetAttributes(m_fullname, FileAttributes.Normal);
                        break;
                    case 1: // readonly
                        File.SetAttributes(m_fullname, FileAttributes.ReadOnly);
                        break;
                    case 2: // hidden
                        File.SetAttributes(m_fullname, FileAttributes.Hidden);
                        break;
                    case 3: // r/o, hidden
                        File.SetAttributes(m_fullname, FileAttributes.Hidden);
                        File.SetAttributes(m_fullname, FileAttributes.ReadOnly);
                        break;
                    case 4: // system
                        File.SetAttributes(m_fullname, FileAttributes.System);
                        break;
                    case 5: // ro/system
                        File.SetAttributes(m_fullname, FileAttributes.System);
                        File.SetAttributes(m_fullname, FileAttributes.ReadOnly);
                        break;
                    case 6: // system/hidden
                        File.SetAttributes(m_fullname, FileAttributes.System);
                        File.SetAttributes(m_fullname, FileAttributes.Hidden);
                        break;
                    case 7: // ro, system, hidden
                        File.SetAttributes(m_fullname, FileAttributes.System);
                        File.SetAttributes(m_fullname, FileAttributes.Hidden);
                        File.SetAttributes(m_fullname, FileAttributes.ReadOnly);
                        break;
                    default:// exception
                        throw new ErrorException(ErrorCodes.InvalidArgument);
                    //                    break;
                }
                m_fs = new FileStream(m_fullname, FileMode.Open);
                m_guineuHandle = Parent.GetNewGuineuHandle(this);
                if (m_fs.SafeFileHandle.IsInvalid)
                    SetError(fErrorCode.InvalidFileHandle);
                else
                    SetError(fErrorCode.NoError);

                m_apiHandle = m_fs.SafeFileHandle.DangerousGetHandle().ToString();
            }
            catch (FileLoadException) // fle)
            {
                SetError(fErrorCode.TooManyFiles);
            }
            catch (PathTooLongException) // pe)
            {
                SetError(fErrorCode.FilenotFound);
            }
            catch (FileNotFoundException) // fe)
            {
                SetError(fErrorCode.FilenotFound);
            }
            catch (DirectoryNotFoundException) // de)
            {
                SetError(fErrorCode.FilenotFound);
            }
            catch (UnauthorizedAccessException) // ue)
            {
                SetError(fErrorCode.AccessDenied);
            }
            catch (Exception)
            {
                SetError(fErrorCode.AccessDenied); // seems to be the default FERROR of VFP
            }

            //           if (m_fs.SafeFileHandle.IsInvalid)
            //               return -1;

            return m_guineuHandle;
        }
        //------------------------------------------
        internal long Fputs(string expression, long carToWrite)
        {
            try
            {
                long oldPos = m_fs.Position;
                long curPos = oldPos;
                long nCarWritten = carToWrite;

                if (!checkHandle())
                    return 0;

                if (carToWrite.CompareTo(0) > 0)
                {
                    string cExpr = expression.ToString();

                    if (nCarWritten.CompareTo(cExpr.Length) < 0)
                        cExpr = cExpr.Substring(1, (int)nCarWritten);

                    nCarWritten = cExpr.Length;

                    if (cExpr.Length > 0)
                        Fwrite(expression, nCarWritten);
                }

                Fwrite(Environment.NewLine.ToString(), Environment.NewLine.Length);

                curPos = m_fs.Position;
                return curPos - oldPos;
            }
            catch (Exception)// e)
            {
                SetError(fErrorCode.AccessDenied);
                return 0;
            }
        }
    }


}

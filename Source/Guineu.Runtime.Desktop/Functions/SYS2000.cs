using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu.Functions
{
    class SYS2000 : ISys
    {
        static int index=-1; // init to react on a first call with , 1 param
        static string mSearchPattern;

        public String getString(CallingContext context, List<ExpressionBase> param)
        {
            int indexOfSep, indexOfUnit;
            DirectoryInfo di;
            String path;
            string pattern;
            FileInfo[] afi;

            if (param.Count >= 2)
            {
                Variant value = param[1].GetVariant(context);
                if (value.IsNull)
                {
                    throw new ErrorException(ErrorCodes.InvalidArgument);
                }
            }

            if (param.Count == 2)
            {
                // set m_index to -1
                index = -1;

                // resolve the directory stuff for directoryinfo
                // and get the SearchPattern
                pattern = param[1].GetString(context);
                indexOfSep = pattern.LastIndexOf(Path.DirectorySeparatorChar);
                indexOfUnit = pattern.IndexOf(Path.VolumeSeparatorChar);

                if (indexOfSep < 0 && indexOfUnit < 0)
                {
                    path = GuineuInstance.FileMgr.CurrentDirectory;
                    mSearchPattern = pattern;
                }
                else
                {
                    if (indexOfUnit >= 0 && indexOfSep < 0)
                    {
                        path = pattern.Substring(0, indexOfUnit + 1);
                        mSearchPattern = pattern.Substring(indexOfUnit + 1);
                    }
                    else
                    {
                        path = pattern.Substring(0, indexOfSep);
                        mSearchPattern = pattern.Substring(indexOfSep + 1);
                    }
                }

                if (Directory.Exists(path))
                {
                    di = new DirectoryInfo(path);
                    // fill the fileinfo array
                    afi = di.GetFiles(mSearchPattern);
                    // index++
                    index++;

                    // return the m_index member
                    if (index < afi.Length)
                        return afi[index].Name;
                }
            }
            else if(param.Count>=3)
            {
                // if index<0 return empty string
                if (index < 0)
                    return string.Empty;

                pattern = param[1].GetString(context);
                indexOfSep = pattern.LastIndexOf(Path.DirectorySeparatorChar);
                indexOfUnit = pattern.IndexOf(Path.VolumeSeparatorChar);

                if (indexOfSep < 0 && indexOfUnit < 0)
                {
                    path = GuineuInstance.FileMgr.CurrentDirectory;
                    // m_SearchPattern = pattern;
                }
                else
                {
                    if (indexOfUnit >= 0 && indexOfSep < 0)
                    {
                        path = pattern.Substring(0, indexOfUnit + 1);
                        // m_SearchPattern = pattern.Substring(indexOfUnit + 1);
                    }
                    else
                    {
                        path = pattern.Substring(0, indexOfSep);
                        // m_SearchPattern = pattern.Substring(indexOfSep + 1);
                    }
                }

                if (Directory.Exists(path))
                {
                    di = new DirectoryInfo(path);
                    // fill the fileinfo array
                    afi = di.GetFiles(mSearchPattern);
                    // index++
                    index++;

                    // return the m_index member
                    if (index < afi.Length)
                        return afi[index].Name;
                }
            }
            return string.Empty;
        }
    }

}
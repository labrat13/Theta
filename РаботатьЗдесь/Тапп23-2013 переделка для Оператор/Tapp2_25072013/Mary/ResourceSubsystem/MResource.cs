using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Mary
{
    public class MResource
    {
        /// <summary>
        /// Backreference to container
        /// </summary>
        private MEngine m_container;
        /// <summary>
        /// Max existing file identifier value
        /// </summary>
        private UInt32 m_fileid;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backref">Container reference backlink</param>
        public MResource(MEngine backref)
        {
            m_container = backref;
            m_fileid = 0;
        }

        #region Properties
        /// <summary>
        /// Resource file ID value
        /// </summary>
        internal uint FileId
        {
            get { return m_fileid; }
            set { m_fileid = value; }
        }


        #endregion


        /// <summary>
        /// NT-Init Resources manager in container opening
        /// </summary>
        internal void Open(MSolutionInfo projectInfo)
        {
            //store max fileid
            m_fileid = GetMaxExistingFileId();
            //сейчас MSolutionInfo не содержит каких-либо данных для менеджера ресурсов.
            //...
        }

        /// <summary>
        /// NR-Exit resorce manager in container closing
        /// </summary>
        internal void Close()
        {
        }


        /// <summary>
        /// NT-Generate pathname of file from file identifier
        /// </summary>
        /// <param name="fileId">Resource file id</param>
        /// <returns>Pathname without extension, like "C:\SolutionName\Resources\A01\B02\C03\01020304"</returns>
        /// <exception cref=""></exception>
        private string GeneratePathNameFromId(UInt32 fileId)
        {
            //get file name
            string name = String.Format("{0:X8}", fileId);
            //get folder names
            string dirA = String.Concat("A", name.Substring(0, 2));
            string dirB = String.Concat("B", name.Substring(2, 2));
            string dirC = String.Concat("C", name.Substring(4, 2));
            //create path
            string path = Path.Combine(this.m_container.SolutionManager.ResourceFolderPath, Path.Combine(Path.Combine(dirA, dirB), Path.Combine(dirC, name)));
            return path;
        }


        /// <summary>
        /// NT-Get id of max of existing files
        /// </summary>
        /// <returns>Max file id number or 0 if not found</returns>
        /// <remarks>
        /// This function removes visited dummy directories, but not all.
        /// For speed, need to remove all dummy directories in optimization service.
        /// </remarks>
        /// <exception cref=""></exception>
        public UInt32 GetMaxExistingFileId()
        {
            //1) search max folder at level A
            //if no folders, return 0
            //2) search max folder at level B
            //if no folders, delete current folder A, restart search
            //3) search max folder at level C
            //if no folders, delete current folder B, restart search
            //4) search max file in folder C
            //if no files, delete current folder C, restart search
            //5) convert filename to uint and return

            DirectoryInfo di = new DirectoryInfo(this.m_container.SolutionManager.ResourceFolderPath);
            FileInfo maxFile = null;
            while (true)
            {
                DirectoryInfo dirA = getMaxFolder(di, "A??");
                if (dirA == null) break;
                DirectoryInfo dirB = getMaxFolder(dirA, "B??");
                if (dirB == null)
                {
                    //folder A is dummy
                    //remove folder A, restart search
                    dirA.Delete(true);
                    continue;
                }
                DirectoryInfo dirC = getMaxFolder(dirB, "C??");
                if (dirC == null)
                {
                    //folder B is dummy
                    //remove folder B, restart search
                    dirB.Delete(true);
                    continue;
                }
                maxFile = getMaxFileName(dirC);
                if (maxFile == null)
                {
                    //folder C is dummy
                    //remove folder C, restart search
                    dirC.Delete(true);
                    continue;
                }
                else
                {
                    break; //break loop
                }
            }//end while
            //here maxFile is null or max file
            if (maxFile == null) return 0;
            return getIdFromFileName(maxFile.Name);
        }

        /// <summary>
        /// NT-Get file Id from file pathname
        /// </summary>
        /// <param name="pathname">file pathname</param>
        /// <returns>File id number or 0 if name is not valid resource filename</returns>
        /// <exception cref=""></exception>
        public uint getIdFromFileName(string pathname)
        {
            string fname = Path.GetFileName(pathname);
            if (!isValidFileName(fname)) return 0;
            //remove extensions
            string ff = fname.Substring(0, 8);
            //parse filename to uint
            uint result;
            if (UInt32.TryParse(ff, System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result)) return result;
            return 0;
        }

        /// <summary>
        /// NT-Get max of file name in specified directory
        /// </summary>
        /// <param name="di">Directory</param>
        /// <returns>Return max file info or return null if no files in directory</returns>
        /// <remarks>Эта функция может использоваться для проверки, есть ли в папке файлы ресурсов</remarks>
        /// <exception cref=""></exception>
        private FileInfo getMaxFileName(DirectoryInfo di)
        {
            FileInfo[] fir = di.GetFiles("????????.*", SearchOption.TopDirectoryOnly);
            if (fir.Length == 0) return null;
            //get max filename
            FileInfo max = fir[0];
            foreach (FileInfo fi in fir)
            {
                if (isValidFileName(fi.Name))
                {
                    if (String.Compare(max.Name, fi.Name) < 0)
                        max = fi;
                }
            }
            return max;
        }
        /// <summary>
        /// NT-Return True if file has valid resource file name
        /// </summary>
        /// <param name="name">File name without path</param>
        /// <returns>Return True if file has valid resource file name. Return False otherwise.</returns>
        /// <exception cref=""></exception>
        private bool isValidFileName(string name)
        {
            //file must math template HHHHHHHH.*
            if (name.Length > 8) return false;
            char[] car = name.ToCharArray(0, 8);
            foreach (char c in car)
                if (!MUtility.isHexDigit(c)) return false; //use class Uri for Hex checking
            return true;
        }



        /// <summary>
        /// NT-Get folder with max name of folders matches of specified mask
        /// </summary>
        /// <param name="di">parent folder</param>
        /// <param name="mask">Searh pattern like "A??"</param>
        /// <returns>Return folder with max name or null if no folders founded</returns>
        /// <exception cref=""></exception>
        private DirectoryInfo getMaxFolder(DirectoryInfo di, string mask)
        {
            DirectoryInfo[] dar = di.GetDirectories(mask, SearchOption.TopDirectoryOnly);
            if (dar.Length == 0) return null;
            DirectoryInfo max = dar[0];
            foreach (DirectoryInfo dd in dar)
            {
                if (String.Compare(max.Name, dd.Name) < 0)
                    max = dd;
            }
            return max;
        }
        /// <summary>
        /// NT-Remove dummy folders to optimize resources
        /// </summary>
        /// <remarks>Пустыми считаются папки, не содержащие файлов ресурсов или папок, содержащих файлы ресурсов</remarks>
        /// <exception cref=""></exception>
        internal void optRemoveDummyFolders()
        {
            //1 - get parent directory
            DirectoryInfo di = new DirectoryInfo(this.m_container.SolutionManager.ResourceFolderPath);
            //2 - check child directories
            DirectoryInfo[] dirA = di.GetDirectories("A??", SearchOption.TopDirectoryOnly);
            //if (dirA.Length == 0) return; //if no A dirs 
            //process A directories
            foreach (DirectoryInfo da in dirA)
            {
                DirectoryInfo[] dirB = da.GetDirectories("B??", SearchOption.TopDirectoryOnly);
                if (dirB.Length == 0)
                {
                    da.Delete(true); //delete dummy folder A
                }
                else
                {
                    //process B directories
                    foreach (DirectoryInfo db in dirB)
                    {
                        DirectoryInfo[] dirC = db.GetDirectories("C??", SearchOption.TopDirectoryOnly);
                        if (dirC.Length == 0)
                        {
                            db.Delete(true); //delete dummy folder B
                        }
                        else
                        {
                            //process C directories
                            foreach (DirectoryInfo dc in dirC)
                            {
                                FileInfo[] fir = dc.GetFiles("????????.*", SearchOption.TopDirectoryOnly);
                                if (fir.Length == 0) 
                                    dc.Delete(true);
                            }
                        }
                    } //end foreach B
                }
            } //end foreach A
            return;
        }

        /// <summary>
        /// NT-Remove resource file by optimizer
        /// </summary>
        /// <param name="fileId">Resource file id</param>
        /// <exception cref=""></exception>
        public void optRemoveResourceFile(UInt32 fileId)
        {
            //Удалить все файлы с указанным именем и любым расширением.
            FileInfo[] fir = getFileInfosById(fileId);
            foreach (FileInfo fi in fir)
            {
                fi.IsReadOnly = false;
                fi.Delete();
            }
            return;
        }

        /// <summary>
        /// NT-Get Resource folder count and size
        /// </summary>
        /// <param name="stat">Object for store results</param>
        /// <remarks>Считаются все существующие файлы и каталоги без разбора.</remarks>
        /// <exception cref=""></exception>
        public void GetResourceStatistics(Mary.Serialization.MStatistic stat)
        {
            //get resource folder path
            DirectoryInfo di = new DirectoryInfo(this.m_container.SolutionManager.ResourceFolderPath);
            stat.ResourceFiles = 0;
            stat.ResourceSize = 0;
            getResourceCount(di, stat);
            return;
        }

        /// <summary>
        /// NT-Count resource files in directories and get statistics
        /// </summary>
        /// <param name="d">DirectoryInfo of current directory</param>
        /// <param name="stat">Object for store results</param>
        /// <exception cref=""></exception>
        private void getResourceCount(DirectoryInfo d, Mary.Serialization.MStatistic stat)
        {
            // Process files.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                stat.ResourceSize += fi.Length;
            }
            stat.ResourceFiles += fis.Length;
            //Process subdirectories.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                getResourceCount(di, stat);
            }
            return;
        }

        /// <summary>
        /// NT-Get array of FileInfo, represent files with names that matches specified file id.
        /// This also detect no files or multiple files with same name but another extensions
        /// </summary>
        /// <param name="fileId">Resource file id</param>
        /// <returns>Returns array of FileInfo, represent files with names that matches specified file id.</returns>
        /// <exception cref=""></exception>
        internal FileInfo[] getFileInfosById(UInt32 fileId)
        {
            //create file pathname without extension
            string filepath = GeneratePathNameFromId(fileId);
            string folder = Path.GetDirectoryName(filepath);
            //open folder that contains required file
            DirectoryInfo di = new DirectoryInfo(folder);
            //create file search mask like xxxxxxxx.*
            string fmask = Path.ChangeExtension(Path.GetFileName(filepath), ".*");
            //search file by mask
            FileInfo[] fir = di.GetFiles(fmask, SearchOption.TopDirectoryOnly);
            return fir;
        }

        /// <summary>
        /// NT-Check that file exists
        /// </summary>
        /// <param name="fileId">Resource file id</param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public bool isFileExists(UInt32 fileId)
        {
            FileInfo[] fir = getFileInfosById(fileId);
            return (fir.Length > 0);
        }

        /// <summary>
        /// NT-Get FileInfo of resource file by file id. Return null if file not exists
        /// </summary>
        /// <param name="fileId">Resource file id</param>
        /// <returns></returns>
        /// <remarks>Возвращается первый из найденных файлов или нуль, если ничего нет.</remarks>
        /// <exception cref=""></exception>
        public FileInfo GetFileInfoById(UInt32 fileId)
        {
            FileInfo[] fir = getFileInfosById(fileId);
            if (fir.Length == 0) return null;
            else return fir[0];
        }


        /// <summary>
        /// NR-Remove all files and folders in Resources folder
        /// </summary>
        internal void ClearResources()
        {
            string[] dar = Directory.GetDirectories(m_container.SolutionManager.ResourceFolderPath);
            foreach(string s in dar)
                Directory.Delete(s, true);
            dar = Directory.GetFiles(m_container.SolutionManager.ResourceFolderPath);
            foreach(string s in dar)
                File.Delete(s);
            return;
        }

    }
}

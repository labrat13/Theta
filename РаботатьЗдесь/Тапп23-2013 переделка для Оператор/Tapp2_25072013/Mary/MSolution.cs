using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.AccessControl;

namespace Mary
{
    /// <summary>
    /// Maintains project file system and project structure
    /// </summary>
    public class MSolution
    {
        public const string m_SnapshotFolderName = "Snapshots";
        public const string m_LogFolderName = "Logs";
        public const string m_MethodFolderName = "Methods";
        public const string m_ResourceFolderName = "Resources";
        
        /// <summary>
        /// Project file 
        /// </summary>
        private MSolutionInfo m_solutionFile;
        /// <summary>
        /// Backref to container
        /// </summary>
        private MEngine m_container;



        /// <summary>
        /// Param constructor
        /// </summary>
        public MSolution(MEngine me)
        {
            m_container = me;
        }


#region Properties
        /// <summary>
        /// Project folder path
        /// </summary>
        public string SolutionFolderPath
        {
            get
            {
                return m_solutionFile.getCurrentSolutionDirectory();
            }
        }
        /// <summary>
        /// Project file reference
        /// </summary>
        public MSolutionInfo SolutionSettings
        {
            get { return m_solutionFile; }
            set
            {
                //set project file ref
                m_solutionFile = value;
            }
        }
        /// <summary>
        /// Is this project use database?
        /// </summary>
        public bool UsesDatabase
        {
            get { return m_solutionFile.IsDBused(); }
        }

        /// <summary>
        /// Snapshot folder path
        /// </summary>
        public string SnapshotFolderPath
        {
            get { return Path.Combine(SolutionFolderPath, m_SnapshotFolderName); } 
        }
        /// <summary>
        /// Resource main folder path
        /// </summary>
        public string ResourceFolderPath
        {
            get { return Path.Combine(SolutionFolderPath, m_ResourceFolderName); } 
        }
        /// <summary>
        /// Log folder path
        /// </summary>
        public string LogFolderPath
        {
            get { return Path.Combine(SolutionFolderPath, m_LogFolderName); } 
        }
        /// <summary>
        /// Method folder path
        /// </summary>
        public string MethodFolderPath
        {
            get { return Path.Combine(SolutionFolderPath, m_MethodFolderName); }
        }


#endregion
        /// <summary>
        /// NT-Initialize manager
        /// </summary>
        /// <param name="projectInfo"></param>
        internal void Open(MSolutionInfo solutionInfo)
        {
            this.m_solutionFile = solutionInfo;
        }

        internal void Close()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// NFT-Create project folders and files
        /// </summary>
        /// <param name="pfile">Fullfilled project file</param>
        /// <param name="rootDir">root directory, above Solution directory</param>
        /// <exception cref="Exception">Выбрасывает исключение если корневой каталог не существует</exception>
        /// <exception cref="Exception">Выбрасывает исключение если каталог солюшена уже существует</exception>
        /// <remarks>Без лога, или проверять его существование!</remarks>
        public static void CreateSolutionFolder(MSolutionInfo pfile, string rootDir)
        {
            //check file permissions
            if (!Directory.Exists(rootDir)) throw new Exception("Directory not exists");
            //DirectorySecurity ds = Directory.GetAccessControl(rootDir);
            //ds.

            //create main directory
            string maindir = Path.Combine(rootDir, pfile.getSolutionName16());
            //надо убедиться, что указаного каталога еще нет - чтобы не перезаписать существующий одноименный солюшен.
            if (Directory.Exists(maindir))
                throw new Exception(String.Format("Directory {0} already exists", maindir));

            Directory.CreateDirectory(maindir);
            //create subdirectories
            Directory.CreateDirectory(Path.Combine(maindir, MSolution.m_LogFolderName));
            Directory.CreateDirectory(Path.Combine(maindir, MSolution.m_MethodFolderName));
            Directory.CreateDirectory(Path.Combine(maindir, MSolution.m_ResourceFolderName));
            Directory.CreateDirectory(Path.Combine(maindir, MSolution.m_SnapshotFolderName));
            //создать ФайлСолюшена
            string fname = String.Format("{0}.tapj", pfile.getSolutionName16());
            pfile.SolutionFilePath = Path.Combine(maindir, fname);
            pfile.Save();

            return;
        }
        /// <summary>
        /// NT- Delete folder and all items within - if exists
        /// </summary>
        /// <param name="p"></param>
        internal static void DeleteFolder(string p)
        {
            //todo: тут удалять в корзину, а не насовсем.
            if(Directory.Exists(p) == true)
                Directory.Delete(p, true);

            return;
        }



    }
}

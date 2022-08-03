using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;

namespace Mary
{
    /// <summary>
    /// Class represent project information and project file 
    /// </summary>
    /// <remarks>ProjectFile old class</remarks>
    /// <seealso cref=""/>
    public class MSolutionInfo: ImSerializable
    {   
        //todo: rename class to SolutionInfo
        //todo: rename here all words Project, prj, etc to Solution
        
        #region *** Constants and Solution settings fields ***        

        /// <summary>
        /// Текущая версия Движка, реализованная в коде этой сборки
        /// </summary>
        private const string EngineVersionString = "2.0.0.0";

        /// <summary>
        /// Текущая версия Движка, реализованная в коде этой сборки
        /// </summary>
        private MEngineVersionInfo m_currentEngineVersion;
        #endregion

        #region *** Fields ***
        //****** Solution properties ***********
        /// <summary>
        /// Project creation date
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private DateTime m_creadate;
        /// <summary>
        /// Project description
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private string m_SolutionDescr;  
        /// <summary>
        /// Project name
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private string m_SolutionName;   
        /// <summary>
        /// Project identifier
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private int m_solutionId;
        /// <summary>
        /// Project file pathname
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private string m_SolutionFilePath;
        /// <summary>
        /// Solution version info
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private MSolutionVersionInfo m_solutionVersion;
        /// <summary>
        /// Engine version info of Solution
        /// </summary>
        private MEngineVersionInfo m_solutionEngineVersion;


        //**** container properties *****
        /// <summary>
        /// Container IsActive flag value
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private bool m_containerIsActive;
        /// <summary>
        /// Container service flag value
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private int m_containerServiceFlag;
        /// <summary>
        /// Container state identifier
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private UInt64 m_containerStateId_U64;

        /// <summary>
        /// Log file number
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private int m_logfileNumber;
        /// <summary>
        /// Log details flags
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private MMessageClass m_logdetails;
        /// <summary>
        /// Container default cell mode
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private MCellMode m_defaultCellMode;

        //******** database properties **********
        /// <summary>
        /// Project database type
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private MDatabaseType m_dbtype;
        /// <summary>
        /// Database name
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private string m_dbname;
        /// <summary>
        /// Database server path
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private string m_dbServerPath;
        /// <summary>
        /// User name of database server account
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private string m_username;
        /// <summary>
        /// User password of database server account
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private string m_userpass;
        /// <summary>
        /// Project connection timeout, seconds
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private int m_dbtimeout;
        /// <summary>
        /// Database server connection port, 0 for default port
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private int m_dbport;
        /// <summary>
        /// Server uses integrated security mode (for mssql2005)
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        private bool m_useIntegratedSecurity;

        #endregion
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public MSolutionInfo()
        {
            //settings
            this.m_currentEngineVersion = new MEngineVersionInfo(EngineVersionString);//TODO: TAGVERSIONNEW: - переделано
            //Solution properties
            m_creadate = DateTime.Now;
            m_solutionVersion = new MSolutionVersionInfo("1.0.0.0");//TODO: TAGVERSIONNEW: - переделано
            m_solutionEngineVersion = new MEngineVersionInfo(EngineVersionString);//TODO: TAGVERSIONNEW: - переделано
            m_SolutionDescr = "";  
            m_SolutionName = "";        
            m_SolutionFilePath = "";
            m_solutionId = 1;
            //container properties
            m_containerServiceFlag = 0;
            m_defaultCellMode = MCellMode.Temporary; //для совместимости с ПроектБезБД
            m_logdetails = MMessageClass.All;
            m_logfileNumber = 0;
            m_containerStateId_U64 = 0;
            //database properties
            m_dbtype = MDatabaseType.Unknown;
            m_dbname = "";
            m_dbServerPath = "";
            m_dbport = 0;
            m_dbtimeout = 60;
            m_useIntegratedSecurity = false;
            m_username = "";
            m_userpass = "";
            m_containerIsActive = true;
            m_containerServiceFlag = 0;
            m_containerStateId_U64 = 0;
        }

        #region *** Solution properties ***
        /// <summary>
        /// Project identifier
        /// </summary>
        /// <remarks>Same as container identifier</remarks>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Solution properties"), Description("Solution Id number")]
        public int SolutionId
        {
            get
            {
                return m_solutionId;
            }
            set
            {
                m_solutionId = value;
            }
        }

        /// <summary>
        /// Project name
        /// </summary>
        /// <remarks>Same as container name</remarks>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Solution properties"), Description("Solution name")]
        public string SolutionName
        {
            get
            {
                return m_SolutionName;
            }
            set
            {
                m_SolutionName = value;
            }
        }

        /// <summary>
        /// Project description
        /// </summary>
        /// <remarks>Same as container description</remarks>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Solution properties"), Description("Solution description")]
        public string SolutionDescription
        {
            get
            {
                return m_SolutionDescr;
            }
            set
            {
                m_SolutionDescr = value;
            }
        }

        /// <summary>
        /// Project creation date
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Solution properties"), Description("Solution initial date")]
        public DateTime CreationDate
        {
            get
            {
                return m_creadate;
            }
            set
            {
                m_creadate = value;
            }
        }
        /// <summary>
        /// Solution engine version
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Solution properties"), Description("Version of engine where Solution is created")]
        public MEngineVersionInfo SolutionEngineVersion
        {
            get
            {
                return m_solutionEngineVersion;
            }
            set
            {
                m_solutionEngineVersion = value;
            }
        }

        /// <summary>
        /// Engine minor version number
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Solution properties"), Description("Solution dataset version")]
        public MSolutionVersionInfo SolutionVersion
        {
            get
            {
                return m_solutionVersion;
            }
            set
            {
                m_solutionVersion = value;
            }
        }

        /// <summary>
        /// Project file pathname
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [XmlIgnore]
        [Category("Solution properties"), Description("Solution file pathname")]
        public string SolutionFilePath
        {
            get
            {
                return m_SolutionFilePath;
            }
            set
            {
                m_SolutionFilePath = value;
            }
        }
    
        #endregion
        #region ** Database properties **
        /// <summary>
        /// Type of database for project
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Database"), Description("Database server type")]
        public MDatabaseType DatabaseType
        {
            get
            {
                return m_dbtype;
            }
            set
            {
                m_dbtype = value;
            }
        }
        /// <summary>
        /// Database server path
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Database"), Description("Database server path")]
        public string DatabaseServerPath
        {
            get
            {
                return m_dbServerPath;
            }
            set
            {
                m_dbServerPath = value;
            }
        }

        /// <summary>
        /// Database name
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Database"), Description("Project database name")]
        public string DatabaseName
        {
            get
            {
                return m_dbname;
            }
            set
            {
                m_dbname = value;
            }
        }

        /// <summary>
        /// Database server port number, 0 for default
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Database"), Description("Database server port number. Specify 0 for default value.")]
        public int DatabasePortNumber
        {
            get
            {
                return m_dbport;
            }
            set
            {
                m_dbport = value;
            }
        }
        /// <summary>
        /// Connection timeout, sec
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Database"), Description("Database server connection timeout in seconds")]
        public int DatabaseTimeout
        {
            get
            {
                return m_dbtimeout;
            }
            set
            {
                m_dbtimeout = value;
            }
        }

        /// <summary>
        /// User name of database server account
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Database"), Description("Database server account user name. For project creation, user must have dbcreator permission on SQL server")]
        public string UserName
        {
            get
            {
                return m_username;
            }
            set
            {
                m_username = value;
            }
        }

        /// <summary>
        /// User password of database server account
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Database"), Description("Database server account user password")]
        public string UserPassword
        {
            get
            {
                return m_userpass;
            }
            set
            {
                m_userpass = value;
            }
        }

        /// <summary>
        /// Flag use integrated security for MsSql2005
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Database"), Description("Database server uses IntegratedSecurity autorization mode. For MsSql2005 only.")]
        public bool UseIntegratedSecurity
        {
            get
            {
                return m_useIntegratedSecurity;
            }
            set
            {
                m_useIntegratedSecurity = value;
            }
        }
#endregion
        #region ** Container Properties **
        /// <summary>
        /// Container state identifier as U64-packed value
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Container properties"), Description("Container state identifier packed to U64")]
        public UInt64 ContainerState
        {
            get
            {
                return m_containerStateId_U64;
            }
            set
            {
                m_containerStateId_U64 = value;
            }
        }

        /// <summary>
        /// Container Is Active flag
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Container properties"), Description("Container is active flag value")]
        public bool ContainerIsActiveFlag
        {
            get
            {
                return m_containerIsActive;
            }
            set
            {
                m_containerIsActive = value;
            }
        }

        /// <summary>
        /// Container service flag value
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Container properties"), Description("Container service flag value")]
        public int ContainerServiceFlag
        {
            get
            {
                return m_containerServiceFlag;
            }
            set
            {
                m_containerServiceFlag = value;
            }
        }

        /// <summary>
        /// Container default cell mode для неявно загружаемых из БД ячеек
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Container properties"), Description("Container default cell mode")]
        public MCellMode ContainerDefaultCellMode
        {
            get
            {
                return m_defaultCellMode;
            }
            set
            {
                m_defaultCellMode = value;
            }
        }

        /// <summary>
        /// Log file number
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Container properties"), Description("Log file incremental number")]
        public int LogfileNumber
        {
            get
            {
                return m_logfileNumber;
            }
            set
            {
                m_logfileNumber = value;
            }
        }

        /// <summary>
        /// Log details flas
        /// </summary>
        /// <remarks></remarks>
        /// <value></value>
        /// <seealso cref=""/>
        [Category("Container properties"), Description("Log details flags")]
        public MMessageClass LogDetailsFlags
        {
            get
            {
                return m_logdetails;
            }
            set
            {
                m_logdetails = value;
            }
        }
        #endregion

        #region *** Solution settings properties ***
        /// <summary>
        /// Текущая версия Движка,реализованная в коде этой сборки
        /// </summary>
        public MEngineVersionInfo CurrentEngineVersion
        {
            get { return m_currentEngineVersion; }
        }

        #endregion

        #region ImSerializable Members
        /// <summary>
        /// Serialize object data to binary stream
        /// </summary>
        /// <param name="writer">Binary writer for data</param>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public void toBinary(BinaryWriter writer)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Deserialize object data from binary stream
        /// </summary>
        /// <param name="reader">Binary reader for data</param>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public void fromBinary(BinaryReader reader)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Serialize object data to text stream
        /// </summary>
        /// <param name="writer">Text writer for data</param>
        /// <param name="withHex">True - include HEX representation of binary data.False - text representation only.</param>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public void toText(TextWriter writer, bool withHex)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Deserialize object data from text stream
        /// </summary>
        /// <param name="reader">Text reader for data</param>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public void fromText(TextReader reader)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion


        /// <summary>
        /// NT-Create or overwrite project file with current path, write values, close project file.
        /// </summary>
        /// <remarks>Без записи в лог, или проверять его существование!</remarks>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public void Save()//TODO: TAGVERSIONNEW: - тестировать выгрузку и загрузку данных
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(this.GetType());
            System.IO.StreamWriter file = new System.IO.StreamWriter(this.m_SolutionFilePath);
            writer.Serialize(file, this);
            file.Close();
        }

        /// <summary>
        /// NT-Open project file, load values, close project file.
        /// </summary>
        /// <remarks>Без записи в лог, или проверять его существование!</remarks>
        /// <remarks></remarks>
        /// <returns>Возвращает информацию о проекте, хранящуюся в файле проекта.</returns>
        /// <seealso cref=""/>
        public static MSolutionInfo Load(string filename)
        {
            //load file
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(MSolutionInfo));
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            MSolutionInfo result = (MSolutionInfo)reader.Deserialize(file);
            file.Close();
            result.m_SolutionFilePath = filename;
            return result;
        }

        /// <summary>
        /// Get current solution directory from project file pathname
        /// </summary>
        /// <returns>Возвращает текущий каталог, в котором находится сейчас файл проекта.</returns>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public string getCurrentSolutionDirectory()
        {
            return Path.GetDirectoryName(this.m_SolutionFilePath);
        }

        /// <summary>
        /// Получить имя проекта, укороченное до 16 символов.
        /// </summary>
        /// <returns>Возвращает строку имени проекта, обрезанную до первых 16 символов.</returns>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public string getSolutionName16()
        {
            string res = this.m_SolutionName.Trim();
            if (res.Length > 16)
                res = res.Remove(16);
            return res;
        }
        /// <summary>
        /// Is project uses database?
        /// </summary>
        /// <returns>Returns true if project use database. False otherwise.</returns>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public bool IsDBused()
        {
            return (m_dbtype != MDatabaseType.NoDatabase);
        }

        /// <summary>
        /// NT-Проверяет данные проекта и выбрасывает исключения при ошибках
        /// </summary>
        internal void checkValues()
        {
            //check project name exists
            m_SolutionName = MUtility.processString(m_SolutionName, 256);
            if (String.IsNullOrEmpty(this.m_SolutionName))
                throw new Exception("Invalid project name");
            //check project description
            m_SolutionDescr = MUtility.processString(m_SolutionDescr, 8192);
            //check engine version
            //if (MVersionInfo.IsCompatibleVersion(getProjectVersion()) == false)//TODO: TAGVERSIONNEW: - переделано
            if (!this.m_currentEngineVersion.isCompatibleVersion(this.m_solutionEngineVersion))
                throw new Exception("Несовместимая версия движка проекта");
            //check database fields
            //таймаут значение не менее 10 секунд
            if (m_dbtimeout < 10) m_dbtimeout = 10;
            //Если имя БД не задано, используем 16 символов имени проекта
            m_dbname = MUtility.processString(m_dbname, 256); //TODO: set correct length here
            if (String.IsNullOrEmpty(m_dbname))
                m_dbname = getSolutionName16();
            //process user name. Имя пользователя и пароль могут быть пустыми.
            //В этом случае они вводятся в момент создания строки подключения.
            //пароль не обрабатываем, чтобы не вырезать пробельные символы
            //todo: тут надо дополнительно проработать процесс
            m_username = MUtility.processString(m_username, 256); //TODO: set correct length here

            //TODO: Проверку настроек БД в MSolutionInfo надо перенести в соответствующий класс адаптера БД
            if (m_dbtype == MDatabaseType.Unknown)
                throw new Exception("Invalid database type");
            else if (m_dbtype == MDatabaseType.MicrosoftSqlServer2005)
            {
                m_dbServerPath = MUtility.processString(m_dbServerPath, 8192);  //TODO: set correct length here
                if (String.IsNullOrEmpty(m_dbServerPath))
                    throw new Exception("Invalid path to database server");
            }
            else if(m_dbtype == MDatabaseType.NoDatabase)
            {
                //проверки специфичные для проекта без БД
                if (m_defaultCellMode != MCellMode.Temporary)
                    throw new Exception("Тип ячеек в ContainerDefaultCellMode не подходит для проекта");
            }
            else if (m_dbtype == MDatabaseType.MsAccess)
            {
                //тут нечего проверять
            }
            else if (m_dbtype == MDatabaseType.MySql5)
            {
                throw new Exception("Invalid database type");
            }
            else if (m_dbtype == MDatabaseType.Sqlite3)
            {
                throw new Exception("Invalid database type");
            }

            return;
        }




        /// <summary>
        /// Get string representation of object.
        /// </summary>
        /// <returns>Return string representation of object.</returns>
        /// <remarks></remarks>
        /// <seealso cref=""/>
        public override string ToString()
        {
            return base.ToString();
        }







    }
}

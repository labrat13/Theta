using System;
using System.Collections.Generic;
using System.Text;
using Mary.Serialization;
using System.IO;
using Mary.DatabaseAdapters;

namespace Mary
{
    public class MEngine: MElement
    {

#region Fields




        //---- Свойства контейнера ----
//TAG:RENEW-13112017 - все свойства перенесены в объект СвойстваСолюшена, а тут оставлены проперти для совместимости с интерфейсом класса MElement
        ///// <summary>
        ///// Container id number
        ///// </summary>
        //private int m_containerId;
        ///// <summary>
        ///// имя контейнера для пользователя
        ///// </summary>
        //private string m_name;
        ///// <summary>
        ///// текстовое описание, null по умолчанию.
        ///// </summary>
        //private String m_description;
        ///// <summary>
        ///// Вынесен из подклассов как общее свойство. //default 0
        ///// </summary>
        ///// <remarks>Непонятно зачем здесь нужен и как будет использоваться, но является частью общего для всех элементов интерфейса</remarks>
        //private MID m_state;
        ///// <summary>
        ///// flag is element active or deleted //default true
        ///// </summary>
        //private bool m_isactive;
        ///// <summary>
        ///// Поле для значения, используемого в сервисных операциях (поиск в графе,  обслуживание и так далее) //default 0
        ///// </summary>
        //private int m_serviceflag;

        //---- переменные движка ----

        /// <summary>
        /// Максимальный идентификатор постоянной ячейки
        /// </summary>
        private int m_maxConstCellID;
        /// <summary>
        /// Максимальный идентификатор временной ячейки
        /// </summary>
        private int m_maxTempCellID;
        ///// <summary>
        ///// Тип создаваемой ячейки, если явно не указан.
        ///// </summary>
        //private MCellMode m_DefaultCellMode; - перенесен в SolutionSettings объект
        /// <summary>
        /// коллекция ячеек
        /// </summary>
        private MCellCollection m_cells;
        /// <summary>
        /// коллекция связей
        /// </summary>
        private MLinkCollection m_links;

        //---- менеджеры подсистем ----
        /// <summary>
        /// Log manager
        /// </summary>
        private MLog m_log;
        /// <summary>
        /// Project filesystem manager
        /// </summary>
        private MSolution m_solutionMan;
        /// <summary>
        /// Resource manager
        /// </summary>
        private MResource m_resMan;
        /// <summary>
        /// Snapshot manager
        /// </summary>
        private MSnapshot m_snapMan;
        /// <summary>
        /// Methods manager
        /// </summary>
        private MMethod m_methodMan;
        /// <summary>
        /// слой абстракции БД
        /// </summary>
        private MDbAdapterBase m_dataLayer;
        //----  ----
        /// <summary>
        /// Solution settings, теперь хранит и настройки контейнера
        /// </summary>
        private MSolutionInfo m_solutionSettings;


#endregion


        /// <summary>
        /// NT-Create new container object. Call SolutionOpen() to open db connection and load container data.
        /// </summary>
        public MEngine()
        {
            m_cells = new MCellCollection();
            m_dataLayer = null;
            m_links = new MLinkCollection();
            m_log = null;
            m_maxConstCellID = 0;
            m_maxTempCellID = 0;

            //TAG:RENEW-13112017 - Эти поля перенесены в объект m_solutionSettings
            //m_isactive = true;
            //m_DefaultCellMode = MCellMode.Compact;
            //m_serviceflag = 0;
            //m_state = MID.fromU64(0);
            //m_description = String.Empty;
            //m_name = String.Empty;
            
            //new project managers
            m_methodMan = null;
            m_solutionMan = null;
            m_resMan = null;
            m_snapMan = null;
            //store engine reference for all cells
            MCell.Container = this;
            //SolutionSettings объект не создаем здесь, он должен приходить снаружи при открытии солюшена.
            //а если его нет, то это где-то ошибка и будет выброшено исключение, что и требуется.
        }

#region Properties

        /// <summary>
        /// имя контейнера для пользователя
        /// </summary>
        public String Name
        {
            get
            {
                return this.m_solutionSettings.SolutionName;
            }
            set
            {
                this.m_solutionSettings.SolutionName = value;
            }
        }

        /// <summary>
        /// Element description string
        /// </summary>
        public override string Description
        {
            get
            {
                return this.m_solutionSettings.SolutionDescription;
            }
            set
            {
                this.m_solutionSettings.SolutionDescription = value;
            }
        }

        /// <summary>
        /// Element is active 
        /// </summary>
        public override bool isActive
        {
            get
            {
                return this.m_solutionSettings.ContainerIsActiveFlag;
            }
            set
            {
                this.m_solutionSettings.ContainerIsActiveFlag = value;
            }
        }
        /// <summary>
        /// Value for servicing
        /// </summary>
        public override int ServiceFlag
        {
            get
            {
                return this.m_solutionSettings.ContainerServiceFlag;
            }
            set
            {
                this.m_solutionSettings.ContainerServiceFlag = value;
            }
        }
        /// <summary>
        /// Element state id
        /// </summary>
        public override MID State
        {
            get
            {
                return MID.fromU64(this.m_solutionSettings.ContainerState);
            }
            set
            {
                this.m_solutionSettings.ContainerState = value.toU64();
            }
        }
        /// <summary>
        /// Container ID number
        /// </summary>
        public int ContainerID
        {
            get { return this.m_solutionSettings.SolutionId; }
            set { this.m_solutionSettings.SolutionId = value; }
        }



        /// <summary>
        /// коллекция ячеек
        /// </summary>
        public MCellCollection Cells
        {
            get
            {
                return m_cells;
            }
        }

        /// <summary>
        /// коллекция связей
        /// </summary>
        public MLinkCollection Links
        {
            get
            {
                return m_links;
            }
        }

        /// <summary>
        /// Log
        /// </summary>
        public MLog Log
        {
            get
            {
                return m_log;
            }
        }

        /// <summary>
        /// слой абстракции БД
        /// </summary>
        public Mary.DatabaseAdapters.MDbAdapterBase DataLayer
        {
            get { return m_dataLayer; }
        }

        /// <summary>
        /// Default cell type, can be Compact, Normal or DelaySave only.
        /// </summary>
        /// <remarks>При старте инициализируется значением Temporary, изменяется пользователем или кодом в процессе работы для управления характеристиками структуры.</remarks>
        public MCellMode DefaultCellMode
        {
            get
            {
                return this.SolutionSettings.ContainerDefaultCellMode;
            }
            set
            {
                this.SolutionSettings.ContainerDefaultCellMode = value;
            }
        }

        /// <summary>
        /// Snapshot manager
        /// </summary>
        public MSnapshot SnapshotManager
        {
            get
            {
                return m_snapMan;
            }
        }

        /// <summary>
        /// Resource manager
        /// </summary>
        public MResource ResourceManager
        {
            get
            {
                return m_resMan;
            }
        }

        /// <summary>
        /// Methods manager
        /// </summary>
        public MMethod MethodManager
        {
            get
            {
                return m_methodMan;
            }
        }

        /// <summary>
        /// Project filesystem manager
        /// </summary>
        public MSolution SolutionManager
        {
            get
            {
                return m_solutionMan;
            }
        }

        /// <summary>
        /// Solution settings 
        /// </summary>
        public MSolutionInfo SolutionSettings
        {
            get { return m_solutionSettings; }
            //set { m_solutionSettings = value; }
        }

#endregion



        #region Serialization function implements from MObject
        /// <summary>
        /// NFT-
        /// </summary>
        /// <param name="writer"></param>
        public override void toBinary(System.IO.BinaryWriter writer)
        {
            Int32 sectionLen = 0;
            
            writer.Write((byte)((int)Serialization.MSerialRecordType.Container)); //1byte
            Int64 beginPos = writer.BaseStream.Position;
            writer.Write(sectionLen); //4byte
            writer.Write(this.ContainerID); //4byte
            writer.Write(this.Name); //string
            writer.Write(this.Description); //string
            writer.Write(this.State.toU64()); //8byte id
            writer.Write(this.isActive);//1byte bool
            writer.Write(this.ServiceFlag);//4byte int32
            writer.Write((byte)(int)this.DefaultCellMode );//1byte enum
            writer.Write((int)this.Log.logDetail );//4byte flags enum
            writer.Write(this.Log.addSymbolicData );//1byte bool
            writer.Write(this.Log.LogFileNumber );//4byte int
            writer.Write(this.DataLayer.Timeout );//4byte int
            writer.Write(this.SnapshotManager.Step );//4byte int //todo:  проверить что это нужно вписывать здесь. Это значение поля MSolutionInfo, который должен входить в шапку файла снимка
            writer.Write((Int16)0); //checksum field must be last field
            //сюда не включены свойства лога: кодировка, разделитель ксв формата.
            Int64 endPos = writer.BaseStream.Position;
            sectionLen = (Int32)(endPos - beginPos);
            //write section length without sectionLen field
            writer.BaseStream.Position = beginPos;
            writer.Write(sectionLen - 4); //4byte
            //crc16
            Int16 crcval = MCrc16.CalculateCrc16FromStream(writer.BaseStream, beginPos, sectionLen - 2);//get bytes from first(section size) to last before crc field
            writer.BaseStream.Position = endPos - 2; //to crc16 field
            writer.Write(crcval);
            //restore end position
            writer.BaseStream.Position = endPos;

            return;
        }
        /// <summary>
        /// NT-Deserialize container from binary stream
        /// </summary>
        /// <param name="reader"></param>
        /// <remarks>
        /// предполагается, что текущая позиция чтения - на дескрипторе секции.
        /// Позиция при выходе из функции - на следующем дескрипторе секции.
        /// Модифицировать переменные, а не проперти, чтобы избежать запросов в БД итп.
        /// </remarks>
        public override void fromBinary(System.IO.BinaryReader reader)
        {
            //read section code
            MSerialRecordType rt = (MSerialRecordType)(int)reader.ReadByte();
            if (rt != MSerialRecordType.Container) throw new Exception("Invalid section type");//serialization error
            //read section length - skip now
            Int64 beginPos = reader.BaseStream.Position;
            reader.BaseStream.Position = reader.BaseStream.Position + 4;
            
            this.ContainerID = reader.ReadInt32();
            this.Name = reader.ReadString();
            this.Description = reader.ReadString();
            this.State = MID.fromU64(reader.ReadUInt64());
            this.isActive = reader.ReadBoolean();
            this.ServiceFlag = reader.ReadInt32();
            this.DefaultCellMode = (MCellMode)(int)reader.ReadByte();
            m_log.logDetail = (MMessageClass)reader.ReadInt32();
            m_log.addSymbolicData = reader.ReadBoolean();
            m_log.LogFileNumber = reader.ReadInt32();
            m_dataLayer.Timeout = reader.ReadInt32();
            m_snapMan.Step = reader.ReadInt32();
            //get crc
            Int64 endPos = reader.BaseStream.Position;
            Int16 crc = reader.ReadInt16();
            //checksum
            Int16 cr = MCrc16.CalculateCrc16FromStream(reader.BaseStream, beginPos, (int)(endPos - beginPos));
            if (cr != crc) throw new Exception("Invalid crc value");
        }
        /// <summary>
        /// NT-
        /// </summary>
        /// <returns></returns>
        public override byte[] toBinaryArray()
        {
            //create memory stream and writer
            MemoryStream ms = new MemoryStream(64);//initial size for data 
            BinaryWriter bw = new BinaryWriter(ms);
            //convert data
            this.toBinary(bw);
            //close memory stream and get bytes
            bw.Close();
            return ms.ToArray();
        }

        public override string toTextString(bool withHex)
        {
            throw new NotImplementedException();
        }

        public override void toText(System.IO.TextWriter writer, bool withHex)
        {
            throw new NotImplementedException();
        }

        public override void fromText(System.IO.TextReader reader)
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// NT-SolutionOpen existing project
        /// </summary>
        /// <param name="projectFilePath">Path of project file to open</param>
        public void SolutionOpen(string projectFilePath)
        {
            //todo: переделать всю функцию и вообще все функции управления солюшеном
            //эта функция должна принимать путь к файлу проекта MSolutionInfo.
            //далее этот файл должен загружаться тут в объект и передаваться всем менеджерам
            //и в том числе самому контейнеру
            //и эти менеджеры создаются при открытии солюшена, так как вне солюшена они не нужны вообще.
            MSolutionInfo projectInfo = MSolutionInfo.Load(projectFilePath);
            //check project version - new code version
            if (projectInfo.CurrentEngineVersion.isCompatibleVersion(projectInfo.SolutionEngineVersion) == false)
                throw new Exception("Solution engine version is not supported by current engine!");
            //todo: Тут надо все переработать - версию движка надо проверять еще при загрузке ФайлСолюшена, а версию проекта (солюшена) реализовать как проперти.
            //Это получается много переделок - я думаю, будет сложно все это связать в один комплект.
            //Также надо переделывать сериализацию и снимки - там используется содержимое файла проекта.
            
            //вписать объект свойств солюшена в объект контейнера
            this.m_solutionSettings = projectInfo;

            //менеджер солюшена
            m_solutionMan = new MSolution(this);
            m_solutionMan.Open(projectInfo);

            //менеджер лога
            m_log = new MLog(this);
            m_log.Open(projectInfo);

            //менеджер снимков
            m_snapMan = new MSnapshot(this);
            m_snapMan.Open(projectInfo);

            //адаптер БД
            //для случая без БД возвращается объект класса MDbAdapterNoDb, который должен притворяться, что работает, но ничего не делать.
            //Это сложно реализовать и проверить, но это есть важная часть моего эксперимента здесь.
            m_dataLayer = MDbAdapterBase.DbSelector(this, projectInfo.DatabaseType);//этот тип данных должен быть взят из объекта MSolutionInfo
            m_dataLayer.Open(projectInfo);//открывает соединение с БД
            //m_dataLayer.ContainerLoad(this); //не загружаем настройки солюшена из БД в контейнер при открытии - только сохраняем, но не загружаем.
            //load max cellid from database
            this.m_maxConstCellID = m_dataLayer.getMaxCellId();

            
            //инициализировать сам контейнер 
            //контейнер должен этими значениями при открытии загружаться из ФайлСолюшена, а не из БД. 
            //Это из-за СолюшенБезБД - там нет БД. 
            //Хотя контейнер в СолюшенБезБД загружается этими значениями из файла снимка.
            //TAG:RENEW-13112017 - проперти контейнера переносят эти данные в этот же объект SolutionInfo, так что здесь ничего не надо делать.
            //this.m_containerId = projectInfo.SolutionId;
            //this.m_DefaultCellMode = projectInfo.ContainerDefaultCellMode;
            //this.m_description = projectInfo.SolutionDescription;
            //this.m_isactive = projectInfo.ContainerIsActiveFlag;
            //this.m_name = projectInfo.SolutionName;
            //this.m_serviceflag = projectInfo.ContainerServiceFlag;
            //this.m_state = MID.fromU64(projectInfo.ContainerState);

            //менеджер ресурсов
            m_resMan = new MResource(this);
            m_resMan.Open(projectInfo);
            //менеджер методов
            m_methodMan = new MMethod(this);
            m_methodMan.Open(projectInfo);


            return;
        }



        /// <summary>
        /// NT-Save all delay-save cells in container
        /// </summary>
        public void SaveAllDelaySavedCells()
        {
            //todo: В каких случаях эта функция должна вызываться извне Движка?
            //Сейчас ее вызывает SolutionSave(), и этого вроде бы должно хватить?
            //PRJNODB: if project without db, return
            if (m_solutionMan.UsesDatabase)
            {
                this.m_cells.SaveCells(MCellMode.DelaySave);
            }
        }

        /// <summary>
        /// NR-Save all temporary cells in container
        /// </summary>
        public void SaveAllTemporaryCells()
        {
            //todo: В каких случаях эта функция должна вызываться извне Движка?
            //Почему она не должна вызываться в  SolutionSave()?
            //PRJNODB: if project without db, return
            if (m_solutionMan.UsesDatabase)
            {
                this.m_cells.SaveCells(MCellMode.Temporary);
            }
        }

#region New project functions - 210213 - уточнение прототипов функций

        ///// <summary>
        ///// NFT-Create new project
        ///// </summary>
        ///// <param name="projectName">New project name, 256 chars max</param>
        ///// <param name="projectDescription">New project description, 8192 chars max</param>
        ///// <param name="rootFolder">Project root folder</param>
        ///// <param name="sqlServerPath">Path to SQLServer if project will use database.  Empty string if project without database.</param>
        ///// <param name="databaseName">Database name. If project not use database, this value ignored. If project use database and this value empty, short project name used fr database name.</param>
        ///// <param name="userLogin">User login. If project not use database, this value ignored. </param>
        ///// <param name="userPassword">User password. If project not use database, this value ignored.</param>
        ///// <param name="timeout">Connecton timeout in seconds. If project not use database, this value ignored.</param>
        //public static void ProjectCreate(string projectName, string projectDescription, string rootFolder, string sqlServerPath, string databaseName, string userLogin, string userPassword, int timeout)
        //{
        //    //1) Create project file object
        //    MProjectFile pf = new MProjectFile();
        //    pf.SolutionName = projectName;
        //    pf.Description = projectDescription;
        //    pf.DatabaseServerPath = sqlServerPath;
        //    pf.DatabaseName = databaseName;
        //    pf.UserName = userLogin;
        //    pf.UserPassword = userPassword;
        //    pf.Timeout = timeout;
        //    //2) Проверка аргументов
        //    pf.checkValues(); //throw some exceptions if one of values invalid.

        //    //3) Проверить, что каталог для проекта существует и доступен на запись. Если это не так, выдать исключение.
        //    DirectoryInfo di = new DirectoryInfo(rootFolder);
        //    if (di.Exists == false) throw new Exception("Invalid root folder");
        //    //create test subfolder and then delete it
        //    DirectoryInfo disub = di.CreateSubdirectory("test");//Здесь будет исключениеUnautorizedAccessException,  если нет прав на создание папки
        //    disub.Delete();

        //    //4) Сейчас входные аргументы проверены и записаны в объект файла проекта. Каталог проекта доступен для работы.
        //    //PRJNODB:
        //    if (pf.IsDBused())
        //    {
        //        //5) Открыть соединение и создать БД проекта. Если будет исключение, то либо параметры соединения неверные,
        //        //либо БД уже существует, либо еще чего.
        //        MDbLayer.DatabaseCreate(pf.DatabaseServerPath, pf.UserName, pf.UserPassword, pf.DatabaseName);
        //        //6) Создать таблицы и индексы БД
        //        MDbLayer.createTablesIndexes(pf.DatabaseServerPath, pf.UserName, pf.UserPassword, pf.DatabaseName);
        //        //7) Создать объект контейнера, инициализировать его данными
        //        //! Проблема! Когда контейнер сохраняется в таблицу, эти операции должны записываться в лог.
        //        //Сейчас файл лога не открыт, да и каталога лога еще нет, поэтому писать в лог нельзя. 
        //        MEngine me = new MEngine();
        //        me.Description = pf.Description;
        //        me.Name = pf.getSolutionName16();
        //        //открыть соединение с БД,
        //        me.DataLayer.ConnectionString = MDbLayer.createConnectionString(pf.DatabaseServerPath, pf.DatabaseName, pf.UserPassword, pf.UserName, 30, false);
        //        me.DataLayer.Timeout = 30;
        //        me.DataLayer.Open(); //! Без лога, или проверять его существование
        //        //сохранить контейнер в бд, закрыть соединение, разрушить объект контейнера.
        //        me.DataLayer.ContainerSave(me); //! Без лога, или проверять его существование
        //        me.DataLayer.Close(); //! Без лога, или проверять его существование
        //        me = null;
        //    }
        //    //8) создать файловую систему проекта
        //    MSolution.CreateSolutionFolder(pf, rootFolder); //! Без лога, или проверять его существование
        //    return;
            
        //}


        
        ///// <summary>
        ///// NT-Create new project
        ///// </summary>
        ///// <param name="rootFolder">Parent directory for project directory</param>
        ///// <param name="info">Информация о создаваемом солюшене</param>
        //public static void ProjectCreate(string rootFolder, MSolutionInfo info)
        //{
        //    //TODO: TAGVERSIONNEW: переработать весь процесс здесь
        //    //2) Проверка параметров солюшена
        //    info.checkValues(); //throw some exceptions if one of values invalid.

        //    //3) Проверить, что каталог для проекта существует и доступен на запись. Если это не так, выдать исключение.
        //    DirectoryInfo di = new DirectoryInfo(rootFolder);
        //    if (di.Exists == false) throw new Exception("Root folder does not exists");
        //    //create test subfolder and then delete it
        //    DirectoryInfo disub = di.CreateSubdirectory("test");//Здесь будет исключениеUnautorizedAccessException,  если нет прав на создание папки
        //    disub.Delete();
        //    //3) Сейчас данные проекта проверены и записаны в объект файла проекта. Каталог для проекта доступен для создания файловой системы проекта.
        //    //8) создать файловую систему проекта
        //    //а почему нельзя сразу попытаться создать каталог проекта, без проверок?
        //    //- если будет неудача, то надо удалить созданные каталог и базу данных - что из этого удалось создать.
        //    //для файловых СУБД нужно сначала создать каталог солюшена, а потом создавать БД
            
        //    MSolution.CreateSolutionFolder(info, rootFolder); //! Без лога, или проверять его существование
        //    //Если здесь будет исключение - например, недостаточно места на диске - то надо удалить БД и перезапустить исключение.
        //    //Если при удалении БД будет исключение - надо его передать вызывающему коду. Тогда пользователь должен вручную удалить и каталог проекта и его БД.


        //    //4) Сейчас входные аргументы проверены и записаны в объект файла проекта. Каталог проекта доступен для работы.
        //    //PRJNODB:
        //    if (info.IsDBused())
        //    {
        //        //5) Открыть соединение и создать БД проекта. Если будет исключение, то либо параметры соединения неверные,
        //        //либо БД уже существует, либо еще чего.
        //        MEngine engine = new MEngine();//откроем временную копию движка без инициализации подсистем
        //        MDbAdapterBase db = MDbAdapterBase.DbSelector(engine, info.DatabaseType);
        //        string connectionString = db.createConnectionString(info.DatabaseServerPath, info.DatabaseName, info.DatabasePortNumber, info.DatabaseTimeout, info.UserName, info.UserPassword, info.UseIntegratedSecurity);
        //        db.DatabaseCreate(connectionString);
        //        //db.createTablesIndexes(...) - это должно быть уже включено в DatabaseCreate()
        //        //и сразу вписать данные контейнера, чтобы потом получать их из таблиц БД, если это зачем-то нужно
        //        //а зачем вообще нужно хранить данные контейнера в БД, если для этого есть файл солюшена?
        //        //- чтобы не потерять их при порче файла солюшена?
        //        //- а если проект без БД, то где они должны тогда храниться?
        //        //- в файле снимка?
        //        //- в общем, эта часть архитектуры движка тоже не проработана 

        //        ////7) Создать объект контейнера, инициализировать его данными
        //        ////! Проблема! Когда контейнер сохраняется в таблицу, эти операции должны записываться в лог.
        //        ////Сейчас файл лога не открыт, да и каталога лога еще нет, поэтому писать в лог нельзя. 
        //        engine.ContainerID = info.SolutionId;
        //        engine.DefaultCellMode = info.ContainerDefaultCellMode;
        //        engine.Description = info.SolutionDescription;
        //        engine.isActive = info.ContainerIsActiveFlag;
        //        engine.Name = info.getSolutionName16();
        //        engine.ServiceFlag = info.ContainerServiceFlag;
        //        engine.State = MID.fromU64(info.ContainerState);
        //        //write to db
        //        //вот неправильный код, но пока ничего лучше нет - надо придумывать общую концепцию для всех этих операций и подсистем проекта
        //        db.ConnectionString = connectionString;
        //        db.Connect();//! Без лога, или проверять его существование
        //        db.ContainerSave(engine);//! Без лога, или проверять его существование
        //        db.Disconnect();//! Без лога, или проверять его существование
        //        db = null;
        //        engine = null;


        //    }
        //    //todo: сохранить файл проекта на диск, если ранее это не было сделано
        //    return;
        //}

        /// <summary>
        /// RT-Create new project
        /// </summary>
        /// <param name="rootFolder">Parent directory for project directory</param>
        /// <param name="info">Информация о создаваемом солюшене</param>
        public void SolutionCreate(string rootFolder, MSolutionInfo info)
        {
            try
            {
                //1 Проверка параметров солюшена
                info.checkValues(); //throw some exceptions if one of values invalid.
                //2 Проверить, что каталог для проекта существует и доступен на запись. Если это не так, выдать исключение.
                DirectoryInfo di = new DirectoryInfo(rootFolder);
                if (di.Exists == false) throw new Exception("Root folder does not exists");
                //2.1 create test subfolder and then delete it
                DirectoryInfo disub = di.CreateSubdirectory("test");//Здесь будет исключениеUnautorizedAccessException,  если нет прав на создание папки
                disub.Delete();

                //3 Сейчас данные проекта проверены и записаны в объект файла проекта. 
                //Каталог для проекта доступен для создания файловой системы проекта.
                //3.1 создать файловую систему проекта и записать в нее ФайлСолюшена
                //а почему нельзя сразу попытаться создать каталог проекта, без проверок?
                //- если будет неудача, то надо удалить созданные каталог и базу данных - что из этого удалось создать.
                //- для файловых СУБД нужно сначала создать каталог солюшена, а потом создавать БД в нем
                //  но создать путь к файлу БД нельзя, пока не выбран адаптер БД, который и определяет этот путь.
                //  поэтому пользователь не должен вписывать путь к БД для файловых СУБД!
                MSolution.CreateSolutionFolder(info, rootFolder); //! Без лога, или проверять его существование
                //Если здесь будет исключение - например, недостаточно места на диске - то надо удалить БД и перезапустить исключение.
                //Если при удалении БД будет исключение - надо его передать вызывающему коду. Тогда пользователь должен вручную удалить и каталог проекта и его БД.

                //Сейчас свойства солюшена проверены и записаны в объект файла солюшена. Каталог солюшена доступен для работы. ФайлСолюшена сохранен на диск.
                //сейчас тут надо вписывать свойства в контейнер так, словно мы его сейчас открываем для использования.
                //но я не уверен, что это лучше, чем просто создать инфраструктуру и затем вызвать функцию открытия солюшена.
                //даже, я думаю, это хуже - два куска кода в разных местах делают одну и ту же работу - это неправильно.
                //Надо просто создать требуемые записи в БД и затем открыть солюшен обычной функцией.

                //4 Открыть соединение и создать БД Солюшена. Если будет исключение, то либо параметры соединения неверные,
                //либо БД с таким именем уже существует на сервере СУБД, либо еще чего.
                //Для файловой БД нужно создать файл БД в каталоге Солюшена
                MDbAdapterBase db = MDbAdapterBase.DbSelector(this, info.DatabaseType);
                //Тут надо бы вписать свойства контейнера в БД, хотя они не должны там храниться, а нужны просто для того, чтобы описывать БД.
                //контейнер же должен этими значениями при открытии загружаться из ФайлСолюшена, а не из БД. Это из-за СолюшенБезБД.
                //создать БД, таблицы и записи в них
                db.DatabaseCreate(info);//создать, открыть, записать и закрыть БД - сделано, не тест.
                db = null;
                //сохранить файл настроек снова, так как в нем теперь прописан новый путь к БД Солюшена.
                info.Save();

                //теперь все это закрываем и открываем солюшен заново как положено
                //this.SolutionOpen(info.SolutionFilePath); - неудачно - открывать лучше отдельным вызовом
                //тут если открытие не удалось, то при удалении бд возникает проблема - файл лога не закрыт.
                //поэтому лучше будет отдельно создавать солюшен, отдельно открывать его. А не как сейчас.
                //сейчас обработчик исключений слишком сложный получается.

            }
            catch (Exception ex)
            {
                //try delete database if exists
                //- это трудно сделать здесь - надо делать это внутри адаптера в функции создания БД
                //try delete project folder
                MSolution.DeleteFolder(info.getCurrentSolutionDirectory());
                //перезапустить исключение теперь для вызывающего кода и пользователя
                throw ex;
            }
            return;
        }

        /// <summary>
        /// NT-Get solution statistics info
        /// </summary>
        /// <returns>Returns solution statistic info</returns>
        /// <remarks>Сейчас это просто алиас для getStatistics(), которую надо заменить этой</remarks>
        public MStatistic SolutionGetStatistics()
        {
            return getStatistics();
        }

        /// <summary>
        /// NT-Saving solution
        /// </summary>
        /// <remarks>
        /// Сохранять проект можно только в устойчивых состояниях процесса.
        /// Ксли проект не использует БД, сохранить проект можно только при помощи моментального снимка.
        /// </remarks>
        public void SolutionSave()
        {
            //PRJNODB: if project uses DB, save container state in db
            //if (this.m_solutionMan.UsesDatabase) - рассчитываем на адаптер без БД
            //{
            m_dataLayer.ContainerSave(this);
            //save MCellBds cells
            SaveAllDelaySavedCells();

            //}
            //TODO: update project statistics in project file
            this.m_solutionSettings.Save();
        }
        /// <summary>
        /// NR-Optimize project
        /// </summary>
        /// <remarks>Основная функция запуска оптимизатора в процессе работы. Пока неясно, что она делает.</remarks>
        public void SolutionOptimize()
        {
            //throw new NotImplementedException();//TODO: Add code here...
        }

        /// <summary>
        /// NT-Close solution
        /// </summary>
        public void SolutionClose(bool withSave)
        {
            if (withSave)
                this.SolutionSave();
            //close database
            m_dataLayer.Close();
            //clear collections
            m_links.Items.Clear();
            m_cells.S1_Clear();

            //TODO: Resources
            m_methodMan.Close();
            m_resMan.Close();
            m_snapMan.Close();
            //close log
            m_log.Close();
        }
        /// <summary>
        /// NT-Clear project. Remove cells and links from memory and database tables. Container name, description and other values not changed.
        /// </summary>
        public void SolutionClear()
        {
            //clear collections
            m_links.Items.Clear();
            m_cells.S1_Clear();
            //clear tables
            //PRJNODB:
            if (m_solutionMan.UsesDatabase)
                this.m_dataLayer.ClearCellTableAndLinkTable();
            //reinit variables
            this.m_maxConstCellID = 0;
            this.m_maxTempCellID = 0;
            this.ServiceFlag = 0;
            this.State = MID.fromU64(0);
            //TODO: update project file - link and cell counters
            //TODO: Resources subsystem - remove all files
            m_resMan.ClearResources();
            //TODO: log subsystem
        }
        /// <summary>
        /// NR-Delete project from filesystem and database server
        /// </summary>
        /// <param name="projectFilePath">Project file pathname</param>
        public static void SolutionDelete(string projectFilePath)
        {
            //load project file
            MSolutionInfo pfile = MSolutionInfo.Load(projectFilePath);
            //PRJNODB: delete database
            //if (pfile.IsDBused())- рассчитываем на адаптер без БД
            //{
                MDbAdapterBase db = MDbAdapterBase.DbSelector(null, pfile.DatabaseType);
                db.DatabaseDelete(pfile);
            //}
            //delete project filesystem
            MSolution.DeleteFolder(pfile.getCurrentSolutionDirectory());  
        }

#endregion

#region Snapshot functions

        /// <summary>
        /// NT-Load full snapshot to container. All previous cells and links must be deleted. 
        /// </summary>
        /// <param name="snapshotFilePathName"></param>
        public void SnapshotFullLoad(string snapshotFilePathName)
        {
            //TODO: проверить, что контейнер пустой перед загрузкой
            this.m_snapMan.LoadFullSnapshot(snapshotFilePathName);

        }

        /// <summary>
        /// NT-Create new snapshot file for current project 
        /// </summary>
        public void SnapshotFullCreate()
        {
            this.m_snapMan.SaveFullSnapshot();

        }


#endregion

#region ID internal functions
        /// <summary>
        /// Get max ID of existing constant cells (from cells table)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Постоянные ячейки всегда существуют в таблице, поэтому там их и считаем.
        /// Если ячеек нет, возвращает 0.
        /// </remarks>
        private int S1_intGetMaxCellConstID()
        {
            if (m_maxConstCellID == 0) m_maxConstCellID = m_dataLayer.getMaxCellId();
            return m_maxConstCellID;
        }
        /// <summary>
        /// Get max ID of existing temporary cells (from cells collection in memory)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Временные ячейки существуют только в памяти, поэтому там их и считаем.
        /// Если ячеек нет, возвращает 0.
        /// </remarks>
        private int S1_intGetMaxCellTempID()
        {
            if (m_maxTempCellID == 0) m_maxTempCellID = m_cells.S1_getMaxTempCellID();
            return m_maxTempCellID;
        }

        /// <summary>
        /// Изменяет значение соответствующего кеша для временной или постоянной ячейки
        /// </summary>
        /// <param name="cellid">Identifier of new cell</param>
        /// <remarks>
        /// Для временной ячейки должен вызываться при создании ячейки около добавления в список ячеек.
        /// Для постоянной ячейки после записи ячейки в БД
        /// </remarks>
        private void S1_ChangeIdCashOnCreateCell(int cellid)
        {
            if(MID.isTemporaryID(cellid))
                m_maxTempCellID = cellid;
            else
                m_maxConstCellID = cellid;
        }
        /// <summary>
        /// Изменяет значение соответствующего кеша для временной ячейки.
        /// </summary>
        /// <param name="cellid">Identifier of new cell</param>
        private void S1_ChangeIdCashOnRemoveTempCell(int cellid)
        {
            //Проверка что ид временных ячеек - вообще-то не должно быть вызовов для постоянных ячеек, поэтому TODO: после отладки движка убрать проверку.
            if (!MID.isTemporaryID(cellid)) throw new Exception("Invalid cell identifier");
            //Если удаляемая ячейка имеет наибольший ИД, сбрасываем кеш для последующего пересчета ИД.
            if (m_maxTempCellID == cellid) m_maxTempCellID = 0;
        }

        /// <summary>
        /// Returns id for new cell without update cash values
        /// Cash values must be updated after succesful cell creation
        /// </summary>
        /// <param name="forTempCell">True for temporary cell, False for constant cell.</param>
        /// <returns></returns>
        internal MID getNewCellId(bool forTempCell)
        {
            int t = 0;
            if (forTempCell)
            {
                t = S1_intGetMaxCellTempID();
                t = MID.getNewTempId(t);
            }
            else
            {
                t = S1_intGetMaxCellConstID();
                t = MID.getNewConstId(t);
            }
            return new MID(t);
        }

        /// <summary>
        /// Set new cash value for cell id's
        /// </summary>
        /// <param name="newId"></param>
        internal void changeIdCashOnCreateCell(MID newId)
        {
            this.S1_ChangeIdCashOnCreateCell(newId.ID);
        }

        /// <summary>
        /// Set new cash value for cell id's
        /// </summary>
        internal void changeIdCashOnRemoveTempCell(MID oldId)
        {
            this.S1_ChangeIdCashOnRemoveTempCell(oldId.ID);
        }

#endregion


#region Cell functions

        /// <summary>
        /// NT-Get first valid cell in project. Return null if no cells in project.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Функция вспомогательная, вызывается только из навигатора, для получения начальной позиции при запуске навигатора</remarks>
        public MCell CellGetAny()
        {
            //get minimal cell in project database or memory
            int id = 0;
            // PRJNODB
            if (m_solutionMan.UsesDatabase)
                id = m_dataLayer.getMinCellId();
            if (id != 0)
            {
                return this.CellGet(new MID(id));
            }
            else
            {
                //get first cell in memory
                return m_cells.getFirstCell();
            }
        }

        /// <summary>
        /// NR-Create container cell with mode. Return cell or throw exception.
        /// If no DB, this function creates Temporary cells 
        /// </summary>
        /// <param name="mode">Cell mode for new cell</param>
        /// <returns></returns>
        /// <remarks>Публичная функция, должна обрабатывать ошибки и выдавать пользователю правильные исключения. Сейчас просто набросок.  </remarks>
        public MCell CellCreate(MCellMode mode)
        {
            //260213 PRJNODB: cellmode = Temporary only
            if (!m_solutionMan.UsesDatabase) mode = MCellMode.Temporary;
            return S1_intCreateCell(mode);
        }

        /// <summary>
        /// NT-Create container cell with mode. Return cell or throw exception
        /// If no DB, this function creates Temporary cells
        /// </summary>
        /// <param name="mode">Cell mode for new cell</param>
        /// <param name="title">Cell title value</param>
        /// <param name="description">Cell descripton value</param>
        /// <returns></returns>
        /// <remarks>Публичная функция, должна обрабатывать ошибки и выдавать пользователю правильные исключения. Сейчас просто набросок.  </remarks>
        public MCell CellCreate(MCellMode mode, String title, String description)
        {
            //TAG:RENEW-13112017
            MCell result = this.CellCreate(mode);
            result.Name = title;
            result.Description = description;

            return result;
        }

        /// <summary>
        /// NT-Получить ячейку по ее идентификатору.
        /// Если ячейка не загружена в контейнер, то она загружается с типом, указанным в DefaultCellMode.
        /// Функция возвращает null, если ячейки с таким идентификатором нет в солюшене.
        /// </summary>
        /// <param name="cellID">cell id</param>
        /// <returns></returns>
        public MCell CellGet(MID cellID)
        {
            return S1_intGetCell(cellID);
        }
        /// <summary>
        /// NT-Get first cell with this title. 
        /// If cell not in memory, load it from table. 
        /// Returns null if cell not exists in project.
        /// </summary>
        /// <param name="cellID">cell id</param>
        /// <returns></returns>
        public MCell CellGet(string title)
        {
            return S1_intGetCell(title);
        }

        /// <summary>
        /// RT-Загрузить ячейку строго указанного типа.
        ///- если ячейки с таким идентификатором нет в контейнере и в БД, функция возвращает нуль.
        ///- если ячейка уже загружена в контейнер, но не указанного типа, функция возвращает нуль.
        ///- если ячейка уже загружена в контейнер и она указанного типа, функция возвращает объект ячейки.
        ///- если ячейка не загружена в контейнер, функция загружает ячейку под указанным типом и возвращает объект ячейки.
        ///- если в процессе работы возникает ошибка, функция выбрасывает исключение.
        /////TAG:RENEW-13112017 - новая функция
        /// </summary>
        /// <param name="cellID">Идентификатор ячейки</param>
        /// <param name="cellMode">Режим ячейки</param>
        /// <returns>Возвращает объект ячейки или null, если не удалось получить ячейку требуемого типа.</returns>
        public MCell CellLoad(MID cellID, MCellMode cellMode)
        {
            //получить ячейку из списка ячеек контейнера
            MCell result = this.Cells.S1_getCell(cellID);
            //если ячейка уже в списке ячеек контейнера,  то 
            //если она нужного типа, возвращаем ее. Иначе возвращаем нуль.
            if (result != null)
            {
                if (result.CellMode == cellMode)
                    return result;
                else return null;
            }
            //если же ее там нет, то пытаемся загрузить ее из БД и сразу нужного типа
            //260213 if PRJNODB skip load from database
            else
            {
                if (cellMode == MCellMode.Temporary)
                    return null;
                else
                {
                    if (this.SolutionManager.UsesDatabase)
                    {
                        result = this.S1_intLoadCell(cellID, cellMode);//функция выбросит исключение при попытке загрузить Temporary ячейку.
                        return result;
                    }
                    else return null;
                }
            }
            //return result; - везде расставлены ретурны
        }

        /// <summary>
        /// NT-Загрузить ячейку строго указанного типа.
        ///- если ячейки с таким идентификатором нет в контейнере и в БД, функция возвращает нуль.
        ///- если ячейка уже загружена в контейнер, но не указанного типа, функция возвращает нуль.
        ///- если ячейка уже загружена в контейнер и она указанного типа, функция возвращает объект ячейки.
        ///- если ячейка не загружена в контейнер, функция загружает ячейку под указанным типом и возвращает объект ячейки.
        ///- если в процессе работы возникает ошибка, функция выбрасывает исключение.
        /////TAG:RENEW-13112017 - новая функция
        /// </summary>
        /// <param name="cellTitle">Название ячейки</param>
        /// <param name="cellMode">Режим ячейки</param>
        /// <returns>Возвращает объект ячейки или null, если не удалось получить ячейку требуемого типа.</returns>
        public MCell CellLoad(string cellTitle, MCellMode cellMode)
        {
            //получить ячейку из списка ячеек контейнера
            MCell result = this.Cells.S1_getCell(cellTitle);
            //если ячейка уже в списке ячеек контейнера,  то 
            //если она нужного типа, возвращаем ее. Иначе возвращаем нуль.
            if (result != null)
            {
                if (result.CellMode == cellMode)
                    return result;
                else return null;
            }
            //если же ее там нет, то пытаемся загрузить ее из БД и сразу нужного типа
            //260213 if PRJNODB skip load from database
            else 
            {
                if (cellMode == MCellMode.Temporary)
                    return null;
                else
                {
                    if (this.SolutionManager.UsesDatabase)
                    {
                        result = this.S1_intLoadCell(cellTitle, cellMode);//функция выбросит исключение при попытке загрузить Temporary ячейку.
                        return result;
                    }
                    else return null;
                }
            }
            //return result; - везде расставлены ретурны
        }

        /// <summary>
        /// NR-Get cell and mark as inactive. Cell not deleted.
        /// </summary>
        /// <param name="cellID">cell id</param>
        public void CellDelete(MID cellID)
        {
            S1_intDeleteCell(cellID);
        }
        /// <summary>
        /// NR-Unload cell from container memory.
        /// See also MCell.Unload()
        /// </summary>
        /// <param name="cellID">cell id</param>
        public void CellUnload(MID cellID)
        {
            S1_intUnloadCell(cellID);
        }

        /// <summary>
        /// NT-Check cell name is unique
        /// </summary>
        /// <param name="cellName">Some name</param>
        /// <returns>Returns True if cell with same name not exists in project, False otherwise.</returns>
        /// <remarks>Функция не изменяет ничего.</remarks>
        public bool CellIsUniqueName(string cellName)
        {
            //check memory
            if (m_cells.S1_containsName(cellName)) return false;
            //check database
            if (m_dataLayer.isCellExists(cellName)) return false;
            return true;
        }

        // internal functions

        /// <summary>
        /// NT-Создать ячейку в контейнере. Возвращает ссылку на ячейку или исключение.
        /// </summary>
        /// <param name="mode">Тип создаваемой ячейки</param>
        /// <returns>Возвращает ссылку на ячейку или исключение.</returns>
        internal MCell S1_intCreateCell(MCellMode mode)
        {
             /*  1)Создается новый идентификатор для ячейки согласно cellMode.
             *  2)Проверяется, что созданный идентификатор допустим.
             *  3)Создается экземпляр ячейки McellB с значениями по умолчанию.
             *  4)Присваивается cellMode и идентификатор.
                5)Если не McellBt,  ячейка добавляется в таблицу.
             *   (Поскольку ячейка McellA не содержит полей, а нужно создать запись в таблице, используем McellB как заготовку.)
                6)Если McellA, берем идентификатор, уничтожаем старую ячейку, создаем McellA, присваиваем идентификатор.
             *  7)Добавляем ячейку в список ячеек контейнера.
             *    Ячейка не имеет связей, так что просто добавляем в список и все.
             *  8)Обновляем значение соответствующего кеша ИД  
             *  9)Возвращаем ссылку на ячейку. 
             *  
             * Обработка ошибок:
             * Функция должна выбрасывать исключения, при этом произведенные изменения должны быть уже отменены.
             * Ошибки:
             * 2 - неправильный идентификатор
             *   Нет изменений для отмены
             * 3 - недостаток памяти
             *   Нет изменений для отмены
             * 5 - ошибка сохранения в БД
             *   Если записи не произошло, нечего отменять. Другие изменения можно откатить внутри функции записи в БД.
             * 6 - недостаток памяти
             *   Надо создать ячейку до записи в БД.
             * 7 - ошибка словаря ячеек
             *   Надо удалить запись из таблицы в обработчике исключения. Хотя это сложно. 
             *   Проще сначала добавить ячейку в список, затем записать в таблицу.
             *   Если при записи будет ошибка, проще удалить ячейку из списка.
             *   
             * Код переделан.
             */

            //1 create cell id - no any global changes
            //можно заменить на вызов internal MID getNewCellId(bool forTempCell) это соответствует концепции идентификаторов, но медленнее. Тогда надо здесь и S1_ChangeIdCashOnCreateCell(cellid) заменить.
            int cellid;
            if (mode == MCellMode.Temporary)
                cellid = MID.getNewTempId(this.S1_intGetMaxCellTempID());
            else
                cellid = MID.getNewConstId(this.S1_intGetMaxCellConstID());
            //2 check cell id and throw exception  - no any global changes
            MID.checkID(cellid); 
            //3 create MCellB object and init by default
            MCellB cell = new MCellB(mode);
            cell.CellID = new MID(cellid); //set cell id
            //6 if MCellA cell required  - no any global changes
            MCell result;
            if (mode == MCellMode.Compact)
            {
                result = new MCellA();
                result.CellID = cell.CellID; //copy cell id value
            }
            else result = cell;
            //7 add cell to container cell list - cell list changed
            this.m_cells.S1_AddCell(result);
            try
            {
                //5 save to database if not temporary cell mode - cell table changed
                if (mode != MCellMode.Temporary) this.m_dataLayer.CellInsert(cell);
            }
            catch (Exception e)
            {
                //Убедиться, что ловится только исключение из this.m_dataLayer.CellInsert(cell);
                //Если это не так, сделать специальный класс исключения и вписать сюда
                //remove cell from cell collection
                this.m_cells.S1_RemoveCell(cell);
                //send exception to caller - чтобы избежать изменения ID cash value, и для информации об ошибке (проверить)
                throw e; 
            }
            //8 change ID cash - id cash changed
            S1_ChangeIdCashOnCreateCell(cellid); //change ID cash value
            //9 return
            return result;
        }

        /// <summary>
        /// NT-Получить ячейку по идентификатору или null если ячейка не найдена в контейнере и в БД.
        /// </summary>
        internal MCell S1_intGetCell(MID cellId)
        {
            /* 1) запросить ячейку из списка ячеек контейнера
             * 2) если ее нет, проверить что ячейка постоянная
             *   если нет, вернуть null
             * 3) если да, загружать ячейку из БД
             * 4) если загружена - вернуть ячейку
             * 5) если не найдена - вернуть null
             */
            //1 get cell from cells collection
            MCell result = this.Cells.S1_getCell(cellId);
            //2 if cell not exists, try load from database
            //260213 if PRJNODB skip load from database
            if ((result == null) && (this.SolutionManager.UsesDatabase))
            {
                //3 if cellid not Temporary, load cell from DB
                if (!cellId.isTemporaryID())
                {
                    result = this.S1_intLoadCell(cellId, this.DefaultCellMode);
                    //4 if cell loaded, return cell. Else return null
                }
            }
            return result;
        }

        /// <summary>
        /// NR-Получить первую же ячейку по названию или null если ячейка не найдена в контейнере и в БД.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private MCell S1_intGetCell(string title)
        {
           //TAG:RENEW-13112017
            /* 1) запросить ячейку из списка ячеек контейнера
             * 2) если ее нет, проверить что ячейка постоянная
             *   если нет, вернуть null
             * 3) если да, загружать ячейку из БД
             * 4) если загружена - вернуть ячейку
             * 5) если не найдена - вернуть null
             */
            //1 get cell from cells collection
            MCell result = this.Cells.S1_getCell(title);
            //2 if cell not exists, try load from database
            //260213 if PRJNODB skip load from database
            if ((result == null) && (this.SolutionManager.UsesDatabase))
            {
                //3 if cellid not Temporary, load cell from DB
                result = this.S1_intLoadCell(title, this.DefaultCellMode);
                
            }
            //4 if cell loaded, return cell. Else return null
            return result;
        }

        /// <summary>
        /// NT-Загрузить ячейку из БД в контейнер. Возвращает ссылку на ячейку или null если ячейка не загружена
        /// //TAG:RENEW-13112017
        /// </summary>
        /// <param name="title"></param>
        /// <param name="cellMode"></param>
        /// <returns></returns>
        private MCell S1_intLoadCell(string title, MCellMode cellMode)
        {
            /* Загрузка ячейки по типам:
 * MCellA - загрузить ячейку 
 * MCellB - загрузить ячейку, получить связи из таблицы, слить со связями из контейнера.
 * MCellBds - как MCellB
 * MCellBt - исключение выдать.
 */
            MCell result = null;
            switch (cellMode)
            {
                case MCellMode.Compact:
                    result = this.DataLayer.CellSelect(title, false);
                    break;
                case MCellMode.Normal:
                case MCellMode.DelaySave:
                    MCellB res = (MCellB)this.DataLayer.CellSelect(title, true);
                    //получить связи из таблицы, слить со связями из контейнера.
                    if (res != null)
                    {
                        //1) получить все связи ячейки из таблицы. 
                        MLinkCollection colD = this.DataLayer.getCellLinks(res.CellID.ID, MAxisDirection.Any);
                        //2) получить все связи ячейки из списка связей контейнера
                        MLinkCollection colM = this.Links.S1_getCellLinks(res.CellID);
                        //3) добавить в выходной список связи из таблицы, если их нет в списке связей из контейнера
                        //связи ячейки, которых нет в контейнере, надо туда добавить. А для этого надо получить разностный список связей.
                        List<MLink> links = colM.getUnicalLinks(colD);
                        // add links to container and to memory links collection
                        this.Links.Items.AddRange(links); //тут связи добавляются в контейнер. Если здесь или дальше будет исключение, нужно откатить добавление связей.
                        colM.Items.AddRange(links);// это будет список связей ячейки
                        res.setLinkCollection(colM);//add links to cell
                    }
                    result = res;
                    break;
                default:
                case MCellMode.Temporary:
                    throw new Exception("Invalid cell mode!");
                //break;
            }
            //устанавливать ссылки на ячейку в связях будем отдельно. Но если нужно, то удобно здесь.
            //add cell to container
            if (result != null) this.Cells.S1_AddCell(result);
            return result;
        }

        /// <summary>
        /// NT-Загрузить ячейку из БД в контейнер. Возвращает ссылку на ячейку или null если ячейка не загружена
        /// </summary>
        /// <remarks>
        /// При исключениях необходимо откатить все изменения и вернуть null.
        /// Если вернуть null не получается, переделать все вызывающие функции для перехвата и обработки исключений.
        /// </remarks>
        internal MCell S1_intLoadCell(MID cellId, MCellMode cellMode)
        {
            /* Загрузка ячейки по типам:
             * MCellA - загрузить ячейку 
             * MCellB - загрузить ячейку, получить связи из таблицы, слить со связями из контейнера.
             * MCellBds - как MCellB
             * MCellBt - исключение выдать.
             */
            MCell result = null;
            switch (cellMode)
            {
                case MCellMode.Compact:
                    result = this.DataLayer.CellSelect(cellId, false);
                    break;
                case MCellMode.Normal:
                case MCellMode.DelaySave:
                    MCellB res = (MCellB)this.DataLayer.CellSelect(cellId, true);
                    //получить связи из таблицы, слить со связями из контейнера.
                    if (res != null)
                    {
                        //1) получить все связи ячейки из таблицы. 
                        MLinkCollection colD = this.DataLayer.getCellLinks(cellId.ID, MAxisDirection.Any);
                        //2) получить все связи ячейки из списка связей контейнера
                        MLinkCollection colM = this.Links.S1_getCellLinks(cellId);
                        //3) добавить в выходной список связи из таблицы, если их нет в списке связей из контейнера
                        //связи ячейки, которых нет в контейнере, надо туда добавить. А для этого надо получить разностный список связей.
                        List<MLink> links = colM.getUnicalLinks(colD);
                        // add links to container and to memory links collection
                        this.Links.Items.AddRange(links); //тут связи добавляются в контейнер. Если здесь или дальше будет исключение, нужно откатить добавление связей.
                        colM.Items.AddRange(links);// это будет список связей ячейки
                        res.setLinkCollection(colM);//add links to cell
                    }
                    result = res;
                    break;
                default:
                case MCellMode.Temporary:
                    throw new Exception("Invalid cell mode!");
                    //break;
            }
            //устанавливать ссылки на ячейку в связях будем отдельно. Но если нужно, то удобно здесь.
            //add cell to container
            if (result != null) this.Cells.S1_AddCell(result);
            return result;
        }

        /// <summary>
        /// NR-Найти и пометить ячейку удаленной. Ячейка загружается в память и помечается удаленной.
        /// </summary>
        internal void S1_intDeleteCell(MID cellId)
        {
            MCell t = this.S1_intGetCell(cellId);
            if (t != null) t.S1_Delete();
            else throw new Exception("Cell not found");
            //TODO: Надо ли выгружать ячейку, если она помечена удаленной?
        }

        /// <summary>
        /// NT-Выгрузить ячейку из памяти по идентификатору. Если ячейка не загружена, ничего не происходит. См. MCell.Unload().
        /// </summary>
        /// <remarks>Этот код лучше исполнять здесь из-за приватных функций контейнера, а MCell.Unload() будет его вызывать. </remarks>
        internal void S1_intUnloadCell(MID cellId)
        {
            //get cell from cell collection
            MCell cell = this.m_cells.S1_getCell(cellId);
            //if cell exists in memory, unload it.
            if (cell != null) this.S1_intUnloadCell(cell);
        }

        /// <summary>
        /// NT-Выгрузить ячейку из памяти. См. MCell.Unload().
        /// </summary>
        /// <remarks>Этот код лучше исполнять здесь из-за приватных функций контейнера, а MCell.Unload() будет его вызывать. </remarks>
        internal void S1_intUnloadCell(MCell cell)
        {
            /*
             * Для McellBds, McellBt - если выгрузка с сохранением, выполняется сохранение,
             * (ячейка превращается в McellB) затем выгрузка как McellB. Остальные типы ячеек не нуждаются в сохранении.
Для McellB типовой процесс:
Получить все связи ячейки. Связи с загруженными ячейками изменить — сбросить ссылки на ячейку. 
             * Связи с незагруженными ячейками удалить из списка связей контейнера.
Незагруженные ячейки — это ячейки, которые отсутствуют в списке ячеек контейнера;
  если в связи отсутствует ссылка на ячейку, это не означает, что ячейка отсутствует в памяти.
Загруженные ячейки присутствуют в списке связей контейнера, могут иметь ссылку на ячейку в связях.
Очистить список связей ячейки. При этом связи, на которые нет ссылок в контейнере или списках других ячеек, будут со временем удалены.
Удалить ячейку из списка контейнера.
McellBt
Для временной ячейки выгрузка эквивалентна полному удалению — отовсюду удаляются все связи с ячейкой и сама ячейка.
McellBds
Ячейка может содержать постоянные и временные связи с другими ячейками. Поскольку ячейка не была сохранена,
временные связи должны быть выгружены (без сохранения), за исключением связей с временными ячейками. 
(Подумать, как могут использоваться временные связи с другими ячейками и как их обрабатывать
     — а как будет использоваться сама ячейка?).
Выгружаются связи ячейки с незагруженными ячейками, выгружается ячейка, обнуляются ссылки на ячейку 
 в списке связей контейнера. ?
McellB
Сбросить все ссылки на ячейку в связях ячейки, выгрузить из памяти (списка контейнера) 
             * все связи этой ячейки с незагруженными в память ячейками, выгрузить ячейку. См выше.
McellA
Сбросить все ссылки на ячейку в списке связей контейнера. Лучше всего проходом по списку связей контейнера.
             * Одновременно? удалить из списка связей контейнера  все связи этой ячейки с незагруженными в память ячейками 
             * (а они там могут быть? Связи McellA-MCellA и McellA-id не хранятся в контейнере, не должны.), 
             * удалить ячейку из списка ячеек контейнера.
            Ячейка может иметь временные связи, их  не выгружать.

             * */
            //пока накидаем кучу по типам, потом переделаем, с учетом исключений
            //Проверить код по диаграмме связей! Что получится при выгрузке каждой из ячеек.
            //И проверить потом на тесте ячеек.
            switch (cell.CellMode)
            {
                case MCellMode.Compact: //MCellA - готово
                    //Сбросить все ссылки на ячейку в списке связей контейнера проходом.
                    this.Links.S1_setCellRefs(cell.CellID, null);
                    //Ячейка может иметь временные связи, их не выгружать - они управляются соотв. ячейкой.
                    //Удалить ячейку из списка ячеек контейнера.
                    this.Cells.S1_RemoveCell(cell);
                    break;
                case MCellMode.Normal: //готово
                    //Получить все связи ячейки - они в списке связей ячейки
                    //Связи с загруженными ячейками изменить — сбросить ссылки на текущую ячейку.
                    //Связи с незагруженными ячейками удалить из списка связей контейнера.
                    //связи с MCellA тоже надо удалять из контейнера
                    //За один проход желательно.
                    foreach (MLink li in cell.Links.Items)
                    {
                        //get id of linked cell
                        MID cid = li.getLinkedCellId(cell.CellID);
                        if (cid != null) 
                        {
                            //связанная ячейка загружена?
                            MCell ce = this.Cells.S1_getCell(cid);
                            //MCellA связи тоже удалять из контейнера
                            if ((ce != null) && (ce.CellMode != MCellMode.Compact))
                                    li.setCellRefsIfExists(cell.CellID, null); //сбросить ссылку на текущую ячейку
                            else
                                this.Links.S1_Remove(li); //удалить из контейнера (медленно?)
                        }
                    }
                    //Очистить список связей ячейки. При этом связи, на которые нет ссылок в контейнере 
                    // или списках других ячеек, будут со временем удалены.
                    cell.Links.Clear();
                    //Удалить ячейку из списка ячеек контейнера.
                    this.Cells.S1_RemoveCell(cell);
                    break;
                case MCellMode.DelaySave: //готово
                    //Получить все связи ячейки - они в списке связей ячейки
                    //Связи с загруженными ячейками:
                    //Временные:
                    // С MCellBds, MCellBt — сбросить ссылки на текущую ячейку. Связи остаются в контейнере и списке связанной ячейки.
                    // C MCellB - удалить из контейнера и связанной ячейки. Поскольку выгружается ячейка, обслуживающая эти связи.
                    // С MCellA - удалить из контейнера. Потому же. 
                    //Постоянные:
                    // С MCellBds, MCellB — сбросить ссылки на текущую ячейку. Связи остаются в контейнере и списке связанной ячейки.
                    // С MCellBt - исключение. Не может быть постоянных связей у временной ячейки.
                    // C MCellA - удалить из контейнера. 
                    //Связи с незагруженными ячейками удалить из списка связей контейнера.
                    foreach (MLink li in cell.Links.Items)
                    {
                        //get id of linked cell
                        MID cid = li.getLinkedCellId(cell.CellID);
                        if (cid != null)
                        {
                            //связанная ячейка загружена?
                            MCell ce = this.Cells.S1_getCell(cid);
                            //MCellA связи тоже удалять из контейнера
                            if (ce != null)
                            {
                                //загруженная ячейка
                                switch (ce.CellMode)
                                {
                                    case MCellMode.Compact:
                                        this.Links.S1_Remove(li);
                                        break;
                                    case MCellMode.Normal:
                                        if (li.isLinkNotTemporary)
                                        {
                                            li.setCellRefsIfExists(cell.CellID, null); //сбросить ссылку на текущую ячейку
                                        }
                                        else
                                        {
                                            this.Links.S1_Remove(li); //незагруженная ячейка. удалить из контейнера (медленно?)
                                            ce.Links.S1_Remove(li);//удалить из связанной ячейки
                                        }
                                        break;
                                    case MCellMode.DelaySave:
                                        li.setCellRefsIfExists(cell.CellID, null); //сбросить ссылку на текущую ячейку
                                        break;
                                    case MCellMode.Temporary:
                                        if (li.isLinkNotTemporary)
                                            throw new Exception("Temporary cell have constant link");
                                        else 
                                            li.setCellRefsIfExists(cell.CellID, null); //сбросить ссылку на текущую ячейку
                                        break;
                                }
                                li.setCellRefsIfExists(cell.CellID, null); //сбросить ссылку на текущую ячейку
                            }
                            else
                                this.Links.S1_Remove(li); //незагруженная ячейка. удалить из контейнера (медленно?)
                        }
                    }
                    //Очистить список связей ячейки. При этом связи, на которые нет ссылок в контейнере 
                    // или списках других ячеек, будут со временем удалены.
                    cell.Links.Clear();
                    //Удалить ячейку из списка ячеек контейнера.
                    this.Cells.S1_RemoveCell(cell);
                    break;
                case MCellMode.Temporary://НЕ ГОТОВО!!!
                    //Удалить связи ячейки из списка ячейки, связанных ячеек и из списка контейнера.
                    //Могут быть временные связи с незагруженными в память ячейками?
                    //Ссылки на ячейку могут быть только во временных связях, а они все удаляются.
                    //Для ускорения процесса хорошо бы сформировать список связанных ячеек, получить их,
                    //и из них выкидывать связи. Но пока можно удалять по одной.
                    //! this.Links не содержит функций для удаления связей!
                    //MCellA - удалить из списка контейнера.
                    //MCellB - удалить из списка контейнера, связанной ячейки
                    //MCellBds - удалить из списка контейнера, связанной ячейки
                    //MCellBt - удалить из списка контейнера, связанной ячейки
                    foreach (MLink li in cell.Links.Items)
                    {
                        if (li.isLinkNotTemporary)
                            throw new Exception("Temporary cell have constant link");
                        else
                        {
                            //get id of linked cell
                            MID cid = li.getLinkedCellId(cell.CellID);
                            if (cid != null)
                            {
                                //связанная ячейка загружена?
                                MCell ce = this.Cells.S1_getCell(cid);
                                if ((ce != null) && (ce.CellMode != MCellMode.Compact))
                                    ce.Links.S1_Remove(li);//удалить из связанной ячейки
                            }
                            this.Links.S1_Remove(li); //удалить из контейнера (медленно?)
                        }
                    }
                    //Очистить список связей ячейки. При этом связи, на которые нет ссылок в контейнере 
                    // или списках других ячеек, будут со временем удалены.
                    cell.Links.Clear();
                    //Удалить ячейку из списка ячеек контейнера.
                    this.Cells.S1_RemoveCell(cell);
                    //если ячейка временная, обновить кеш идентификаторов. Если постоянная, ничего не надо обновлять.
                    S1_ChangeIdCashOnRemoveTempCell(cell.CellID.ID);
                    break;
                default:
                    throw new Exception("Invalid cell mode");
            }
            return;
        }

 
        ///// <summary>
        ///// NT-Get cells meet specified template. Ячейки не добавляются в список в контейнере, это независимый список.
        ///// </summary>
        ///// <param name="tmp">Cell template for search. Если не указать ни одного параметра, ничего не найдется.</param>
        ///// <param name="useLargeCells"> True create MCellB cells for small intensive used set's, False create MCellA cells for big set's</param>
        ///// <returns>Cell collection</returns>
        //public MCellCollection GetCellsInTable(MCellTemplate tmp, bool useLargeCells)
        //{
        //    return m_dataLayer.getCellsByTemplate(tmp, useLargeCells);

        //}
        ///// <summary>
        ///// NT-Get cells meet specified template. Ячейки не добавляются в список в контейнере, это независимый список.
        ///// </summary>
        ///// <param name="tmp">Cell template for search.</param>
        ///// <param name="useLargeCells"> True create MCellB cells for small intensive used set's, False create MCellA cells for big set's</param>
        ///// <returns>Cell collection</returns>
        //public MCellCollection GetCells(MCellTemplate tmp, bool useLargeCells)
        //{
        //    throw new NotImplementedException();

        //}




#endregion 


#region Link functions
        ///// <summary>
        ///// Is active link exists in table or memory?
        ///// </summary>
        ///// <param name="srcCell"></param>
        ///// <param name="dstCell"></param>
        ///// <param name="Axis"></param>
        ///// <param name="axisDir"></param>
        ///// <returns></returns>
        //internal bool containsLink(MCell srcCell, MCell dstCell, uint Axis, MAxisDirection axisDir)
        //{
        //    //find link in table
        //    //find link in memory
        //    MLinkTemplate t = new MLinkTemplate();
        //    t.Axis = Axis;
        //    t.setCellsByDirection(axisDir, srcCell.CellID, dstCell.CellID);
        //    t.isActive = true;
        //    int id = this.DataLayer.getLinkID(t.downCellID.Value, t.upCellID.Value, Axis);
        //    if (id != 0) return true; //link exists in table (but may be inactive!)
        //    else
        //        return this.Links.containsLink(t);
        //}



        /// <summary>
        /// NR-Удалить все связи указанной ячейки из контейнера и связанных ячеек. Упрощенная приблизительная версия.
        /// </summary>
        /// <param name="curCell"></param>
        /// <remarks>Не учитывается MCellA особенности!</remarks>
        internal void S1_RemoveCellLinksFromContainerAndCells(MCell curCell)
        {
            //1 получить количество связей. Если список пустой - выходим.
            int linksCount = curCell.Links.Items.Count; //links count
            if (linksCount == 0) return; //no links
            //2 последовательно проходя по списку связей контейнера, ищем связи с текущей ячейкой.
            //Получаем связанную ячейку и удаляем связь из ее списка связей.
            //Удаляем их по позиции, восстанавливаем позицию, и так до конца списка, если список не пустой.
            //в результате список связей текущей ячейки должен быть пуст. Проверяем. Если это не так, выдаем исключенте.
            int i = 0; 
            int cellid = curCell.CellID.ID; //cash cell id value
            MID targCellId = null;
            while (i < linksCount)
            {
                //find any links to/from current cell
                //Будет работать неправильно для связей ячейки с ней самой
                MLink li = this.Links.Items[i];
                targCellId = null; //as flag for id checking
                if (li.intGetDownId() == cellid)
                {
                    //get terget cell id
                    targCellId = li.upCellID;
                }
                else if (li.intGetUpId() == cellid)
                {
                    //get terget cell id
                    targCellId = li.downCellID;
                }
                //if link matched
                if(targCellId != null)
                {
                    //get target cell if loaded, but not load cell if not loaded
                    MCell tcell = this.Cells.S1_getCell(targCellId);
                    //remove link from cell links list if cell is loaded
                    if (tcell != null) tcell.Links.S1_Remove(li);
                    //remove link from current cell links list
                    curCell.Links.S1_Remove(li);
                    //remove link from container list
                    this.Links.S1_RemoveAt(i);
                    //change links counter for valid enumeration
                    linksCount = this.Links.Items.Count;
                    //Предполагается, что при удалении элемента весь остальной список сдвигается вверх. Однако это может быть не так, нужно проверить.
                }
                else
                {
                    //Link not matched
                    i++;
                }

            }


        }


#endregion

        /// <summary>
        /// Get container description text
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} {1} Cells:{2} Links:{3}", this.ContainerID, this.Name, this.Cells.Items.Count, this.Links.Items.Count );
            return sb.ToString();
        }

        /// <summary>
        /// NT-Get current container and resource statistics
        /// </summary>
        /// <returns></returns>
        internal MStatistic getStatistics()
        {
            MStatistic stat = new MStatistic();
            //todo: можно бы перенести этот код в сам объект статистики, но пока не буду - и так много работы
            //а впрочем - тут быстрее и проще заполнять это все.

            stat.CellsInMemory = this.Cells.Count;
            //PRJNODB: if project has database
            if (this.SolutionManager.UsesDatabase)
            {
                //get number of cells in cell table
                stat.ConstantCells = this.DataLayer.getNumberOfCells();
                //get number of links in link table
                stat.ConstantLinks = this.DataLayer.getNumberOfLinks();
            }
            //пока неясно как работать с внешними ячейками, пока 0
            stat.ExternalCells = 0;
            stat.ExternalLinks = 0;

            stat.LinksInMemory = this.Links.Items.Count;
            stat.TemporaryCells = this.Cells.getNumberOfTempCells();
            stat.TemporaryLinks = this.Links.getNumberOfTempLinks();

            //Statistics from Resource manager - 210213
            this.ResourceManager.GetResourceStatistics(stat);

            return stat;

        }





    }




}

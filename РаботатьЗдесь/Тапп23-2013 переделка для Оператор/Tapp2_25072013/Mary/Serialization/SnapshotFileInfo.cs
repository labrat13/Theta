using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace Mary.Serialization
{
    /// <summary>
    /// Represent snapshot file header for serialization and user browsing
    /// </summary>
    public class MSnapshotFileInfo: MStatistic
    {
        private string m_name;
        private string m_descr;
        //private int m_ver;//engine version
        //private int m_sver;//engine subversion
        private MEngineVersionInfo m_engineVersion;
        //private int m_dver_step;//solution version
        private MSolutionVersionInfo m_solutionVersion;

        private Mary.Serialization.MSnapshotType m_snapshotType;
        /// <summary>
        /// default constructor
        /// </summary>
        public MSnapshotFileInfo()
        {
            m_snapshotType = MSnapshotType.Unknown;
        }



        #region Properties
        /// <summary>
        /// Project name string
        /// </summary>
        [Category("Properties"), Description("Project name")]
        public string ProjectName
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Project description string
        /// </summary>
        [Category("Properties"), Description("Project description")]
        public string ProjectDescription
        {
            get
            {
                return m_descr;
            }
            set
            {
                m_descr = value;
            }
        }

        /// <summary>
        /// Engine version
        /// </summary>
        [Category("Properties"), Description("Engine version number")]
        public MEngineVersionInfo EngineVersion
        {
            get
            {
                return m_engineVersion;
            }
            set
            {
                m_engineVersion = value;
            }
        }

        ///// <summary>
        ///// Engine subversion
        ///// </summary>
        //[Category("Properties"), Description("Engine subversion number")]
        //public int EngineSubVersion
        //{
        //    get
        //    {
        //        return m_sver;
        //    }
        //    set
        //    {
        //        m_sver = value;
        //    }
        //}

        /// <summary>
        /// Data version number
        /// </summary>
        [Category("Properties"), Description("Project data structure version number ")]
        public MSolutionVersionInfo DataVersion
        {
            get
            {
                return this.m_solutionVersion;
            }
            set
            {
                this.m_solutionVersion = value;
            }
        }

        /// <summary>
        /// Snapshot type
        /// </summary>
        [Category("Properties"), Description("Snapshot type")]
        public Serialization.MSnapshotType SnapshotType
        {
            get
            {
                return m_snapshotType;
            }
            set
            {
                m_snapshotType = value;
            }
        }

        #endregion

        /// <summary>
        /// NT-Return snapshot file header information
        /// </summary>
        /// <param name="pathname">snapshot file pathname</param>
        /// <returns></returns>
        public static MSnapshotFileInfo Load(string pathname)
        {
            MSnapshotFileInfo info = null;
            BinaryReader br = null;
            //open file
            br = new BinaryReader(File.Open(pathname, FileMode.Open, FileAccess.Read, FileShare.Read));
            info = new MSnapshotFileInfo();
            //read snapshot file header
            info.LoadHeader(br);
            //close stream
            br.Close();
            return info;
        }

        /// <summary>
        /// NT-Load snapshot file header. Set file position to next section descriptor.
        /// </summary>
        /// <param name="reader"></param>
        /// <remarks>После возврата из функции позиция чтения файла - на дескрипторе следующей секции</remarks>
        internal void LoadHeader(BinaryReader reader)
        {
            Int64 beginPos = 0;
            Int64 endPos = 0;
            //read signature
            UInt64 signature = reader.ReadUInt64();
            if (signature != MSnapshot.SnapshotFileSignature)
                throw new Exception("Invalid snapshot file signature");

            //else signature valid
            beginPos = reader.BaseStream.Position;
            //read section size int32
            int sectionSize = reader.ReadInt32();
            //check crc 
            endPos = beginPos + sectionSize + 4 - 2; //to crc value
            reader.BaseStream.Position = endPos;
            short crc = reader.ReadInt16();
            //calc and compare checksum and set validity flag
            short cr = MCrc16.CalculateCrc16FromStream(reader.BaseStream, beginPos, (int)(endPos - beginPos));
            if(crc != cr)
                throw new Exception("Invalid header checksum");
            //restore file position
            reader.BaseStream.Position = beginPos + 4;//to after sectionsize
            //read num of cells in memory int32
            this.m_cellsmem = reader.ReadInt32();
            //read num of cells in table int32
            this.m_cellsconst = reader.ReadInt32();
            //read num of temp cells  int32
            this.m_cellstemp = reader.ReadInt32();
            //read num of refcells int32
            this.m_cellsext = reader.ReadInt32();
            //read num of links in memory int32
            this.m_linksmem = reader.ReadInt32();
            //read num of links in table
            this.m_linksconst = reader.ReadInt32();
            //read num of temp links  int32
            this.m_linkstemp = reader.ReadInt32();
            //read num of external links  int32
            this.m_linksext = reader.ReadInt32();
            //read num of resource files - 210213
            this.m_resourcefiles = reader.ReadInt32();
            //read size of resource files - 210213
            this.m_resourcesize = reader.ReadInt64();
            //read engine version int
            this.m_engineVersion.fromBinary(reader);//TODO: TAGVERSIONNEW: done
            ////subversion int
            //this.m_sver = reader.ReadInt32();//TODO: TAGVERSIONNEW: - входит в engineVersion
            //step int
            this.m_solutionVersion.fromBinary(reader);//TODO: TAGVERSIONNEW: done
            //snapshot type byte-int-enum
            this.m_snapshotType = (MSnapshotType)(int)reader.ReadByte();
            //project name string
            this.m_name = reader.ReadString();
            //project descr string
            this.m_descr = reader.ReadString();
            //skip checksum 2 byte
            reader.BaseStream.Position = reader.BaseStream.Position + 2;  
        }

        /// <summary>
        /// NT - write snapshot file header from current info
        /// </summary>
        /// <param name="writer"></param>
        /// <remarks>После возврата из функции позиция файла - на дескрипторе следующей секции</remarks>
        internal void SaveHeader(BinaryWriter writer)
        {
            Int32 sectionLength; //length of section
            Int64 beginPos;//writing point for section length storing
            Int64 endPos;//writing point for section length storing
            Int16 crcval; //crc value

            //записать шапку снимка
            //1 signature 8 bytes
            writer.Write(MSnapshot.SnapshotFileSignature);
            //get file position for crc
            beginPos = writer.BaseStream.Position;
            //2 section size 4 bytes - include crc but not include signature and length
            writer.Write((Int32)0);
            //3 num of cells in memory - 4 bytes
            writer.Write(this.m_cellsmem);
            //4 num of cells in table - 4 bytes
            writer.Write(this.m_cellsconst);
            //5 num of temp cells  - 4 bytes
            writer.Write(this.m_cellstemp);
            //6 num of cells in table - 4 bytes
            writer.Write(this.m_cellsext);
            //7 num of links in memory - 4 bytes
            writer.Write(this.m_linksmem);
            //8 num of links in table - 4 bytes
            writer.Write(this.m_linksconst);
            //9 num of temp links  - 4 bytes
            writer.Write(this.m_linkstemp);
            //10 num of external links  - 4 bytes
            writer.Write(this.m_linksext);
            //11 Resource files count - 210213
            writer.Write(this.m_resourcefiles);
            //12 Resource files size - 210213
            writer.Write(this.m_resourcesize);
            //13 engine version
            this.m_engineVersion.toBinary(writer);//TODO: TAGVERSIONNEW: done
            ////14 SubVersion 
            //writer.Write(this.m_sver);//на всякий случай запишем//TODO: TAGVERSIONNEW: - входит в engineVersion
            //15 solution version 
            this.m_solutionVersion.toBinary(writer);//TODO: TAGVERSIONNEW: done
            //16 snapshot type
            writer.Write((byte)((int) m_snapshotType));
            //17 project name string
            writer.Write(this.m_name);
            //18 project description string
            writer.Write(this.m_descr);
            endPos = writer.BaseStream.Position;
            //overwrite section length
            writer.BaseStream.Position = beginPos;
            sectionLength  = (Int32)(endPos + 2 - 4 - beginPos);
            writer.Write(sectionLength); //len with crc field but without sectionsize field
            writer.BaseStream.Position = endPos; //before crc field
            //19 write crc
            crcval = MCrc16.CalculateCrc16FromStream(writer.BaseStream, beginPos, (Int32)(endPos - beginPos));
            writer.Write(crcval); //checksum
            //...еще чего для шапки?
            return;
        }


        /// <summary>
        /// NT - get container statistic, other values for header
        /// </summary>
        /// <param name="mEngine"></param>
        internal void getContainerState(MEngine ct)
        {
            MStatistic stat = ct.getStatistics();
            //Cells links stat
            this.m_cellsconst = stat.ConstantCells;
            this.m_cellsext = stat.ExternalCells;
            this.m_cellsmem = stat.CellsInMemory;
            this.m_cellstemp = stat.TemporaryCells;
            this.m_linksconst = stat.ConstantLinks;
            this.m_linksext = stat.ExternalLinks;
            this.m_linksmem = stat.LinksInMemory;
            this.m_linkstemp = stat.TemporaryLinks;
            //Resources stat - 210213
            this.m_resourcefiles = stat.ResourceFiles;
            this.m_resourcesize = stat.ResourceSize;
            //snapshot info
            this.m_name = ct.SolutionSettings.SolutionName;//TAG:RENEW-13112017
            this.m_snapshotType = MSnapshotType.Unknown;
            //this.m_sver = 0;//TODO: TAGVERSIONNEW: - в объект надо вставить поля версий солюшена и движка, но это много переделок.
            //this.m_ver = 0;//TODO: TAGVERSIONNEW: - в объект надо вставить поля версий солюшена и движка, но это много переделок. 
            this.m_engineVersion = ct.SolutionSettings.SolutionEngineVersion;
            this.m_descr = ct.SolutionSettings.SolutionDescription;
            this.m_solutionVersion = ct.SolutionSettings.SolutionVersion;//TODO: TAGVERSIONNEW: уточнить, какие поля действительно нужны здесь. Нельзя ли выводить в снимок весь ФайлСолюшена вместо копирования данных из него?

            return;
        }
    }
}

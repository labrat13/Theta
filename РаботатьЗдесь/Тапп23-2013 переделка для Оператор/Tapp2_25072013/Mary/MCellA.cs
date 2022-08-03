using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mary.Serialization;

namespace Mary
{
    /// <summary>
    /// Представляет оболочку для доступа к ячейке в БД
    /// </summary>
    public class MCellA : MCell
    {
        /// <summary>
        /// Идентификатор ячейки. Остальные данные читаются из таблицы ячеек
        /// </summary>
        private int m_cellid;

        /// <summary>
        /// Normal constructor
        /// </summary>
        public MCellA()
        {
            m_cellid = 0;
        }

#region Properties
        /// <summary>
        /// Cell description string
        /// </summary>
        public override string Description
        {
            get
            {
                return MCell.Container.DataLayer.getCellDescription(m_cellid);
            }
            set
            {
                MCell.Container.DataLayer.setCellDescription(m_cellid, value);
            }
        }

        public override bool isActive
        {
            get
            {
                return MCell.Container.DataLayer.getCellActive(m_cellid);
            }
            set
            {
                MCell.Container.DataLayer.setCellActive(m_cellid, value);
            }
        }

        public override int ServiceFlag
        {
            get
            {
                return MCell.Container.DataLayer.getCellServiceFlag(m_cellid);
            }
            set
            {
                MCell.Container.DataLayer.setCellServiceFlag(m_cellid, value);
            }
        }

        public override MID State
        {
            get
            {
                return MCell.Container.DataLayer.getCellState(m_cellid);
            }
            set
            {
                MCell.Container.DataLayer.setCellState(m_cellid, value);
            }
        }
        /// <summary>
        /// Cell name
        /// </summary>
        public override String Name
        {
            get
            {
                return MCell.Container.DataLayer.getCellName(m_cellid);
            }
            set
            {
                MCell.Container.DataLayer.setCellName(m_cellid, value);
            }
        }

        /// <summary>
        /// Cell type id
        /// </summary>
        public override MID TypeId
        {
            get
            {
                return MCell.Container.DataLayer.getCellTypeId(m_cellid);
            }
            set
            {
                MCell.Container.DataLayer.setCellTypeId(m_cellid, value);
            }
        }

        /// <summary>
        /// Cell creation timestamp
        /// </summary>
        public override DateTime CreaTime
        {
            get
            {
                return MCell.Container.DataLayer.getCellCreationTime(m_cellid);
            }
            //internal set
            //{
            //    MCell.Container.DataLayer.setCellCreationTime(m_cellid, value);
            //}
        }

        /// <summary>
        /// Last modification timestamp. Any changes in other field refresh timestamp here automatically.
        /// </summary>
        public override DateTime ModiTime
        {
            get
            {
                return MCell.Container.DataLayer.getCellModificationTime(m_cellid);
            }
            //internal set
            //{
            //    //MCell.Container.DataLayer.setCellModificationTime(m_cellid, value); //throw SqlException, just skip it
            //}
        }

        /// <summary>
        /// Cell is read-only flag
        /// </summary>
        public override  bool ReadOnly
        {
            get
            {
                return MCell.Container.DataLayer.getCellReadOnly(m_cellid);
            }
            set
            {
                MCell.Container.DataLayer.setCellReadOnly(m_cellid, value);
            }
        }

        /// <summary>
        /// Cell data value
        /// </summary>
        public override byte[] Value
        {
            get
            {
                return MCell.Container.DataLayer.getCellValue(m_cellid);
            }
            set
            {
                MCell.Container.DataLayer.setCellValue(m_cellid, value);
            }
        }

        /// <summary>
        /// Cell data value type id
        /// </summary>
        public override MID ValueTypeId
        {
            get
            {
                return MCell.Container.DataLayer.getCellValueTypeId(m_cellid);
            }
            set
            {
                MCell.Container.DataLayer.setCellValueTypeId(m_cellid, value);
            }
        }

        /// <summary>
        /// One-time link collection. 
        /// </summary>
        public override MLinkCollection Links
        {
            get
            {
                return this.AssemblyMCellALinks(); //Для MCellA это целое большое приключение - сборка одноразового списка.
            }
        }



        /// <summary>
        /// Cell saving mode
        /// </summary>
        public override MCellMode CellMode
        {
            get
            {
                return MCellMode.Compact;//other not supported   
            }
            //internal set
            //{
            //    if (value != MCellMode.Compact)
            //        throw new Exception("MCellA cannot change cell save mode");
            //}
        }

        /// <summary>
        /// идентификатор ячейки в контейнере.
        /// </summary>
        public override MID CellID
        {
            get
            {
                return new MID(m_cellid);
            }
            set { m_cellid = value.ID; }
        }

        /// <summary>
        /// Current cell is MCellB cell?
        /// </summary>
        public override bool isLargeCell
        {
            get { return false; }
        }
#endregion


        #region Serialization function implements from MObject

        /// <summary>
        /// NT-Write cell record to serialization stream
        /// </summary>
        /// <param name="writer">serialization stream</param>
        public override void toBinary(System.IO.BinaryWriter writer)
        {
            //получить из таблицы всю ячейку разом.
            MCellB tc = (MCellB)MCell.Container.DataLayer.CellSelect(this.CellID, true);//get cell values
            tc.toBinarySub(writer, MCellMode.Compact);//serialize with current cellmode
        }
        /// <summary>
        /// NT-Deserialize container from binary stream
        /// </summary>
        /// <param name="reader"></param>
        /// <remarks>
        /// предполагается, что текущая позиция чтения - на дескрипторе секции.
        /// Позиция при выходе из функции - на следующем дескрипторе секции.
        /// Модифицировать переменные, а не проперти, чтобы избежать запросов в БД итп.
        /// Для MCellA читаем только cellid. Остальное не хранится, а получается из таблицы.
        /// </remarks>
        public override void fromBinary(System.IO.BinaryReader reader)
        {
            //check section descriptor
            MSerialRecordType rt = (MSerialRecordType)(int)reader.ReadByte();
            if (rt != MSerialRecordType.Cell) throw new Exception("Invalid section type");//serialization error
            //read section length 4bytes
            Int64 beginPos = reader.BaseStream.Position;//for crc check
            int SectionLen = reader.ReadInt32();
            //read cellid only
            this.CellID = MID.fromU64(reader.ReadUInt64());//cellID 8b
            //check crc
            Int64 endPos = beginPos + SectionLen + 4 - 2; //before crc
            reader.BaseStream.Position = endPos;//set position before crc
            Int16 crc = reader.ReadInt16();
            //checksum
            Int16 cr = MCrc16.CalculateCrc16FromStream(reader.BaseStream, beginPos, (int)(endPos - beginPos));
            if (cr != crc) throw new Exception("Invalid crc value");
        }

        /// <summary>
        /// NT-serialize cell to binary array
        /// </summary>
        /// <returns></returns>
        public override byte[] toBinaryArray()
        {
            //create memory stream and writer
            MemoryStream ms = new MemoryStream(64);//initial size for cell data 
            BinaryWriter bw = new BinaryWriter(ms, Encoding.Unicode);
            //convert data
            toBinary(bw);
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
        /// NT- Сохранить ячейку и связи в БД. Для Compact ячеек ничего не делает.
        /// </summary>
        public override void S1_Save()
        {
            //ничего не делать здесь
            return;
        }

        /// <summary>
        /// помечает ячейку удаленной. 
        /// </summary>
        public override void S1_Delete()
        {
            //mark cell as deleted
            this.isActive = false;
            //еще чего-то?

        }

        ///// <summary>
        ///// выгружает ячейку из памяти, когда она больше не нужна. При выгрузке надо просмотреть список связей ячейки, сбросить все ссылки на ячейку, выгрузить из памяти также все связи ячейки с незагруженными в память ячейками.
        ///// </summary>
        //public override void S1_Unload()
        //{
        //}


        /// <summary>
        /// NFT-Create complete list of links for MCellA links colection
        /// </summary>
        /// <returns></returns>
        internal MLinkCollection AssemblyMCellALinks()
        {
            //Глобального ничего не изменяется, поэтому откатывать нечего, исключения не проверяем.
            //1) получить все связи ячейки из таблицы. 
            MLinkCollection colD = MCell.Container.DataLayer.getCellLinks(m_cellid, MAxisDirection.Any);
            //2) получить все связи ячейки из списка связей контейнера
            MLinkCollection colM = MCell.Container.Links.S1_getCellLinks(this.CellID);
            //3) добавить в выходной список связи из таблицы, если их нет в списке связей из контейнера
            //Если один из списков пустой, возвращаем другой.
            List<MLink> links = colM.getUnicalLinks(colD); //слияние списков без дополнительной обработки. Если надо допиливать, нужен другой код.
            colM.Items.AddRange(links);
            //4) вернуть выходной список
            return colM;
        }

        /// <summary>
        /// NT-Получить текстовое представление ячейки
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String s = null;
            MCellB tmpCell;
            try
            {
                //получить из таблицы всю ячейку разом.
                tmpCell = (MCellB) MCell.Container.DataLayer.CellSelect(this.CellID, true);//get cell values
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("ID={0} Name={1} Mode={2} ", this.CellID.ToString(), tmpCell.Name, this.CellMode.ToString());
                sb.AppendFormat("Type={0} Data", tmpCell.TypeId.ToString());
                sb.Append("("); sb.Append(tmpCell.ValueTypeId.ToString()); sb.Append(")");
                sb.Append("["); sb.Append(tmpCell.Value.Length.ToString()); sb.Append("]");                    
                //sb.AppendFormat(" Links{0}", this.Links.Items.Count); disable - too many work for MCellA
                s = sb.ToString();
            }
            catch (Exception ex)
            {
                s = ex.GetType().Name;  
            }
            return s;

        }


        


    }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace Mary.Serialization
{
    /// <summary>
    /// Тип записи в сериализованном снимке структуры
    /// </summary>
    public enum MSerialRecordType
    {
        /// <summary>
        /// Unknown record type
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// File header record
        /// </summary>
        Header = 1,
        /// <summary>
        /// Container  record
        /// </summary>
        Container = 2,
        /// <summary>
        /// Cell record for cells in memory
        /// </summary>
        MemoryCellSection = 3,
        /// <summary>
        /// CellSection record for cells in table
        /// </summary>
        TableCellSection = 4,
        /// <summary>
        /// Referenced cells section record
        /// </summary>
        /// <remarks>In partial snapshot ths section contains referenced cells. 
        /// For full snapshot, this section contains external cells.</remarks>
        RefCellSection = 5,
        /// <summary>
        /// LinkSection record
        /// </summary>
        MemoryLinkSection = 6,
        /// <summary>
        /// LinkSection record
        /// </summary>
        TableLinkSection = 7,
        /// <summary>
        /// Cell record
        /// </summary>
        Cell = 8,
        /// <summary>
        /// Link record
        /// </summary>
        Link = 9,
        /// <summary>
        /// File end record
        /// </summary>
        Footer = 10,
        /// <summary>
        /// Combined link section for memory and table link 
        /// </summary>
        LinkSection = 11,
    }
}

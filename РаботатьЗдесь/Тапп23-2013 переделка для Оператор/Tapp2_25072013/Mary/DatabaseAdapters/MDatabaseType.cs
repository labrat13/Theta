using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    /// <summary>
    /// Обозначает тип БД проекта.
    /// </summary>
    /// <remarks></remarks>
    /// <seealso cref=""/>
    public enum MDatabaseType
    {
        /// <summary>
        /// Unknown database type
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// No database in project
        /// </summary>
        NoDatabase = 1,
        /// <summary>
        /// MsSql2005 database
        /// </summary>
        MicrosoftSqlServer2005 = 2,
        /// <summary>
        /// MySql5 database
        /// </summary>
        MySql5 = 3,

        MsAccess = 4,
        
        Sqlite3 = 5,

    }
}

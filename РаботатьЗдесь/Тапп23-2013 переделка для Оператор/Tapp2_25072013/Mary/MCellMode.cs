using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    /// <summary>
    /// Cell work mode: Compact, Normal, DelaySave or Temporary.
    /// </summary>
    /// <remarks>Значения элементов используются при выборе приоритетной ячейки при создании связи. Не менять без необходимости!</remarks>
    public enum MCellMode
    {
         /// <summary>
        /// MCellA cell type
        /// </summary>
        Compact = 0,
        /// <summary>
        /// MCellB cell type. Normal cell mode, all changes immediately save in cell table
        /// </summary>
        Normal = 1,
        /// <summary>
        /// MCellBds cell type. Запись изменений откладывается до вызова Save(). Ячейка автоматически сохраняется при создании и при выгрузке из памяти.
        /// </summary>
        DelaySave = 2,
        /// <summary>
        /// MCellBt cell type. Временная ячейка. Запись ячейки в таблицу производится только вызовом Save(), после которого ячейка переходит в состояние Normal. 
        /// При создании ячейка не записывается в таблицу.
        /// </summary>
        Temporary = 3,

    }
}

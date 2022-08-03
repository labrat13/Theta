using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    public class MCellCollection
    {
        private Dictionary<int, MCell> m_items;

        /// <summary>
        /// NT-Constructor
        /// </summary>
        public MCellCollection()
        {
            m_items = new Dictionary<int, MCell>();
        }

        /// <summary>
        ///  NT-Get cell collection in dictionary
        /// </summary>
        public Dictionary<int, MCell>.ValueCollection Items
        {
            get
            {
                return m_items.Values;
            }
        }

        /// <summary>
        /// Get count of cells in collection
        /// </summary>
        public int Count
        {
            get { return m_items.Count; }
        }

        /// <summary>
        /// NT-Проверяет, что список содержит ячейку с указанным именем. 
        /// Аналогично можно запросить список ячеек по имени. 
        /// Только тут быстрее, поскольку не надо формировать список возвращаемых ячеек.
        /// Нужна для имен, которым положено быть уникальными (имена типов, например).
        /// </summary>
        /// <param name="name"></param>
        public bool S1_containsName(string name)
        {
            foreach (KeyValuePair<int, MCell> kvp in m_items)
            {
                if (kvp.Value.Name == name) return true;
            }
            return false;
        }

        /// <summary>
        /// NT-Проверяет, что список содержит ячейку с указанным идентификатором. Для проверки наличия ячейки в списке перед загрузкой из БД.
        /// </summary>
        public bool S1_containsCell(MID cellid)
        {
            return m_items.ContainsKey(cellid.ID);
        }

        /// <summary>
        /// NT-получить ячейку по идентификатору, null if not exists
        /// </summary>
        public MCell S1_getCell(MID cellid)
        {
            MCell t;
            m_items.TryGetValue(cellid.ID, out t);
            return t;
        }

        /// <summary>
        /// NT-получить ячейку по названию, null if not exists
        /// //TAG:RENEW-13112017
        /// </summary>
        /// <param name="title">Cell title</param>
        /// <returns></returns>
        public MCell S1_getCell(string title)
        {
            MCell result = null;
            foreach (KeyValuePair<int, MCell> kvp in m_items)
            {
                if (String.Equals(kvp.Value.Name, title, StringComparison.Ordinal))
                {
                    result = kvp.Value;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// NT-Получить список ячеек по значению свойств, описанных в ячейке-шаблоне. Все поля в шаблоне, не содержащие null, используются для отбора ячеек. 
        /// </summary>
        /// <param name="template">Шаблон ячейки. Если ни одно поле не указано, все ячейки будут включены в результат</param>
        /// <returns>Cell collection</returns>
        /// <remarks>Эта универсальная функция работает медленно, и может быть заменена на упрощенные версии при оптимизации движка. Но для прототипа она хороша.</remarks>
        public MCellCollection S1_getCells(MCellTemplate template)
        {
            MCellCollection mc = new MCellCollection();
            MCell t;
            foreach (KeyValuePair<int, MCell> kvp in m_items)
            {
                t = kvp.Value;
                if (template.CellMatch(t) == true)
                    mc.S1_AddCell(t);
            }
            return mc;
        }

        ///// <summary>
        ///// NT-получить имя ячейки по идентификатору. Быстрая функция. возвращает нуль если ячейки нет в списке.
        ///// </summary>
        ///// <remarks>
        ///// Все равно надо сначала получить ячейку. Эта функция для SQL нужна - там долго ячейку доставать,
        ///// а в памяти оно нафиг.
        ///// </remarks>
        //public string S1_getNameByID(MID cellid)
        //{
        //    MCell m = m_items[cellid.ID];
        //    if (m != null) return m.Name;
        //    else return null;
        //}

        /// <summary>
        /// NT-Добавить ячейку в словарь.
        /// Добавляет запись KeyValuePair в словарь.
        /// </summary>
        /// <exception cref="ArgumentException">Cell already exists</exception>
        public void S1_AddCell(MCell cell)
        {
            m_items.Add(cell.CellID.ID, cell);
        }

        /// <summary>
        /// NT-Удалить ячейку из словаря
        /// </summary>
        public void S1_RemoveCell(MCell cell)
        {
            S1_RemoveCell(cell.CellID);
        }

        /// <summary>
        /// NT-Удалить ячейку из словаря.
        /// Удаляет запись словаря с указанным ключом.
        /// </summary>
        public void S1_RemoveCell(MID cellid)
        {
            m_items.Remove(cellid.ID);
        }
        /// <summary>
        /// NT-удалить все ячейки.
        /// Удаляет все записи из словаря.
        /// </summary>
        public void S1_Clear()
        {
            m_items.Clear();
        }

        /// <summary>
        /// Get max ID of temporary cells in collection
        /// </summary>
        /// <returns></returns>
        internal int S1_getMaxTempCellID()
        {
            // Временные идентификаторы меньше 0 и растут в отрицательную сторону. Поэтому ищем минимальный идентификатор.
            int min = 0;
            foreach (KeyValuePair<int, MCell> kvp in m_items)
                if (min > kvp.Key) min = kvp.Key;
            return min;
        }
        /// <summary>
        /// NT-get number of temp cells in dictionary
        /// </summary>
        /// <returns></returns>
        internal int getNumberOfTempCells()
        {
            //Временные идентификаторы меньше 0
            int cnt = 0;
            foreach (KeyValuePair<int, MCell> kvp in m_items)
                if (kvp.Key < 0) cnt++;
            return cnt;
        }

        /// <summary>
        /// Save all cells with specified cell type
        /// </summary>
        /// <param name="mCellMode"></param>
        internal void SaveCells(MCellMode cellMode)
        {
            foreach (KeyValuePair<int, MCell> kvp in m_items)
                if (kvp.Value.CellMode == cellMode)
                    kvp.Value.S1_Save();
            return;
        }
        /// <summary>
        /// Return first cell in collection or null if no cells
        /// </summary>
        /// <returns></returns>
        internal MCell getFirstCell()
        {
            //return first cell in dictionary
            foreach (KeyValuePair<int, MCell> kvp in m_items)
                return kvp.Value;
            //return null if no cells
            return null;
        }

    }
}

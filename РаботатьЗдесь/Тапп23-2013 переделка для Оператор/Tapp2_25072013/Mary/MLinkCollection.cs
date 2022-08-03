using System;
using System.Collections.Generic;
using System.Text;

namespace Mary
{
    public class MLinkCollection
    {
        private List<MLink> m_items;
        /// <summary>
        /// Constructor
        /// </summary>
        public MLinkCollection()
        {
            m_items = new List<MLink>();
        }

        /// <summary>
        /// Get list of links in collection
        /// </summary>
        public List<MLink> Items
        {
            get
            {
                return m_items;
            }
        }

        /// <summary>
        /// NT-Получить список связей по значению свойств, описанных в связи-шаблоне. 
        /// Все поля в шаблоне, не содержащие null, используются для отбора связей.
        /// </summary>
        /// <param name="template">Шаблон связи. X AND Y. Если ни одно поле не указано, все связи будут включены в результат</param>
        public MLinkCollection getLinks(MLinkTemplate template)
        {
            MLinkCollection col = new MLinkCollection();
            foreach (MLink li in m_items)
            {
                if (template.isLinkMatch(li) == true)
                    col.AddLink(li);
            }
            return col;
        }

        /// <summary>
        /// NT-Получить список связей по значению свойств
        /// 23122017-новая функция, должна быть быстрее, чем с шаблоном MLinkTemplate
        /// </summary>
        /// <param name="axis">Тип связи</param>
        /// <param name="axisDirection">направление связи: Up, Down, Any</param>
        /// <param name="currentCellId">Идентификатор текущей ячейки</param>
        /// <param name="targetCellId">Идентификатор конечной ячейки</param>
        /// <param name="activeOnly">True - возвращать только активные связи, False - возвращать активные и неактивные связи</param>
        /// <returns>Возвращает объект коллекции связей MLinkCollection, содержащий отобранные связи.</returns>
        public MLinkCollection getLinks(MID axis, MAxisDirection axisDirection, MID currentCellId, MID targetCellId, bool activeOnly)
        {
            MLinkCollection col = new MLinkCollection();
            foreach (MLink li in m_items)
            {
                //check link.isActive
                //если нужны только активные связи, то все неактивные отбрасываются сразу без дальнейших проверок
                if ((activeOnly == true) && (li.isActive == false))
                    continue;
                //тут проверяем остальные параметры
                if (li.Axis.isEqual(axis))
                {
                    if ((axisDirection == MAxisDirection.Any) || (axisDirection == MAxisDirection.Down))
                    {
                        if ((li.upCellID.isEqual(currentCellId)) && (li.downCellID.isEqual(targetCellId)))
                            col.AddLink(li);
                    }
                    else if ((axisDirection == MAxisDirection.Any) || (axisDirection == MAxisDirection.Up))
                    {
                        if ((li.upCellID.isEqual(targetCellId)) && (li.downCellID.isEqual(currentCellId)))
                            col.AddLink(li);
                    }
                }
            }
            return col;
        }


        /// <summary>
        /// NT-Проверить, что связь с такими параметрами существует в списке.
        /// </summary>
        public bool containsLink(MLinkTemplate template)
        {
            foreach (MLink li in m_items)
            {
                if (template.isLinkMatch(li) == true)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// NT-Проверить существование подобной связи по cellId, Axis, Active 
        /// </summary>
        /// <param name="link">link example</param>
        /// <returns></returns>
        public bool containsLink(MLink link)
        {
            //ищем активную связь, совпадающую по ячейкам и оси
            foreach (MLink li in m_items)
            {
                if (link.isEqualLink(li) == true)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// NT-Add link to collection
        /// </summary>
        /// <param name="link"></param>
        public void AddLink(MLink link)
        {
            m_items.Add(link);

        }

        /// <summary>
        /// Remove one link item from list
        /// </summary>
        /// <param name="link"></param>
        public void S1_Remove(MLink link)
        {
            m_items.Remove(link);
        }
        /// <summary>
        /// Remove link at specified list index
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="ArgumentOutOfRangeException">index out of range 0 - count</exception>
        public void S1_RemoveAt(int index)
        {
            m_items.RemoveAt(index);
        }

        /// <summary>
        /// Remove all links
        /// </summary>
        public void Clear()
        {
            this.m_items.Clear();
        }

        /// <summary>
        /// NT-Get all links for specified cell
        /// </summary>
        /// <param name="cellid">cell id</param>
        /// <returns></returns>
        internal MLinkCollection S1_getCellLinks(MID cellid)
        {
            MLinkCollection col = new MLinkCollection();
            foreach (MLink li in m_items)
            {
                if (li.isLinkHaveCell(cellid))
                    col.AddLink(li);
            }
            return col;
        }

        /// <summary>
        /// NT-Set cell reference for specified cell
        /// </summary>
        /// <param name="cellid">cell id</param>
        /// <param name="cell">cell reference or null</param>
        /// <returns></returns>
        internal void S1_setCellRefs(MID cellid, MCell cell)
        {
            foreach (MLink li in m_items)
            {
                li.setCellRefsIfExists(cellid, cell);
            }
        }

        /// <summary>
        /// NT-Получить связи из указанного списка, которых нет в текущем списке.
        /// </summary>
        /// <param name="colD">Список добавляемых связей</param>
        /// <remarks>
        /// Это почти аналог S1_Merge, только здесь копирует связи в возвращаемый список,
        /// а там сразу в общий список, это быстрее, но нет разностного списка.
        /// </remarks>
        internal List<MLink> getUnicalLinks(MLinkCollection colD)
        {
            List<MLink> lout = new List<MLink>();//output list
            //Если  список-аргумент пустой, выходим
            if (colD.m_items.Count == 0) return lout;
            //Если текущий список пустой, копируем все связи из аргумента и выходим
            if (m_items.Count == 0)
            {
                lout.AddRange(colD.Items);
                return lout;
            }
            //поиск перебором
            bool flag;
            //Отсутствующие связи поместить во временный список.
            //в среднем получается N * M/2 проходов-сравнений
            foreach (MLink li in colD.m_items)
            {
                flag = false;
                foreach (MLink lm in this.m_items)
                {
                    if (lm.isEqual(li)) { flag = true; break; } //Связь есть в обеих списках, незачем дальше искать. проверить!
                }
                if (flag == false) // нет совпадения
                    lout.Add(li);
            }
            //output list must contains unical links only
            return lout;
        }

        /// <summary>
        /// NT-Replace id in cell links.
        /// При сохранении временных ячеек меняется идентификатор, его надо заменить и в связях ячейки
        /// </summary>
        /// <param name="oldId">Old id</param>
        /// <param name="newId">New id</param>
        internal void replaceCellId(MID oldId, MID newId)
        {
            foreach (MLink li in this.m_items)
                li.intReplaceID(oldId, newId);
        }
        /// <summary>
        /// NT-Get number of temporary links in memory
        /// </summary>
        /// <returns></returns>
        internal int getNumberOfTempLinks()
        {
            int cnt = 0;
            foreach (MLink li in this.m_items)
                if (!li.isLinkNotTemporary) cnt++;
            return cnt;
        }

        /// <summary>
        /// NT-Get dictionary of link axis with counters.
        /// </summary>
        /// <returns>Dictionary of MID.toU64 id's with counters</returns>
        /// <remarks>
        /// Если словарь будет маленький, его можно сделать постоянным членом коллекции
        /// и обновлять при операциях вставки-удаления-изменения связей
        /// </remarks>
        internal Dictionary<UInt64, int> getLinkAxises()
        {
            Dictionary<UInt64, int> dic = new Dictionary<UInt64,int>();
            foreach (MLink li in this.m_items)
            {
                //считаем только активные связи
                if (li.isActive)
                {
                    UInt64 mid = li.Axis.toU64();
                    if (dic.ContainsKey(mid))
                        dic[mid] += 1;
                    else
                        dic.Add(mid, 1);
                }
            }
            return dic;
        }


    }
}

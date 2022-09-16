using System;
using System.Collections.Generic;
using System.Text;


namespace TestForTapp
{
    /* Тест - сравнение словаря и словаря списков.
     * Это для двойного индекса списка ячеек по их ид и именам.
     * 1 миллион итемов занял 143мб памяти, был создан за 1..2 секунды
     * словарь итемов занял еще 56 мб это +60 байт на итем
     * словарь списков занял еще 79мб это +86 байт на итем
     * итого весь индекс занял почти столько же, сколько сами итемы.
     * Все итемы в словаре списков добавлялись по имени, а имя уникальное, поэтому на один итем получился один список.
     * тут небольшой перераход. Впрочем, и у ячеек имена редко совпадают.
     * 
     * Код оставлен в качестве примера, если потребуется написать новую коллекцию ячеек с индексом по имени ячейки. 
     * Вывод: вообще-то невыгодно делать дополнительные индексы. Хотя, если очень надо, то можно, по нижеприведенной схеме.
     * Вывод: Надо рассчитывать контейнер на миллион загруженных ячеек. Не больше.
    
    */
    /// <summary>
    /// Тестовый итем который обозначать должен некий класс данных
    /// </summary>
    public class Item
    {
        public String m_name;
        public Char[] m_NameArray;
        public UInt64 m_id;

        public Item(UInt64 id)
        {
            m_name = "VeryLongString " + id.ToString();
            m_id = id;
            m_NameArray = m_name.ToCharArray();
        }
    }

    public class ListDictionary
    {
        private Dictionary<String, List<Item>> m_dict;
        
        public ListDictionary()
        {
            m_dict = new Dictionary<String, List<Item>>();
        }

        public void Add(Item item)
        {
            String key = item.m_name;
            //add to list
            if (m_dict.ContainsKey(key))
                m_dict[key].Add(item);
            else
            {
                List<Item> list = new List<Item>();
                list.Add(item);
                m_dict.Add(key, list);
            }
        }

        public Item[] get(String title)
        {
            if (!m_dict.ContainsKey(title))
                return new Item[0];
            else
            {
                return m_dict[title].ToArray();
            }
        }

        internal void Clear()
        {
            this.m_dict.Clear();
        }
    }

    class Program
    {
        public static Dictionary<UInt64, Item> m_dictionary;
        
        
        static void Main(string[] args)
        {
            PrintTimestamp("TestStarted");
            
            //1 создадим 1 млн объектов для словаря в небольшом списке
            List<Item> src = new List<Item>(1000000);
            PrintTimestamp("Src created");
            for (UInt64 i = 0; i < 1000000; i++)
                src.Add(new Item(i));
            PrintTimestamp("Src filled");
            //2 добавим эти объекты в обычный словарь
            m_dictionary = new Dictionary<ulong, Item>();
            foreach (Item it in src)
                m_dictionary.Add(it.m_id, it);
            PrintTimestamp("Dictionary filled");
            
            //3 добавим эти объекты в словарь списков
            ListDictionary ld = new ListDictionary();
            foreach (Item it in src)
                ld.Add(it);
            PrintTimestamp("LD filled");
            //4 очистим список словарей
            ld.Clear();
            ld = null;
            GC.Collect();
            PrintTimestamp("LD is empty");
            //5 очистим словарь
            m_dictionary.Clear();
            m_dictionary = null;
            GC.Collect();
            PrintTimestamp("dictionary is empty");
            //6 очистим исходный набор
            src.Clear();
            src = null;
            GC.Collect();
            PrintTimestamp("Src list is empty");
            return;
        }

        private static void PrintTimestamp(string p)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(p);
            sb.Append(": Time = ");
            sb.Append(DateTime.Now.ToString());

            Console.WriteLine(sb.ToString());
            sb = null;
            Console.ReadLine();
        }



    }
}

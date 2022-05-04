using System;
using System.Collections.Generic;

namespace XamrainMvvm.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }

    public class EntityInfo
    {

        public string EntityName { get; set; }
        public string Count { get; set; }
        public string Color { get; set; }
    }


    public class EntityRecordCountCollection
    {
        public int Count { get; set; }
        public bool IsReadOnly { get; set; }
        public List<string> Keys { get; set; }
        public List<int> Values { get; set; }
    }

    public class EntityData
    {
        
        public EntityRecordCountCollection EntityRecordCountCollection { get; set; }
    }

}
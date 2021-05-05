using System.Collections.Generic;

namespace Archon.Models
{
    /// <summary>
    /// An IEnumerableObject with a key, used for ListViews with grouped items
    /// </summary>
    public class GroupInfoList : List<object>
    {
        public GroupInfoList(IEnumerable<object> items) : base(items)
        {
        }
        public object Key { get; set; }
    }
}

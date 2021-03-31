using System.Collections.Generic;

namespace PublicApi.v1.Common
{
    public class CollectionDTO<TKey>
    {
        public long PageIndex { get; set; }
        public long PagesCount { get; set; }
        public IEnumerable<TKey> Items { get; set; } = default!;
    }
}
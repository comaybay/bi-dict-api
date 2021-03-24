namespace bi_dict_api.Utils.Extensions
{
    using Fizzler.Systems.HtmlAgilityPack;
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class HtmlNodeExtensions
    {
        static private readonly string marker;
        static HtmlNodeExtensions()
        {
            marker = Guid.NewGuid().ToString();
        }
        /* same as QuerySelector, but the query only apply at direct childrens of given node
         * Warning: this method will not work if given node is a root node
         */
        public static HtmlNode QuerySelectorDirect(this HtmlNode node, string query)
        {
            node.AddClass(marker);
            var res = node.ParentNode.QuerySelector($"[class~='{marker}'] > {query}");
            node.RemoveClass(marker);
            return res;
        }

        /* same as QuerySelectorAll, but the query only apply at direct childrens of given node
         * Warning: this method will not work if given node is a root node
         */
        public static IEnumerable<HtmlNode> QuerySelectorAllDirect(this HtmlNode node, string query)
        {
            node.AddClass(marker);
            var res = node.ParentNode.QuerySelectorAll($"[class~='{marker}'] > {query}");
            node.RemoveClass(marker);
            return res;
        }
    }
}

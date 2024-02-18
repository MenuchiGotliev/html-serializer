using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Html_Serializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }= new List<string>();   
        public List<string> Classes { get; set; }=new List<string>();       
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }= new List<HtmlElement>();
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> q = new Queue<HtmlElement>();
            q.Enqueue(this);
            while (q.Count > 0)
            {
                HtmlElement htmlElement = q.Dequeue();
                foreach (var child in htmlElement.Children)
                {
                    q.Enqueue(child);
                }
                yield return htmlElement;
            }
        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement temp = this;
            while(temp.Parent!=null)
            {
                yield return temp;
                temp = temp.Parent;
            }
        }
        public static List<HtmlElement> Search(HtmlElement htmlElement,Selector selector,List<HtmlElement>elements)
        {
            if(selector==null)
            {
                return null;
            }

            IEnumerable<HtmlElement> descendants = htmlElement.Descendants();

            foreach (var descendant in descendants)
            {
                if(descendant.Id!=null&&selector.Id!=null)
                {
                    if(descendant.Id!=selector.Id)
                        continue;
                }
                if(descendant.Name!=null&&selector.TagName!=null)
                {
                    if (descendant.Name != selector.TagName)
                        continue;
                }
                if (descendant.Classes != null && selector.Classes != null)
                {
                    if (!selector.Classes.All(c => descendant.Classes.Contains(c)))
                        continue;
                }

                if (selector.Child == null)
                {
                    elements.Add(descendant);
                }
                else
                    Search(htmlElement, selector.Child, elements);

            }
            var h = new HashSet<HtmlElement>(elements);
            return h.ToList();
        }


       
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Handler;
using Newtonsoft.Json;

namespace Pinduoduo
{
    public class WinFormResourceRequestHandler : ResourceRequestHandler
    {
        protected override IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            var filter = FilterManager.CreateFilter(request.Identifier.ToString());
            return filter;
        }

        protected override void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            if (request.Method.Equals("POST")&&request.Url.Contains("order_list"))
            {
                var filter = FilterManager.GetFileter(request.Identifier.ToString()) as TestJsonFilter;
                if (filter==null)
                {
                    return;
                }
                ASCIIEncoding encoding = new ASCIIEncoding();
                //这里截获返回的数据
                var data = encoding.GetString(filter.DataAll.ToArray());
                var orderList= JsonConvert.DeserializeObject<Order>(data);
                if (orderList.orders!=null&& orderList.orders.Count!=0)
                {
                    Thread.Sleep(1000);
                    FilterManager.AddOrder(orderList);
                    //反向排序
                    orderList.orders.Reverse();
                    //获取第一个
                    var order=orderList.orders.FirstOrDefault();
                    string json = "{\"timeout\":1300,\"type\":\"all\",\"page\":1,\"pay_channel_list\":[\"9\",\"30\",\"31\",\"35\",\"38\",\"52\",\"97\",\"122\",\"135\",\"322\",\"-1\"],\"origin_host_name\":\"mobile.yangkeduo.com\",\"size\":10,\"offset\":\"#offset#\"}";
                    json=json.Replace("#offset#", order.order_sn);
                    chromiumWebBrowser.Navigate(Encoding.Default.GetBytes(json), request);
                }
            }
        }

       
    }

    public static class NavigateExt
    {
        public static void Navigate(this IWebBrowser browser, byte[] postDataBytes,IRequest requestCopy)
        {
            IFrame frame = browser.GetBrowser().MainFrame;
            IRequest request = frame.CreateRequest();

            request.Url = requestCopy.Url;
            request.Method = "POST";

            request.InitializePostData();
            var element = request.PostData.CreatePostDataElement();
            element.Bytes = postDataBytes;
            request.PostData.AddElement(element);

            request.Headers = requestCopy.Headers;

            frame.LoadRequest(request);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinduoduo
{
    public class Order
    {
        public long server_time { get; set; }
        public List<OrderItem> orders { get; set; }
    }
    public class OrderItem { 
        /// <summary>
        /// 快递单号
        /// </summary>
        public string tracking_number { get; set; }
        /// <summary>
        /// 订单编码
        /// </summary>
        public string order_sn { get; set; }
        /// <summary>
        /// 快递公司ID
        /// </summary>
        public string shipping_id { get; set; }
        /// <summary>
        /// 订单状态 4为已收货
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public int display_amount { get; set; }
    }
}

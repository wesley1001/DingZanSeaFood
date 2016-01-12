using System;
using System.IO;
using System.Web;

namespace BreezeShop.Web.Helper.WxLib
{
    public class WxPayApi
    {
        /**
        * 
        * 申请退款
        * @param WxPayData inputObj 提交给申请退款API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回接口调用结果，其他抛异常
        */

        public static WxPayData Refund(double totalMoney, double refundMoney, string orderBatchNo, string transactionId,
            string key, string appid, string mchid, string outTradeNo)
        {
            string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";

            var inputObj = new WxPayData();
            inputObj.SetValue("transaction_id", transactionId??"");
            inputObj.SetValue("out_trade_no", outTradeNo);
            inputObj.SetValue("total_fee", (int) (totalMoney*100)); //订单总金额
            inputObj.SetValue("refund_fee", (int) (refundMoney*100)); //退款金额
            inputObj.SetValue("out_refund_no", orderBatchNo); //随机生成商户退款单号
            inputObj.SetValue("op_user_id", mchid); //操作员，默认为商户号

            inputObj.SetValue("appid", appid); //公众账号ID
            inputObj.SetValue("mch_id", mchid); //商户号
            inputObj.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", "")); //随机字符串
            inputObj.SetValue("sign", inputObj.MakeSign(key)); //签名

            var xml = inputObj.ToXml();
            //var start = DateTime.Now;
            //File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/1.txt"), "2\n");

            //Log.Debug("WxPayApi", "Refund request : " + xml);
            //Log.Debug("WxPayApi", HttpContext.Current.Server.MapPath("~/app_data/apiclient_cert.p12"));
            //Log.Debug("WxPayApi", mchid);
            var response = HttpService.Post(xml, url, true, 6, HttpContext.Current.Server.MapPath("~/app_data/apiclient_cert.p12"), mchid); //调用HTTP通信接口提交数据到API
            //Log.Debug("WxPayApi", "Refund response : " + response);

            //var end = DateTime.Now;
            //int timeCost = (int) ((end - start).TotalMilliseconds); //获得接口耗时
            //File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/1.txt"), "3\n");
            //将xml格式的结果转换为对象以返回
            var result = new WxPayData();
            result.FromXml(response, key);

            return result;
        }


    }
}
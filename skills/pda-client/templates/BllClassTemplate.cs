using System;
using System.Web;
using Mobile.PrinxChengShan.Dal;
using Mobile.PrinxChengShan.Model;
using Mobile.PrinxChengShan.Util;
using DataOperate.Net;

namespace Mobile.PrinxChengShan.Bll
{
    /// <summary>
    /// 功能描述(Description)：{MODULE_TITLE} 业务逻辑处理
    /// 参考代码：04-服务器端程序/LonSon.Mobile.PrinxChengShan.App.Web/Mobile.PrinxChengShan.Bll/LTA0001Bll.cs
    /// </summary>
    public class {MODULE_NAME}Bll
    {
        private {MODULE_NAME}Dal dal = new {MODULE_NAME}Dal();

        public string ProcessRequest(HttpContext context)
        {
            string action = context.Request["action"] ?? string.Empty;
            switch (action)
            {
                case "by":
                    return GetByBarcode(context);
                case "up":
                    return UpdateData(context);
                default:
                    return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("404", "未知操作请求"));
            }
        }

        /// <summary>
        /// 条码扫描查询
        /// </summary>
        private string GetByBarcode(HttpContext context)
        {
            try
            {
                string barcode = context.Request["BARCODE"];
                if (string.IsNullOrEmpty(barcode))
                {
                    return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("1", "条码不能为空"));
                }

                var model = dal.GetByModel(barcode.Trim());
                if (model != null)
                {
                    return JsonHelper<Messaging<object>>.EntityToJson(new Messaging<object>("0", "", model));
                }
                return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("1", "未找到扫描的条码信息!"));
            }
            catch (Exception ex)
            {
                SystemErrorPlug.ErrorRecord(ex.ToString());
                return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("500", ex.Message));
            }
        }

        /// <summary>
        /// 数据提交处理
        /// </summary>
        private string UpdateData(HttpContext context)
        {
            try
            {
                string barcode = context.Request["BARCODE"];
                string fac = context.Request["FAC"];
                string loginName = context.Request["LOGINNAM"];
                string enam = context.Request["ENAM"];
                string qty = context.Request["QTY"];

                // 调用 DAL 执行入库操作
                bool success = dal.UpdateStatus(barcode, fac, loginName, enam, qty);
                if (success)
                {
                    return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("0", "操作成功!"));
                }
                return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("1", "数据保存失败!"));
            }
            catch (Exception ex)
            {
                SystemErrorPlug.ErrorRecord(ex.ToString());
                return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("500", ex.Message));
            }
        }
    }
}

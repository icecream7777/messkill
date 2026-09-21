using System;
using System.Data;
using System.Web;
using Mobile.PrinxChengShan.Dal;
using Mobile.PrinxChengShan.Model;
using Mobile.PrinxChengShan.Util;
using DataOperate.Net;

namespace Mobile.PrinxChengShan.Bll
{
    /// <summary>
    /// 功能描述(Description)：{MODULE_TITLE} 业务逻辑处理
    /// 注意：严禁建立实体类！直接通过 DataTable 和 string 传参交互
    /// 参考代码：04-服务器端程序/LonSon.Mobile.PrinxChengShan.App.Web/Mobile.PrinxChengShan.Bll/BarcodeQueryBll.cs
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
                    return GetBarcodeQueryData(context);
                case "up":
                    return UpdateData(context);
                default:
                    return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("404", "未知操作请求"));
            }
        }

        /// <summary>
        /// 条码扫描查询：直接返回 DataTable（前端通过 data.TL 接收）
        /// </summary>
        private string GetBarcodeQueryData(HttpContext context)
        {
            try
            {
                string barcode = context.Request["BARCODE"];
                string fac = context.Request["FAC"];

                if (string.IsNullOrEmpty(barcode))
                {
                    return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("1", "扫描条码不能为空！"));
                }

                // DAL 层直接执行 SQL 返回 DataTable，严禁建实体类
                DataTable dt = dal.GetBarcodeQueryData(barcode.Trim(), fac);
                if (dt != null && dt.Rows.Count > 0)
                {
                    // 传入 DataTable，序列化后前端通过 data.TL[0] 读取
                    return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("0", "查询成功", dt));
                }
                else
                {
                    return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("1", "未找到扫描的条码信息！"));
                }
            }
            catch (Exception ex)
            {
                SystemErrorPlug.ErrorRecord(ex.ToString());
                return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("500", ex.Message.Trim().Replace("\r\n", "")));
            }
        }

        /// <summary>
        /// 业务提交保存：直接返回操作结果字符串（前端通过 data.Error 接收提示）
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

                if (string.IsNullOrEmpty(barcode))
                {
                    return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("1", "条码信息缺失！"));
                }

                // DAL 层执行更新 SQL
                bool success = dal.UpdateData(barcode.Trim(), fac, loginName, enam, qty);
                if (success)
                {
                    return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("0", "操作成功！"));
                }
                else
                {
                    return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("1", "数据提交失败，请重试！"));
                }
            }
            catch (Exception ex)
            {
                SystemErrorPlug.ErrorRecord(ex.ToString());
                return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("500", ex.Message.Trim().Replace("\r\n", "")));
            }
        }
    }
}

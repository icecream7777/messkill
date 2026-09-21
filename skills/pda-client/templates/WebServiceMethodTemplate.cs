//======================WebService后端处理方法===============================
//  功能描述(Description)：{MODULE_TITLE} - PDA 移动端接口
//  作者(Author)：AI Assistant
//  日期(Create Time)：{CREATE_DATE}
//==================================================================
using System;
using System.Data;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json;
using Core.Interface;

namespace MES.WebService
{
    public class {SERVICE_CLASS}
    {
        private IConfig Config;

        public {SERVICE_CLASS}(IConfig config)
        {
            this.Config = config;
        }

        /// <summary>
        /// 查询条码基础物料信息
        /// Params: [0]FAC, [1]LOGINNAME, [2]BARCODE
        /// </summary>
        public string {SERVICE_PREFIX}_GetBarcodeInfo(string[] Params)
        {
            try
            {
                if (Params == null || Params.Length < 3)
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "参数缺失" });
                }

                string fac = Params[0];
                string user = Params[1];
                string barcode = Params[2];

                string sql = string.Format(@"SELECT TOP 1 ITNBR, ITDSC, QTY 
                                             FROM {SOURCE_TABLE} WITH (NOLOCK) 
                                             WHERE BARCODE = '{0}' AND FAC = '{1}'", 
                                             barcode, fac);

                DataTable dt = Config.DataBase.GetTable(sql);
                if (dt.Rows.Count > 0)
                {
                    var dataObj = new
                    {
                        ITNBR = dt.Rows[0]["ITNBR"].ToString(),
                        ITDSC = dt.Rows[0]["ITDSC"].ToString(),
                        QTY = dt.Rows[0]["QTY"].ToString()
                    };
                    return JsonConvert.SerializeObject(new { Success = true, Data = dataObj });
                }
                else
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "未找到该条码的物料信息！" });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = "系统异常: " + ex.Message });
            }
        }

        /// <summary>
        /// 提交扫码实绩保存入库
        /// Params: [0]FAC, [1]LOGINNAME, [2]BARCODE, [3]ITNBR, [4]QTY
        /// </summary>
        public string {SERVICE_PREFIX}_SubmitData(string[] Params)
        {
            try
            {
                if (Params == null || Params.Length < 5)
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "提交参数不完整" });
                }

                string fac = Params[0];
                string loginName = Params[1];
                string barcode = Params[2];
                string itnbr = Params[3];
                string qty = Params[4];

                // 防重复提交校验
                string checkSql = string.Format(@"SELECT COUNT(1) FROM {TARGET_TABLE} WITH (NOLOCK) 
                                                  WHERE BARCODE = '{0}' AND WDATE = CONVERT(date, GETDATE())", barcode);
                int count = Convert.ToInt32(Config.DataBase.GetTable(checkSql).Rows[0][0]);
                if (count > 0)
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "该条码今日已被扫描记录，不可重复提交！" });
                }

                // 插入记录
                string insertSql = string.Format(@"INSERT INTO {TARGET_TABLE} (FAC, BARCODE, ITNBR, QTY, LOGNAM, CRTIM, WDATE)
                                                   VALUES ('{0}', '{1}', '{2}', {3}, '{4}', GETDATE(), CONVERT(date, GETDATE()))",
                                                   fac, barcode, itnbr, qty, loginName);

                int rows = Config.DataBase.ExecuteNonQuery(insertSql);
                if (rows > 0)
                {
                    return JsonConvert.SerializeObject(new { Success = true, Message = "保存成功" });
                }
                else
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "保存失败，未影响数据行" });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = "服务异常: " + ex.Message });
            }
        }
    }
}

//======================Bus弹窗控制器===============================
//  文件名(File Name)：{MODULE_CODE}B1.cs
//  功能描述(Description)：{MODULE_TITLE} - 对话框业务控制器
//  作者(Author)：AI Assistant
//  日期(Create Time)：{CREATE_DATE}
//==================================================================
using System;
using System.Data;
using System.Windows.Forms;
using Core.Interface;
using MESService;

namespace {MODULE_PREFIX}_BUS
{
    public class {MODULE_CODE}B1 : BusniessDialogClassBase
    {
        public {MODULE_CODE}B1(IConfig Config)
            : base(Config)
        { }

        // 绑定控件
        private TextBox TxtITNBR => this.GetControlByName("TxtITNBR") as TextBox;
        private TextBox TxtQTY => this.GetControlByName("TxtQTY") as TextBox;
        private ComboBox CmbWSHT => this.GetControlByName("CmbWSHT") as ComboBox;
        private DateTimePicker DtpWDATE => this.GetControlByName("DtpWDATE") as DateTimePicker;

        // 注入的数据行对象
        private DataRow Row => this.GetPropertieByName("Row") as DataRow;

        /// <summary>
        /// 对话框加载与数据回填
        /// </summary>
        public override void Form_Load(object sender, EventArgs e)
        {
            // 初始化下拉列表
            string shiftSql = Config.SqlExec.GetSqlText("Shift_GetData");
            if (!string.IsNullOrEmpty(shiftSql))
            {
                DataTable dtShift = Config.DataBase.GetTable(shiftSql);
                CmbWSHT.DataSource = dtShift;
                CmbWSHT.ValueMember = "DCOD";
                CmbWSHT.DisplayMember = "DNAM";
            }

            // 若为修改模式，回填已有数据
            if (Row != null && Row["ID"] != DBNull.Value && !string.IsNullOrEmpty(Row["ID"].ToString()))
            {
                if (TxtITNBR != null) TxtITNBR.Text = Row["ITNBR"]?.ToString() ?? "";
                if (TxtQTY != null) TxtQTY.Text = Row["QTY"]?.ToString() ?? "0";
                if (CmbWSHT != null) CmbWSHT.SelectedValue = Row["WSHT"]?.ToString() ?? "1";
                if (DtpWDATE != null && Row["WDATE"] != DBNull.Value)
                {
                    DtpWDATE.Value = Convert.ToDateTime(Row["WDATE"]);
                }
            }
            else
            {
                if (DtpWDATE != null) DtpWDATE.Value = TimeService.GetFrameDateTime();
            }

            base.Form_Load(sender, e);
        }

        /// <summary>
        /// 确定保存校验
        /// </summary>
        public override void BtnOK_Click(object sender, EventArgs e)
        {
            if (TxtITNBR != null && string.IsNullOrWhiteSpace(TxtITNBR.Text))
            {
                MessageService.ShowError("物料编码不能为空！");
                return;
            }

            if (TxtQTY != null)
            {
                if (!decimal.TryParse(TxtQTY.Text.Trim(), out decimal qty) || qty <= 0)
                {
                    MessageService.ShowError("请输入大于 0 的有效数量！");
                    return;
                }
            }

            // 数据映射回填至 DataRow
            if (Row != null)
            {
                if (TxtITNBR != null) Row["ITNBR"] = TxtITNBR.Text.Trim();
                if (TxtQTY != null) Row["QTY"] = Convert.ToDecimal(TxtQTY.Text.Trim());
                if (CmbWSHT != null) Row["WSHT"] = CmbWSHT.SelectedValue?.ToString() ?? "1";
                if (DtpWDATE != null) Row["WDATE"] = DtpWDATE.Value.ToString("yyyy-MM-dd");
            }

            base.BtnOK_Click(sender, e);
        }
    }
}

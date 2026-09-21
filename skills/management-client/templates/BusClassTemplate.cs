//======================Bus主控制器===============================
//  文件名(File Name)：{MODULE_CODE}B.cs
//  功能描述(Description)：{MODULE_TITLE}
//  数据表(Table)：{TABLE_NAME}
//  作者(Author)：AI Assistant
//  日期(Create Time)：{CREATE_DATE}
//==================================================================
using System;
using System.Data;
using System.Windows.Forms;
using Core.Interface;
using MESService;
using UserControls;

namespace {MODULE_PREFIX}_BUS
{
    public class {MODULE_CODE}B : BusniessClassBase
    {
        public {MODULE_CODE}B(IConfig Config)
            : base(Config)
        { }

        // 控件动态实例化
        private LSDataGrid GrdMain => this.GetControlByName("GrdMain") as LSDataGrid;
        private DataTable dtSource = new DataTable();

        /// <summary>
        /// 窗体加载初始化
        /// </summary>
        public override void Form_Load(object sender, EventArgs e)
        {
            GetData();
            base.Form_Load(sender, e);
        }

        /// <summary>
        /// 查询数据源
        /// </summary>
        public override void GetData()
        {
            try
            {
                string sql = @"SELECT * FROM {TABLE_NAME} WITH (NOLOCK) ORDER BY ID DESC";
                dtSource = Config.DataBase.GetTable(sql);
                GrdMain.DataSource = dtSource;
            }
            catch (Exception ex)
            {
                MessageService.ShowError("数据查询失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 查询按钮事件
        /// </summary>
        public override void BtnQuery_Click(object sender, EventArgs e)
        {
            GetData();
        }

        /// <summary>
        /// 新增按钮事件
        /// </summary>
        public override void BtnNew_Click(object sender, EventArgs e)
        {
            GrdMain.NewRow();
            GetData();
        }

        /// <summary>
        /// 修改按钮事件
        /// </summary>
        public override void BtnEdit_Click(object sender, EventArgs e)
        {
            if (GrdMain.CurrentRow == null)
            {
                MessageService.ShowError("请先选择一行要修改的数据！");
                return;
            }
            GrdMain.EditRow();
            GetData();
        }

        /// <summary>
        /// 删除按钮事件
        /// </summary>
        public override void BtnDelete_Click(object sender, EventArgs e)
        {
            if (GrdMain.CurrentRow == null)
            {
                MessageService.ShowError("请先选择一行要删除的数据！");
                return;
            }

            if (MessageService.ShowAsk("确定要删除所选数据吗？") == DialogResult.OK)
            {
                GrdMain.DeleteRow();
                GetData();
            }
        }

        /// <summary>
        /// 表格增删改统一事件拦截
        /// </summary>
        public void GrdMain_OnDataChange(object sender, LSDataRowEventArgs e)
        {
            if (e.ChangeType == ChangeType.New)
            {
                if (Config.FormService.ShowDialog(this.ViewForm.Text, "{MODULE_PREFIX}_VIEW|{MODULE_CODE}W1",
                    "{MODULE_PREFIX}_BUS|{MODULE_CODE}B1", new object[] { e.Row }) == DialogResult.OK)
                {
                    e.Row["UPTIM"] = TimeService.GetFrameDateTime();
                    e.Row["CRTIM"] = TimeService.GetFrameDateTime();
                    e.Row["ENAM"] = Config.DataList["mEmployeeName"]?.ToString() ?? "";

                    Config.DataBase.InsertRow("{TABLE_NAME}", e.Row);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else if (e.ChangeType == ChangeType.Edit)
            {
                if (Config.FormService.ShowDialog(this.ViewForm.Text, "{MODULE_PREFIX}_VIEW|{MODULE_CODE}W1",
                    "{MODULE_PREFIX}_BUS|{MODULE_CODE}B1", new object[] { e.Row }) == DialogResult.OK)
                {
                    e.Row["UPTIM"] = TimeService.GetFrameDateTime();
                    e.Row["ENAM"] = Config.DataList["mEmployeeName"]?.ToString() ?? "";

                    Config.DataBase.UpdateRow("{TABLE_NAME}", e.Row);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else if (e.ChangeType == ChangeType.Delete)
            {
                Config.DataBase.DeleteRow("{TABLE_NAME}", e.Row);
            }
        }
    }
}

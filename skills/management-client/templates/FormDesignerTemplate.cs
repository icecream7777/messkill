namespace {MODULE_PREFIX}_VIEW
{
    partial class {MODULE_CODE}W
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnQuery = new UserControls.LSToolButton();
            this.BtnNew = new UserControls.LSToolButton();
            this.BtnEdit = new UserControls.LSToolButton();
            this.BtnDelete = new UserControls.LSToolButton();
            this.GrdMain = new UserControls.LSDataGrid();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtnQuery);
            this.panel1.Controls.Add(this.BtnNew);
            this.panel1.Controls.Add(this.BtnEdit);
            this.panel1.Controls.Add(this.BtnDelete);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(900, 60);
            this.panel1.TabIndex = 0;
            // 
            // BtnQuery
            // 
            this.BtnQuery.Location = new System.Drawing.Point(20, 15);
            this.BtnQuery.Name = "BtnQuery";
            this.BtnQuery.ShowText = true;
            this.BtnQuery.Size = new System.Drawing.Size(65, 30);
            this.BtnQuery.TabIndex = 1;
            this.BtnQuery.Title = "查询";
            // 
            // BtnNew
            // 
            this.BtnNew.Location = new System.Drawing.Point(95, 15);
            this.BtnNew.Name = "BtnNew";
            this.BtnNew.ShowText = true;
            this.BtnNew.Size = new System.Drawing.Size(65, 30);
            this.BtnNew.TabIndex = 2;
            this.BtnNew.Title = "新增";
            // 
            // BtnEdit
            // 
            this.BtnEdit.Location = new System.Drawing.Point(170, 15);
            this.BtnEdit.Name = "BtnEdit";
            this.BtnEdit.ShowText = true;
            this.BtnEdit.Size = new System.Drawing.Size(65, 30);
            this.BtnEdit.TabIndex = 3;
            this.BtnEdit.Title = "修改";
            // 
            // BtnDelete
            // 
            this.BtnDelete.Location = new System.Drawing.Point(245, 15);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.ShowText = true;
            this.BtnDelete.Size = new System.Drawing.Size(65, 30);
            this.BtnDelete.TabIndex = 4;
            this.BtnDelete.Title = "删除";
            // 
            // GrdMain
            // 
            this.GrdMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrdMain.Location = new System.Drawing.Point(0, 60);
            this.GrdMain.Name = "GrdMain";
            this.GrdMain.Size = new System.Drawing.Size(900, 440);
            this.GrdMain.TabIndex = 1;
            // 
            // {MODULE_CODE}W
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.Controls.Add(this.GrdMain);
            this.Controls.Add(this.panel1);
            this.Name = "{MODULE_CODE}W";
            this.Text = "{MODULE_TITLE}";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.{MODULE_CODE}W_FormClosed);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private UserControls.LSToolButton BtnQuery;
        private UserControls.LSToolButton BtnNew;
        private UserControls.LSToolButton BtnEdit;
        private UserControls.LSToolButton BtnDelete;
        private UserControls.LSDataGrid GrdMain;
    }
}

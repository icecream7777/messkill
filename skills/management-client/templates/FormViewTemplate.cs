//======================View主窗体===============================
//  文件名(File Name)：{MODULE_CODE}W.cs
//  功能描述(Description)：{MODULE_TITLE}
//  作者(Author)：AI Assistant
//  日期(Create Time)：{CREATE_DATE}
//==================================================================
using System;
using System.Windows.Forms;
using Core.Interface;
using UserControls;

namespace {MODULE_PREFIX}_VIEW
{
    public partial class {MODULE_CODE}W : Form
    {
        private IConfig config;

        public {MODULE_CODE}W(IConfig Config)
        {
            InitializeComponent();
            this.config = Config;
            GetPic();
        }

        private static {MODULE_CODE}W instance = null;

        public static {MODULE_CODE}W Instance(IConfig Config)
        {
            if (instance == null) instance = new {MODULE_CODE}W(Config);
            return instance;
        }

        private void {MODULE_CODE}W_FormClosed(object sender, FormClosedEventArgs e)
        {
            instance = null;
        }

        public void GetPic()
        {
            foreach (Control col in panel1.Controls)
            {
                if (col is LSToolButton btn)
                {
                    btn.SelectedImage = config.ImageFactory.GetImage(btn.Title, true);
                    btn.Image = config.ImageFactory.GetImage(btn.Title, false);
                }
            }
        }
    }
}

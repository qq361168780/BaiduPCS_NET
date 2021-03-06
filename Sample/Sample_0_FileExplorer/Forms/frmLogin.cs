﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BaiduPCS_NET;

namespace FileExplorer
{
    public partial class frmLogin : Form
    {
        BaiduPCS pcs;

        public frmLogin(BaiduPCS pcs)
        {
            InitializeComponent();
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.pcs = pcs;
            pcs.GetCaptcha += new GetCaptchaFunction(OnGetCaptcha);
            pcs.GetInput += new GetInputFunction(onGetInput);
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            pcs.GetCaptcha -= new GetCaptchaFunction(OnGetCaptcha);
            pcs.GetInput -= new GetInputFunction(onGetInput);
            base.OnClosing(e);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string username = txUserName.Text.Trim();
            string password = txPassword.Text.Trim();
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please input User Name and Password");
                return;
            }
            btnOk.Enabled = false;
            btnOk.Text = "Login...";
            btnCancel.Enabled = false;
            PcsRes rc = pcs.login(username, password);
            if (rc != PcsRes.PCS_OK)
            {
                string errmsg = pcs.getError();
                MessageBox.Show("Failed to login: " + errmsg);
                btnOk.Enabled = true;
                btnOk.Text = "Login";
                btnCancel.Enabled = true;
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        static bool OnGetCaptcha(BaiduPCS sender, byte[] imgBytes, out string captcha, object userdata)
        {
            captcha = null;
            frmCaptcha frm = new frmCaptcha(imgBytes);
            frm.TopMost = true;
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                captcha = frm.Captcha;
                return true;
            }
            return false;
        }

        private bool onGetInput(BaiduPCS sender, string tips, out string captcha, object userdata)
        {
            captcha = null;
            frmInput frm = new frmInput();
            frm.TopMost = true;
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                captcha = frm.Value;
                return true;
            }
            return false;
        }
    }
}

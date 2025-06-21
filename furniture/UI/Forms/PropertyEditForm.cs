using System;
using System.Windows.Forms;
using yz.furniture.Features;

namespace yz.furniture.UI.Forms
{
    public partial class PropertyEditForm : Form
    {
        private DrawerBox _drawerBox;

        // UI Controls
        private System.Windows.Forms.Label lblComponentName;
        private System.Windows.Forms.TextBox txtComponentName;
        private System.Windows.Forms.Label lblPartName;
        private System.Windows.Forms.TextBox txtPartName;
        private System.Windows.Forms.Label lblMaterial;
        private System.Windows.Forms.TextBox txtMaterial;
        private System.Windows.Forms.GroupBox grpAllowances;
        private System.Windows.Forms.Label lblAllowanceX;
        private System.Windows.Forms.TextBox txtAllowanceX;
        private System.Windows.Forms.Label lblAllowanceY;
        private System.Windows.Forms.TextBox txtAllowanceY;
        private System.Windows.Forms.Label lblAllowanceZ;
        private System.Windows.Forms.TextBox txtAllowanceZ;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;

        public PropertyEditForm(DrawerBox drawerBox)
        {
            _drawerBox = drawerBox;
            InitializeComponent();
            this.Load += OnFormLoad;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            // 将DrawerBox对象的数据加载到UI控件中
            txtComponentName.Text = _drawerBox.ComponentName;
            txtPartName.Text = _drawerBox.Name;
            txtMaterial.Text = _drawerBox.Material;
            txtAllowanceX.Text = _drawerBox.AllowanceX.ToString();
            txtAllowanceY.Text = _drawerBox.AllowanceY.ToString();
            txtAllowanceZ.Text = _drawerBox.AllowanceZ.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                // 将UI控件中的数据保存回DrawerBox对象
                _drawerBox.ComponentName = txtComponentName.Text;
                _drawerBox.Name = txtPartName.Text;
                _drawerBox.Material = txtMaterial.Text;
                _drawerBox.AllowanceX = double.Parse(txtAllowanceX.Text);
                _drawerBox.AllowanceY = double.Parse(txtAllowanceY.Text);
                _drawerBox.AllowanceZ = double.Parse(txtAllowanceZ.Text);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("请输入有效的数字作为下料余量。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存数据时发生错误: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InitializeComponent()
        {
            this.lblComponentName = new System.Windows.Forms.Label();
            this.txtComponentName = new System.Windows.Forms.TextBox();
            this.lblPartName = new System.Windows.Forms.Label();
            this.txtPartName = new System.Windows.Forms.TextBox();
            this.lblMaterial = new System.Windows.Forms.Label();
            this.txtMaterial = new System.Windows.Forms.TextBox();
            this.grpAllowances = new System.Windows.Forms.GroupBox();
            this.lblAllowanceX = new System.Windows.Forms.Label();
            this.txtAllowanceX = new System.Windows.Forms.TextBox();
            this.lblAllowanceY = new System.Windows.Forms.Label();
            this.txtAllowanceY = new System.Windows.Forms.TextBox();
            this.lblAllowanceZ = new System.Windows.Forms.Label();
            this.txtAllowanceZ = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpAllowances.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblComponentName
            // 
            this.lblComponentName.AutoSize = true;
            this.lblComponentName.Location = new System.Drawing.Point(12, 15);
            this.lblComponentName.Name = "lblComponentName";
            this.lblComponentName.Size = new System.Drawing.Size(59, 12);
            this.lblComponentName.TabIndex = 0;
            this.lblComponentName.Text = "部件名称:";
            // 
            // txtComponentName
            // 
            this.txtComponentName.Location = new System.Drawing.Point(77, 12);
            this.txtComponentName.Name = "txtComponentName";
            this.txtComponentName.Size = new System.Drawing.Size(215, 21);
            this.txtComponentName.TabIndex = 1;
            // 
            // lblPartName
            // 
            this.lblPartName.AutoSize = true;
            this.lblPartName.Location = new System.Drawing.Point(12, 42);
            this.lblPartName.Name = "lblPartName";
            this.lblPartName.Size = new System.Drawing.Size(59, 12);
            this.lblPartName.TabIndex = 2;
            this.lblPartName.Text = "零件名称:";
            // 
            // txtPartName
            // 
            this.txtPartName.Location = new System.Drawing.Point(77, 39);
            this.txtPartName.Name = "txtPartName";
            this.txtPartName.Size = new System.Drawing.Size(215, 21);
            this.txtPartName.TabIndex = 3;
            // 
            // lblMaterial
            // 
            this.lblMaterial.AutoSize = true;
            this.lblMaterial.Location = new System.Drawing.Point(12, 69);
            this.lblMaterial.Name = "lblMaterial";
            this.lblMaterial.Size = new System.Drawing.Size(35, 12);
            this.lblMaterial.TabIndex = 4;
            this.lblMaterial.Text = "材质:";
            // 
            // txtMaterial
            // 
            this.txtMaterial.Location = new System.Drawing.Point(77, 66);
            this.txtMaterial.Name = "txtMaterial";
            this.txtMaterial.Size = new System.Drawing.Size(215, 21);
            this.txtMaterial.TabIndex = 5;
            // 
            // grpAllowances
            // 
            this.grpAllowances.Controls.Add(this.lblAllowanceZ);
            this.grpAllowances.Controls.Add(this.txtAllowanceZ);
            this.grpAllowances.Controls.Add(this.lblAllowanceY);
            this.grpAllowances.Controls.Add(this.txtAllowanceY);
            this.grpAllowances.Controls.Add(this.lblAllowanceX);
            this.grpAllowances.Controls.Add(this.txtAllowanceX);
            this.grpAllowances.Location = new System.Drawing.Point(14, 100);
            this.grpAllowances.Name = "grpAllowances";
            this.grpAllowances.Size = new System.Drawing.Size(278, 110);
            this.grpAllowances.TabIndex = 6;
            this.grpAllowances.TabStop = false;
            this.grpAllowances.Text = "下料余量";
            // 
            // lblAllowanceX
            // 
            this.lblAllowanceX.AutoSize = true;
            this.lblAllowanceX.Location = new System.Drawing.Point(15, 27);
            this.lblAllowanceX.Name = "lblAllowanceX";
            this.lblAllowanceX.Size = new System.Drawing.Size(47, 12);
            this.lblAllowanceX.TabIndex = 0;
            this.lblAllowanceX.Text = "X方向:";
            // 
            // txtAllowanceX
            // 
            this.txtAllowanceX.Location = new System.Drawing.Point(68, 24);
            this.txtAllowanceX.Name = "txtAllowanceX";
            this.txtAllowanceX.Size = new System.Drawing.Size(190, 21);
            this.txtAllowanceX.TabIndex = 1;
            // 
            // lblAllowanceY
            // 
            this.lblAllowanceY.AutoSize = true;
            this.lblAllowanceY.Location = new System.Drawing.Point(15, 54);
            this.lblAllowanceY.Name = "lblAllowanceY";
            this.lblAllowanceY.Size = new System.Drawing.Size(47, 12);
            this.lblAllowanceY.TabIndex = 2;
            this.lblAllowanceY.Text = "Y方向:";
            // 
            // txtAllowanceY
            // 
            this.txtAllowanceY.Location = new System.Drawing.Point(68, 51);
            this.txtAllowanceY.Name = "txtAllowanceY";
            this.txtAllowanceY.Size = new System.Drawing.Size(190, 21);
            this.txtAllowanceY.TabIndex = 3;
            // 
            // lblAllowanceZ
            // 
            this.lblAllowanceZ.AutoSize = true;
            this.lblAllowanceZ.Location = new System.Drawing.Point(15, 81);
            this.lblAllowanceZ.Name = "lblAllowanceZ";
            this.lblAllowanceZ.Size = new System.Drawing.Size(47, 12);
            this.lblAllowanceZ.TabIndex = 4;
            this.lblAllowanceZ.Text = "Z方向:";
            // 
            // txtAllowanceZ
            // 
            this.txtAllowanceZ.Location = new System.Drawing.Point(68, 78);
            this.txtAllowanceZ.Name = "txtAllowanceZ";
            this.txtAllowanceZ.Size = new System.Drawing.Size(190, 21);
            this.txtAllowanceZ.TabIndex = 5;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(136, 226);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(217, 226);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PropertyEditForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(304, 261);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpAllowances);
            this.Controls.Add(this.txtMaterial);
            this.Controls.Add(this.lblMaterial);
            this.Controls.Add(this.txtPartName);
            this.Controls.Add(this.lblPartName);
            this.Controls.Add(this.txtComponentName);
            this.Controls.Add(this.lblComponentName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertyEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "编辑部件属性";
            this.grpAllowances.ResumeLayout(false);
            this.grpAllowances.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

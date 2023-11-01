namespace CDK.Assets.Scenes
{
    partial class GizmoControl
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel = new CDK.Assets.Components.StackPanel();
            this.objectPanel = new System.Windows.Forms.Panel();
            this.targetTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.targetAddButton = new System.Windows.Forms.Button();
            this.targetRemoveButton = new System.Windows.Forms.Button();
            this.bindingPanel = new System.Windows.Forms.Panel();
            this.nodeNameClearButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.bindingComboBox = new CDK.Assets.Components.ComboBox();
            this.positionPanel = new System.Windows.Forms.Panel();
            this.fromGroundCheckBox = new System.Windows.Forms.CheckBox();
            this.positionControl = new CDK.Assets.Components.Vector3Control();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.gridPositionControl = new CDK.Assets.Components.Vector3Control();
            this.panel2 = new System.Windows.Forms.Panel();
            this.scaleResetButton = new System.Windows.Forms.Button();
            this.rotationResetButton = new System.Windows.Forms.Button();
            this.scaleControl = new CDK.Assets.Components.Vector3Control();
            this.rotationControl = new CDK.Assets.Components.Vector3Control();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel.SuspendLayout();
            this.objectPanel.SuspendLayout();
            this.bindingPanel.SuspendLayout();
            this.positionPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.objectPanel);
            this.panel.Controls.Add(this.bindingPanel);
            this.panel.Controls.Add(this.positionPanel);
            this.panel.Controls.Add(this.panel2);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(283, 180);
            this.panel.TabIndex = 21;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // objectPanel
            // 
            this.objectPanel.Controls.Add(this.targetTextBox);
            this.objectPanel.Controls.Add(this.label1);
            this.objectPanel.Controls.Add(this.targetAddButton);
            this.objectPanel.Controls.Add(this.targetRemoveButton);
            this.objectPanel.Location = new System.Drawing.Point(0, 0);
            this.objectPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.objectPanel.Name = "objectPanel";
            this.objectPanel.Size = new System.Drawing.Size(283, 22);
            this.objectPanel.TabIndex = 0;
            // 
            // targetTextBox
            // 
            this.targetTextBox.AllowDrop = true;
            this.targetTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.targetTextBox.Location = new System.Drawing.Point(62, 0);
            this.targetTextBox.Name = "targetTextBox";
            this.targetTextBox.ReadOnly = true;
            this.targetTextBox.Size = new System.Drawing.Size(163, 21);
            this.targetTextBox.TabIndex = 1;
            this.targetTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.ObjectTextBox_DragDrop);
            this.targetTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this.ObjectTextBox_DragOver);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Target";
            // 
            // targetAddButton
            // 
            this.targetAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.targetAddButton.Location = new System.Drawing.Point(231, -1);
            this.targetAddButton.Name = "targetAddButton";
            this.targetAddButton.Size = new System.Drawing.Size(23, 23);
            this.targetAddButton.TabIndex = 2;
            this.targetAddButton.Text = "+";
            this.targetAddButton.UseVisualStyleBackColor = true;
            this.targetAddButton.Click += new System.EventHandler(this.ObjectAddButton_Click);
            // 
            // targetRemoveButton
            // 
            this.targetRemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.targetRemoveButton.Location = new System.Drawing.Point(260, -1);
            this.targetRemoveButton.Name = "targetRemoveButton";
            this.targetRemoveButton.Size = new System.Drawing.Size(23, 23);
            this.targetRemoveButton.TabIndex = 3;
            this.targetRemoveButton.Text = "-";
            this.targetRemoveButton.UseVisualStyleBackColor = true;
            this.targetRemoveButton.Click += new System.EventHandler(this.ObjectRemoveButton_Click);
            // 
            // bindingPanel
            // 
            this.bindingPanel.Controls.Add(this.nodeNameClearButton);
            this.bindingPanel.Controls.Add(this.label3);
            this.bindingPanel.Controls.Add(this.bindingComboBox);
            this.bindingPanel.Location = new System.Drawing.Point(0, 28);
            this.bindingPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.bindingPanel.Name = "bindingPanel";
            this.bindingPanel.Size = new System.Drawing.Size(283, 22);
            this.bindingPanel.TabIndex = 1;
            // 
            // nodeNameClearButton
            // 
            this.nodeNameClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nodeNameClearButton.Location = new System.Drawing.Point(260, 0);
            this.nodeNameClearButton.Name = "nodeNameClearButton";
            this.nodeNameClearButton.Size = new System.Drawing.Size(23, 22);
            this.nodeNameClearButton.TabIndex = 19;
            this.nodeNameClearButton.Text = "-";
            this.nodeNameClearButton.UseVisualStyleBackColor = true;
            this.nodeNameClearButton.Click += new System.EventHandler(this.NodeNameClearButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "Binding";
            // 
            // bindingComboBox
            // 
            this.bindingComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bindingComboBox.FormattingEnabled = true;
            this.bindingComboBox.Location = new System.Drawing.Point(62, 1);
            this.bindingComboBox.Name = "bindingComboBox";
            this.bindingComboBox.Size = new System.Drawing.Size(192, 20);
            this.bindingComboBox.TabIndex = 18;
            this.bindingComboBox.DropDown += new System.EventHandler(this.NodeNameComboBox_DropDown);
            // 
            // positionPanel
            // 
            this.positionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.positionPanel.Controls.Add(this.fromGroundCheckBox);
            this.positionPanel.Controls.Add(this.positionControl);
            this.positionPanel.Controls.Add(this.label2);
            this.positionPanel.Controls.Add(this.label4);
            this.positionPanel.Controls.Add(this.gridPositionControl);
            this.positionPanel.Location = new System.Drawing.Point(0, 56);
            this.positionPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.positionPanel.Name = "positionPanel";
            this.positionPanel.Size = new System.Drawing.Size(283, 70);
            this.positionPanel.TabIndex = 20;
            // 
            // fromGroundCheckBox
            // 
            this.fromGroundCheckBox.AutoSize = true;
            this.fromGroundCheckBox.Location = new System.Drawing.Point(91, 54);
            this.fromGroundCheckBox.Name = "fromGroundCheckBox";
            this.fromGroundCheckBox.Size = new System.Drawing.Size(98, 16);
            this.fromGroundCheckBox.TabIndex = 18;
            this.fromGroundCheckBox.Text = "From Ground";
            this.fromGroundCheckBox.UseVisualStyleBackColor = true;
            // 
            // positionControl
            // 
            this.positionControl.DecimalPlaces = 2;
            this.positionControl.Increment = 1F;
            this.positionControl.Location = new System.Drawing.Point(91, 0);
            this.positionControl.Maximum = 10000F;
            this.positionControl.Minimum = -10000F;
            this.positionControl.Name = "positionControl";
            this.positionControl.Size = new System.Drawing.Size(192, 21);
            this.positionControl.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Position";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 12);
            this.label4.TabIndex = 17;
            this.label4.Text = "Grid";
            // 
            // gridPositionControl
            // 
            this.gridPositionControl.DecimalPlaces = 2;
            this.gridPositionControl.Increment = 1F;
            this.gridPositionControl.Location = new System.Drawing.Point(91, 27);
            this.gridPositionControl.Maximum = 10000F;
            this.gridPositionControl.Minimum = -10000F;
            this.gridPositionControl.Name = "gridPositionControl";
            this.gridPositionControl.Size = new System.Drawing.Size(192, 21);
            this.gridPositionControl.TabIndex = 16;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.scaleResetButton);
            this.panel2.Controls.Add(this.rotationResetButton);
            this.panel2.Controls.Add(this.scaleControl);
            this.panel2.Controls.Add(this.rotationControl);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Location = new System.Drawing.Point(0, 132);
            this.panel2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(283, 48);
            this.panel2.TabIndex = 21;
            // 
            // scaleResetButton
            // 
            this.scaleResetButton.Location = new System.Drawing.Point(61, 26);
            this.scaleResetButton.Name = "scaleResetButton";
            this.scaleResetButton.Size = new System.Drawing.Size(23, 23);
            this.scaleResetButton.TabIndex = 24;
            this.scaleResetButton.Text = "←";
            this.scaleResetButton.UseVisualStyleBackColor = true;
            this.scaleResetButton.Click += new System.EventHandler(this.ScaleResetButton_Click);
            // 
            // rotationResetButton
            // 
            this.rotationResetButton.Location = new System.Drawing.Point(61, -1);
            this.rotationResetButton.Name = "rotationResetButton";
            this.rotationResetButton.Size = new System.Drawing.Size(23, 23);
            this.rotationResetButton.TabIndex = 23;
            this.rotationResetButton.Text = "←";
            this.rotationResetButton.UseVisualStyleBackColor = true;
            this.rotationResetButton.Click += new System.EventHandler(this.RotationResetButton_Click);
            // 
            // scaleControl
            // 
            this.scaleControl.DecimalPlaces = 2;
            this.scaleControl.Increment = 0.01F;
            this.scaleControl.Location = new System.Drawing.Point(91, 27);
            this.scaleControl.Maximum = 100F;
            this.scaleControl.Minimum = 0.01F;
            this.scaleControl.Name = "scaleControl";
            this.scaleControl.Size = new System.Drawing.Size(192, 21);
            this.scaleControl.TabIndex = 22;
            // 
            // rotationControl
            // 
            this.rotationControl.DecimalPlaces = 2;
            this.rotationControl.Increment = 1F;
            this.rotationControl.Location = new System.Drawing.Point(91, 0);
            this.rotationControl.Maximum = 180F;
            this.rotationControl.Minimum = -180F;
            this.rotationControl.Name = "rotationControl";
            this.rotationControl.Size = new System.Drawing.Size(192, 21);
            this.rotationControl.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 12);
            this.label5.TabIndex = 20;
            this.label5.Text = "Scale";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 2);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 12);
            this.label6.TabIndex = 19;
            this.label6.Text = "Rotation";
            // 
            // GizmoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "GizmoControl";
            this.Size = new System.Drawing.Size(283, 180);
            this.panel.ResumeLayout(false);
            this.objectPanel.ResumeLayout(false);
            this.objectPanel.PerformLayout();
            this.bindingPanel.ResumeLayout(false);
            this.bindingPanel.PerformLayout();
            this.positionPanel.ResumeLayout(false);
            this.positionPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox targetTextBox;
        private System.Windows.Forms.Button targetAddButton;
        private System.Windows.Forms.Button targetRemoveButton;
        private System.Windows.Forms.Label label2;
        private Components.Vector3Control positionControl;
        private System.Windows.Forms.Label label3;
        private Components.Vector3Control gridPositionControl;
        private System.Windows.Forms.Label label4;
        private Components.ComboBox bindingComboBox;
        private System.Windows.Forms.Button nodeNameClearButton;
        private System.Windows.Forms.Panel positionPanel;
        private Components.StackPanel panel;
        private System.Windows.Forms.Panel objectPanel;
        private System.Windows.Forms.Panel bindingPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button scaleResetButton;
        private System.Windows.Forms.Button rotationResetButton;
        private Components.Vector3Control scaleControl;
        private Components.Vector3Control rotationControl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox fromGroundCheckBox;
    }
}

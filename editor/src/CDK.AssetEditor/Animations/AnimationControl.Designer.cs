
namespace CDK.Assets.Animations
{
    partial class AnimationControl
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
            this.target1RadioButton = new System.Windows.Forms.RadioButton();
            this.target2RadioButton = new System.Windows.Forms.RadioButton();
            this.target3RadioButton = new System.Windows.Forms.RadioButton();
            this.targetControl = new CDK.Assets.Scenes.GizmoControl();
            this.keyListBox = new System.Windows.Forms.CheckedListBox();
            this.panel = new CDK.Assets.Components.StackPanel();
            this.objectControl = new CDK.Assets.Scenes.SceneObjectControl();
            this.targetPanel = new System.Windows.Forms.Panel();
            this.originRadioButton = new System.Windows.Forms.RadioButton();
            this.targetCheckBox = new System.Windows.Forms.CheckBox();
            this.panel.SuspendLayout();
            this.targetPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // target1RadioButton
            // 
            this.target1RadioButton.AutoSize = true;
            this.target1RadioButton.Location = new System.Drawing.Point(65, 3);
            this.target1RadioButton.Name = "target1RadioButton";
            this.target1RadioButton.Size = new System.Drawing.Size(69, 16);
            this.target1RadioButton.TabIndex = 0;
            this.target1RadioButton.TabStop = true;
            this.target1RadioButton.Text = "Target 1";
            this.target1RadioButton.UseVisualStyleBackColor = true;
            this.target1RadioButton.CheckedChanged += new System.EventHandler(this.Target1RadioButton_CheckedChanged);
            // 
            // target2RadioButton
            // 
            this.target2RadioButton.AutoSize = true;
            this.target2RadioButton.Location = new System.Drawing.Point(140, 3);
            this.target2RadioButton.Name = "target2RadioButton";
            this.target2RadioButton.Size = new System.Drawing.Size(69, 16);
            this.target2RadioButton.TabIndex = 1;
            this.target2RadioButton.TabStop = true;
            this.target2RadioButton.Text = "Target 2";
            this.target2RadioButton.UseVisualStyleBackColor = true;
            this.target2RadioButton.CheckedChanged += new System.EventHandler(this.Target2RadioButton_CheckedChanged);
            // 
            // target3RadioButton
            // 
            this.target3RadioButton.AutoSize = true;
            this.target3RadioButton.Location = new System.Drawing.Point(215, 3);
            this.target3RadioButton.Name = "target3RadioButton";
            this.target3RadioButton.Size = new System.Drawing.Size(69, 16);
            this.target3RadioButton.TabIndex = 2;
            this.target3RadioButton.TabStop = true;
            this.target3RadioButton.Text = "Target 3";
            this.target3RadioButton.UseVisualStyleBackColor = true;
            this.target3RadioButton.CheckedChanged += new System.EventHandler(this.Target3RadioButton_CheckedChanged);
            // 
            // targetControl
            // 
            this.targetControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.targetControl.Location = new System.Drawing.Point(3, 264);
            this.targetControl.Name = "targetControl";
            this.targetControl.Size = new System.Drawing.Size(314, 180);
            this.targetControl.TabIndex = 21;
            // 
            // keyListBox
            // 
            this.keyListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.keyListBox.FormattingEnabled = true;
            this.keyListBox.IntegralHeight = false;
            this.keyListBox.Location = new System.Drawing.Point(0, 450);
            this.keyListBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.keyListBox.Name = "keyListBox";
            this.keyListBox.Size = new System.Drawing.Size(320, 189);
            this.keyListBox.TabIndex = 22;
            this.keyListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.KeyListBox_ItemCheck);
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.AutoSize = true;
            this.panel.Controls.Add(this.objectControl);
            this.panel.Controls.Add(this.targetPanel);
            this.panel.Controls.Add(this.targetControl);
            this.panel.Controls.Add(this.keyListBox);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(320, 639);
            this.panel.TabIndex = 23;
            this.panel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // objectControl
            // 
            this.objectControl.Location = new System.Drawing.Point(3, 3);
            this.objectControl.Name = "objectControl";
            this.objectControl.Size = new System.Drawing.Size(314, 205);
            this.objectControl.TabIndex = 0;
            // 
            // targetPanel
            // 
            this.targetPanel.Controls.Add(this.originRadioButton);
            this.targetPanel.Controls.Add(this.targetCheckBox);
            this.targetPanel.Controls.Add(this.target2RadioButton);
            this.targetPanel.Controls.Add(this.target1RadioButton);
            this.targetPanel.Controls.Add(this.target3RadioButton);
            this.targetPanel.Location = new System.Drawing.Point(0, 214);
            this.targetPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.targetPanel.Name = "targetPanel";
            this.targetPanel.Size = new System.Drawing.Size(320, 44);
            this.targetPanel.TabIndex = 1;
            // 
            // originRadioButton
            // 
            this.originRadioButton.AutoSize = true;
            this.originRadioButton.Location = new System.Drawing.Point(3, 3);
            this.originRadioButton.Name = "originRadioButton";
            this.originRadioButton.Size = new System.Drawing.Size(56, 16);
            this.originRadioButton.TabIndex = 4;
            this.originRadioButton.TabStop = true;
            this.originRadioButton.Text = "Origin";
            this.originRadioButton.UseVisualStyleBackColor = true;
            this.originRadioButton.CheckedChanged += new System.EventHandler(this.OriginRadioButton_CheckedChanged);
            // 
            // targetCheckBox
            // 
            this.targetCheckBox.AutoSize = true;
            this.targetCheckBox.Location = new System.Drawing.Point(3, 25);
            this.targetCheckBox.Name = "targetCheckBox";
            this.targetCheckBox.Size = new System.Drawing.Size(82, 16);
            this.targetCheckBox.TabIndex = 3;
            this.targetCheckBox.Text = "Transform";
            this.targetCheckBox.UseVisualStyleBackColor = true;
            // 
            // AnimationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Name = "AnimationControl";
            this.Size = new System.Drawing.Size(320, 639);
            this.panel.ResumeLayout(false);
            this.targetPanel.ResumeLayout(false);
            this.targetPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton target1RadioButton;
        private System.Windows.Forms.RadioButton target2RadioButton;
        private System.Windows.Forms.RadioButton target3RadioButton;
        private Scenes.GizmoControl targetControl;
        private System.Windows.Forms.CheckedListBox keyListBox;
        private Assets.Components.StackPanel panel;
        private Scenes.SceneObjectControl objectControl;
        private System.Windows.Forms.Panel targetPanel;
        private System.Windows.Forms.RadioButton originRadioButton;
        private System.Windows.Forms.CheckBox targetCheckBox;
    }
}

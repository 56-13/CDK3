
namespace CDK.Assets.Animations.Particle
{
    partial class ParticleControl
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
            this.mainPanel = new CDK.Assets.Components.StackPanel();
            this.objectPanel = new CDK.Assets.Components.StackPanel();
            this.objectControl = new CDK.Assets.Scenes.SceneObjectControl();
            this.sourcePanel = new CDK.Assets.Components.StackPanel();
            this.sourcesControl = new CDK.Assets.Animations.Sources.AnimationSourcesControl();
            this.shapePanel = new CDK.Assets.Components.StackPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.shapeShellCheckBox = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.shapeTypeComboBox = new CDK.Assets.Components.ComboBox();
            this.shapeControlPanel = new System.Windows.Forms.Panel();
            this.instancePanel = new CDK.Assets.Components.StackPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.clearCheckBox = new System.Windows.Forms.CheckBox();
            this.finishCheckBox = new System.Windows.Forms.CheckBox();
            this.localSpaceCheckBox = new System.Windows.Forms.CheckBox();
            this.stretchRateUpDown = new CDK.Assets.Components.NumericUpDown();
            this.stretchRateLabel = new System.Windows.Forms.Label();
            this.viewComboBox = new CDK.Assets.Components.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.prewarmCheckBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.emissionMaxControl = new CDK.Assets.Components.TrackControl();
            this.label3 = new System.Windows.Forms.Label();
            this.emissionRateControl = new CDK.Assets.Components.TrackControl();
            this.lifeControl = new CDK.Assets.Animations.Components.AnimationFloatConstantControl();
            this.colorPanel = new CDK.Assets.Components.StackPanel();
            this.colorControl = new CDK.Assets.Animations.Components.AnimationColorControl();
            this.positionPanel = new CDK.Assets.Components.StackPanel();
            this.radialControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.xControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.yControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.zControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.billboardXControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.billboardYControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.billboardZControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.rotationPanel = new CDK.Assets.Components.StackPanel();
            this.rotationXControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.rotationYControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.rotationZControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.scalePanel = new CDK.Assets.Components.StackPanel();
            this.scaleEachCheckBox = new System.Windows.Forms.CheckBox();
            this.scaleXControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.scaleYControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.scaleZControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.subPanel = new CDK.Assets.Components.StackPanel();
            this.mainPanel.SuspendLayout();
            this.objectPanel.SuspendLayout();
            this.sourcePanel.SuspendLayout();
            this.shapePanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.instancePanel.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stretchRateUpDown)).BeginInit();
            this.colorPanel.SuspendLayout();
            this.positionPanel.SuspendLayout();
            this.rotationPanel.SuspendLayout();
            this.scalePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.AutoSize = true;
            this.mainPanel.Controls.Add(this.objectPanel);
            this.mainPanel.Controls.Add(this.sourcePanel);
            this.mainPanel.Controls.Add(this.shapePanel);
            this.mainPanel.Controls.Add(this.instancePanel);
            this.mainPanel.Controls.Add(this.colorPanel);
            this.mainPanel.Controls.Add(this.positionPanel);
            this.mainPanel.Controls.Add(this.rotationPanel);
            this.mainPanel.Controls.Add(this.scalePanel);
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(320, 1164);
            this.mainPanel.TabIndex = 0;
            this.mainPanel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // objectPanel
            // 
            this.objectPanel.AutoSize = true;
            this.objectPanel.Collapsible = true;
            this.objectPanel.Controls.Add(this.objectControl);
            this.objectPanel.Location = new System.Drawing.Point(0, 0);
            this.objectPanel.Margin = new System.Windows.Forms.Padding(0);
            this.objectPanel.Name = "objectPanel";
            this.objectPanel.Size = new System.Drawing.Size(320, 232);
            this.objectPanel.TabIndex = 133;
            this.objectPanel.Title = "Object";
            // 
            // objectControl
            // 
            this.objectControl.Location = new System.Drawing.Point(3, 24);
            this.objectControl.Name = "objectControl";
            this.objectControl.Size = new System.Drawing.Size(314, 205);
            this.objectControl.TabIndex = 15;
            // 
            // sourcePanel
            // 
            this.sourcePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourcePanel.AutoSize = true;
            this.sourcePanel.Collapsible = true;
            this.sourcePanel.Controls.Add(this.sourcesControl);
            this.sourcePanel.Location = new System.Drawing.Point(0, 232);
            this.sourcePanel.Margin = new System.Windows.Forms.Padding(0);
            this.sourcePanel.Name = "sourcePanel";
            this.sourcePanel.Size = new System.Drawing.Size(320, 127);
            this.sourcePanel.TabIndex = 7;
            this.sourcePanel.Title = "Source";
            // 
            // sourcesControl
            // 
            this.sourcesControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourcesControl.Location = new System.Drawing.Point(3, 24);
            this.sourcesControl.Name = "sourcesControl";
            this.sourcesControl.Size = new System.Drawing.Size(314, 100);
            this.sourcesControl.TabIndex = 0;
            // 
            // shapePanel
            // 
            this.shapePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shapePanel.AutoSize = true;
            this.shapePanel.Collapsible = true;
            this.shapePanel.Controls.Add(this.panel2);
            this.shapePanel.Controls.Add(this.shapeControlPanel);
            this.shapePanel.Location = new System.Drawing.Point(0, 359);
            this.shapePanel.Margin = new System.Windows.Forms.Padding(0);
            this.shapePanel.Name = "shapePanel";
            this.shapePanel.Size = new System.Drawing.Size(320, 113);
            this.shapePanel.TabIndex = 8;
            this.shapePanel.Title = "Shape";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.shapeShellCheckBox);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.shapeTypeComboBox);
            this.panel2.Location = new System.Drawing.Point(3, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(314, 20);
            this.panel2.TabIndex = 5;
            // 
            // shapeShellCheckBox
            // 
            this.shapeShellCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.shapeShellCheckBox.AutoSize = true;
            this.shapeShellCheckBox.Location = new System.Drawing.Point(262, 3);
            this.shapeShellCheckBox.Name = "shapeShellCheckBox";
            this.shapeShellCheckBox.Size = new System.Drawing.Size(52, 16);
            this.shapeShellCheckBox.TabIndex = 5;
            this.shapeShellCheckBox.Text = "Shell";
            this.shapeShellCheckBox.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(0, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 12);
            this.label8.TabIndex = 3;
            this.label8.Text = "Type";
            // 
            // shapeTypeComboBox
            // 
            this.shapeTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shapeTypeComboBox.FormattingEnabled = true;
            this.shapeTypeComboBox.Location = new System.Drawing.Point(90, 0);
            this.shapeTypeComboBox.Name = "shapeTypeComboBox";
            this.shapeTypeComboBox.Size = new System.Drawing.Size(166, 20);
            this.shapeTypeComboBox.TabIndex = 4;
            // 
            // shapeControlPanel
            // 
            this.shapeControlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shapeControlPanel.Location = new System.Drawing.Point(3, 50);
            this.shapeControlPanel.Name = "shapeControlPanel";
            this.shapeControlPanel.Size = new System.Drawing.Size(314, 60);
            this.shapeControlPanel.TabIndex = 6;
            // 
            // instancePanel
            // 
            this.instancePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.instancePanel.AutoSize = true;
            this.instancePanel.Collapsible = true;
            this.instancePanel.Controls.Add(this.panel3);
            this.instancePanel.Location = new System.Drawing.Point(0, 472);
            this.instancePanel.Margin = new System.Windows.Forms.Padding(0);
            this.instancePanel.Name = "instancePanel";
            this.instancePanel.Size = new System.Drawing.Size(320, 222);
            this.instancePanel.TabIndex = 9;
            this.instancePanel.Title = "Instance";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.clearCheckBox);
            this.panel3.Controls.Add(this.finishCheckBox);
            this.panel3.Controls.Add(this.localSpaceCheckBox);
            this.panel3.Controls.Add(this.stretchRateUpDown);
            this.panel3.Controls.Add(this.stretchRateLabel);
            this.panel3.Controls.Add(this.viewComboBox);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.prewarmCheckBox);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.emissionMaxControl);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.emissionRateControl);
            this.panel3.Controls.Add(this.lifeControl);
            this.panel3.Location = new System.Drawing.Point(3, 24);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(314, 195);
            this.panel3.TabIndex = 0;
            // 
            // clearCheckBox
            // 
            this.clearCheckBox.AutoSize = true;
            this.clearCheckBox.Location = new System.Drawing.Point(0, 179);
            this.clearCheckBox.Name = "clearCheckBox";
            this.clearCheckBox.Size = new System.Drawing.Size(139, 16);
            this.clearCheckBox.TabIndex = 127;
            this.clearCheckBox.Text = "Clear When Stopped";
            this.clearCheckBox.UseVisualStyleBackColor = true;
            // 
            // finishCheckBox
            // 
            this.finishCheckBox.AutoSize = true;
            this.finishCheckBox.Location = new System.Drawing.Point(145, 179);
            this.finishCheckBox.Name = "finishCheckBox";
            this.finishCheckBox.Size = new System.Drawing.Size(136, 16);
            this.finishCheckBox.TabIndex = 126;
            this.finishCheckBox.Text = "Finish After Emitting";
            this.finishCheckBox.UseVisualStyleBackColor = true;
            // 
            // localSpaceCheckBox
            // 
            this.localSpaceCheckBox.AutoSize = true;
            this.localSpaceCheckBox.Location = new System.Drawing.Point(145, 157);
            this.localSpaceCheckBox.Name = "localSpaceCheckBox";
            this.localSpaceCheckBox.Size = new System.Drawing.Size(95, 16);
            this.localSpaceCheckBox.TabIndex = 124;
            this.localSpaceCheckBox.Text = "Local Space";
            this.localSpaceCheckBox.UseVisualStyleBackColor = true;
            // 
            // stretchRateUpDown
            // 
            this.stretchRateUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stretchRateUpDown.DecimalPlaces = 3;
            this.stretchRateUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.stretchRateUpDown.Location = new System.Drawing.Point(254, 129);
            this.stretchRateUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.stretchRateUpDown.Name = "stretchRateUpDown";
            this.stretchRateUpDown.Size = new System.Drawing.Size(60, 21);
            this.stretchRateUpDown.TabIndex = 123;
            // 
            // stretchRateLabel
            // 
            this.stretchRateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stretchRateLabel.AutoSize = true;
            this.stretchRateLabel.Location = new System.Drawing.Point(88, 131);
            this.stretchRateLabel.Name = "stretchRateLabel";
            this.stretchRateLabel.Size = new System.Drawing.Size(73, 12);
            this.stretchRateLabel.TabIndex = 122;
            this.stretchRateLabel.Text = "Stretch Rate";
            // 
            // viewComboBox
            // 
            this.viewComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewComboBox.FormattingEnabled = true;
            this.viewComboBox.Location = new System.Drawing.Point(90, 103);
            this.viewComboBox.Name = "viewComboBox";
            this.viewComboBox.Size = new System.Drawing.Size(224, 20);
            this.viewComboBox.TabIndex = 121;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 107);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 12);
            this.label7.TabIndex = 120;
            this.label7.Text = "View";
            // 
            // prewarmCheckBox
            // 
            this.prewarmCheckBox.AutoSize = true;
            this.prewarmCheckBox.Location = new System.Drawing.Point(0, 157);
            this.prewarmCheckBox.Name = "prewarmCheckBox";
            this.prewarmCheckBox.Size = new System.Drawing.Size(75, 16);
            this.prewarmCheckBox.TabIndex = 119;
            this.prewarmCheckBox.Text = "Prewarm";
            this.prewarmCheckBox.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 12);
            this.label4.TabIndex = 114;
            this.label4.Text = "Emission Max";
            // 
            // emissionMaxControl
            // 
            this.emissionMaxControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.emissionMaxControl.Increment = 1F;
            this.emissionMaxControl.Location = new System.Drawing.Point(90, 78);
            this.emissionMaxControl.Maximum = 10000F;
            this.emissionMaxControl.Minimum = 0F;
            this.emissionMaxControl.Name = "emissionMaxControl";
            this.emissionMaxControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.emissionMaxControl.Size = new System.Drawing.Size(224, 21);
            this.emissionMaxControl.TabIndex = 113;
            this.emissionMaxControl.Value = 0F;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 12);
            this.label3.TabIndex = 112;
            this.label3.Text = "Emission Rate";
            // 
            // emissionRateControl
            // 
            this.emissionRateControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.emissionRateControl.DecimalPlaces = 2;
            this.emissionRateControl.Increment = 1F;
            this.emissionRateControl.Location = new System.Drawing.Point(90, 51);
            this.emissionRateControl.Maximum = 10000F;
            this.emissionRateControl.Minimum = 0F;
            this.emissionRateControl.Name = "emissionRateControl";
            this.emissionRateControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.emissionRateControl.Size = new System.Drawing.Size(224, 21);
            this.emissionRateControl.TabIndex = 111;
            this.emissionRateControl.Value = 0F;
            // 
            // lifeControl
            // 
            this.lifeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lifeControl.Increment = 0.1F;
            this.lifeControl.Location = new System.Drawing.Point(0, 0);
            this.lifeControl.Name = "lifeControl";
            this.lifeControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.None;
            this.lifeControl.Size = new System.Drawing.Size(314, 45);
            this.lifeControl.TabIndex = 110;
            this.lifeControl.Title = "Life";
            // 
            // colorPanel
            // 
            this.colorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorPanel.AutoSize = true;
            this.colorPanel.Collapsible = true;
            this.colorPanel.Controls.Add(this.colorControl);
            this.colorPanel.Location = new System.Drawing.Point(0, 694);
            this.colorPanel.Margin = new System.Windows.Forms.Padding(0);
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.Size = new System.Drawing.Size(320, 47);
            this.colorPanel.TabIndex = 10;
            this.colorPanel.Title = "Color";
            // 
            // colorControl
            // 
            this.colorControl.Location = new System.Drawing.Point(3, 24);
            this.colorControl.Name = "colorControl";
            this.colorControl.Size = new System.Drawing.Size(314, 20);
            this.colorControl.TabIndex = 115;
            this.colorControl.Title = "Color";
            // 
            // positionPanel
            // 
            this.positionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.positionPanel.AutoSize = true;
            this.positionPanel.Collapsible = true;
            this.positionPanel.Controls.Add(this.radialControl);
            this.positionPanel.Controls.Add(this.xControl);
            this.positionPanel.Controls.Add(this.yControl);
            this.positionPanel.Controls.Add(this.zControl);
            this.positionPanel.Controls.Add(this.billboardXControl);
            this.positionPanel.Controls.Add(this.billboardYControl);
            this.positionPanel.Controls.Add(this.billboardZControl);
            this.positionPanel.Location = new System.Drawing.Point(0, 741);
            this.positionPanel.Margin = new System.Windows.Forms.Padding(0);
            this.positionPanel.Name = "positionPanel";
            this.positionPanel.Size = new System.Drawing.Size(320, 203);
            this.positionPanel.TabIndex = 11;
            this.positionPanel.Title = "Position";
            // 
            // radialControl
            // 
            this.radialControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radialControl.Location = new System.Drawing.Point(3, 24);
            this.radialControl.Name = "radialControl";
            this.radialControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.radialControl.Size = new System.Drawing.Size(314, 20);
            this.radialControl.TabIndex = 112;
            this.radialControl.Title = "Radial";
            // 
            // xControl
            // 
            this.xControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xControl.Location = new System.Drawing.Point(3, 50);
            this.xControl.Name = "xControl";
            this.xControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.xControl.Size = new System.Drawing.Size(314, 20);
            this.xControl.TabIndex = 113;
            this.xControl.Title = "X";
            // 
            // yControl
            // 
            this.yControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.yControl.Location = new System.Drawing.Point(3, 76);
            this.yControl.Name = "yControl";
            this.yControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.yControl.Size = new System.Drawing.Size(314, 20);
            this.yControl.TabIndex = 114;
            this.yControl.Title = "Y";
            // 
            // zControl
            // 
            this.zControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zControl.Location = new System.Drawing.Point(3, 102);
            this.zControl.Name = "zControl";
            this.zControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.zControl.Size = new System.Drawing.Size(314, 20);
            this.zControl.TabIndex = 115;
            this.zControl.Title = "Z";
            // 
            // billboardXControl
            // 
            this.billboardXControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.billboardXControl.Location = new System.Drawing.Point(3, 128);
            this.billboardXControl.Name = "billboardXControl";
            this.billboardXControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.billboardXControl.Size = new System.Drawing.Size(314, 20);
            this.billboardXControl.TabIndex = 116;
            this.billboardXControl.Title = "Billboard X";
            // 
            // billboardYControl
            // 
            this.billboardYControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.billboardYControl.Location = new System.Drawing.Point(3, 154);
            this.billboardYControl.Name = "billboardYControl";
            this.billboardYControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.billboardYControl.Size = new System.Drawing.Size(314, 20);
            this.billboardYControl.TabIndex = 117;
            this.billboardYControl.Title = "Billboard Y";
            // 
            // billboardZControl
            // 
            this.billboardZControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.billboardZControl.Location = new System.Drawing.Point(3, 180);
            this.billboardZControl.Name = "billboardZControl";
            this.billboardZControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.billboardZControl.Size = new System.Drawing.Size(314, 20);
            this.billboardZControl.TabIndex = 118;
            this.billboardZControl.Title = "Billboard Z";
            // 
            // rotationPanel
            // 
            this.rotationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rotationPanel.AutoSize = true;
            this.rotationPanel.Collapsible = true;
            this.rotationPanel.Controls.Add(this.rotationXControl);
            this.rotationPanel.Controls.Add(this.rotationYControl);
            this.rotationPanel.Controls.Add(this.rotationZControl);
            this.rotationPanel.Location = new System.Drawing.Point(0, 944);
            this.rotationPanel.Margin = new System.Windows.Forms.Padding(0);
            this.rotationPanel.Name = "rotationPanel";
            this.rotationPanel.Size = new System.Drawing.Size(320, 99);
            this.rotationPanel.TabIndex = 12;
            this.rotationPanel.Title = "Rotation";
            // 
            // rotationXControl
            // 
            this.rotationXControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rotationXControl.Location = new System.Drawing.Point(3, 24);
            this.rotationXControl.Name = "rotationXControl";
            this.rotationXControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.rotationXControl.Size = new System.Drawing.Size(314, 20);
            this.rotationXControl.TabIndex = 109;
            this.rotationXControl.Title = "X";
            // 
            // rotationYControl
            // 
            this.rotationYControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rotationYControl.Location = new System.Drawing.Point(3, 50);
            this.rotationYControl.Name = "rotationYControl";
            this.rotationYControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.rotationYControl.Size = new System.Drawing.Size(314, 20);
            this.rotationYControl.TabIndex = 110;
            this.rotationYControl.Title = "Y";
            // 
            // rotationZControl
            // 
            this.rotationZControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rotationZControl.Location = new System.Drawing.Point(3, 76);
            this.rotationZControl.Name = "rotationZControl";
            this.rotationZControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.rotationZControl.Size = new System.Drawing.Size(314, 20);
            this.rotationZControl.TabIndex = 111;
            this.rotationZControl.Title = "Z";
            // 
            // scalePanel
            // 
            this.scalePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scalePanel.AutoSize = true;
            this.scalePanel.Collapsible = true;
            this.scalePanel.Controls.Add(this.scaleEachCheckBox);
            this.scalePanel.Controls.Add(this.scaleXControl);
            this.scalePanel.Controls.Add(this.scaleYControl);
            this.scalePanel.Controls.Add(this.scaleZControl);
            this.scalePanel.Location = new System.Drawing.Point(0, 1043);
            this.scalePanel.Margin = new System.Windows.Forms.Padding(0);
            this.scalePanel.Name = "scalePanel";
            this.scalePanel.Size = new System.Drawing.Size(320, 121);
            this.scalePanel.TabIndex = 13;
            this.scalePanel.Title = "Scale";
            // 
            // scaleEachCheckBox
            // 
            this.scaleEachCheckBox.AutoSize = true;
            this.scaleEachCheckBox.Checked = true;
            this.scaleEachCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.scaleEachCheckBox.Location = new System.Drawing.Point(6, 24);
            this.scaleEachCheckBox.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.scaleEachCheckBox.Name = "scaleEachCheckBox";
            this.scaleEachCheckBox.Size = new System.Drawing.Size(311, 16);
            this.scaleEachCheckBox.TabIndex = 132;
            this.scaleEachCheckBox.Text = "Each";
            this.scaleEachCheckBox.UseVisualStyleBackColor = true;
            // 
            // scaleXControl
            // 
            this.scaleXControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scaleXControl.Increment = 0.01F;
            this.scaleXControl.Location = new System.Drawing.Point(3, 46);
            this.scaleXControl.Name = "scaleXControl";
            this.scaleXControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Log;
            this.scaleXControl.Size = new System.Drawing.Size(314, 20);
            this.scaleXControl.TabIndex = 109;
            this.scaleXControl.Title = "X";
            // 
            // scaleYControl
            // 
            this.scaleYControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scaleYControl.Increment = 0.01F;
            this.scaleYControl.Location = new System.Drawing.Point(3, 72);
            this.scaleYControl.Name = "scaleYControl";
            this.scaleYControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Log;
            this.scaleYControl.Size = new System.Drawing.Size(314, 20);
            this.scaleYControl.TabIndex = 110;
            this.scaleYControl.Title = "Y";
            // 
            // scaleZControl
            // 
            this.scaleZControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scaleZControl.Increment = 0.01F;
            this.scaleZControl.Location = new System.Drawing.Point(3, 98);
            this.scaleZControl.Name = "scaleZControl";
            this.scaleZControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Log;
            this.scaleZControl.Size = new System.Drawing.Size(314, 20);
            this.scaleZControl.TabIndex = 111;
            this.scaleZControl.Title = "Z";
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.mainPanel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.subPanel);
            this.splitContainer.Panel2Collapsed = true;
            this.splitContainer.Size = new System.Drawing.Size(320, 1164);
            this.splitContainer.SplitterDistance = 160;
            this.splitContainer.TabIndex = 1;
            // 
            // subPanel
            // 
            this.subPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subPanel.AutoSize = true;
            this.subPanel.Location = new System.Drawing.Point(0, 0);
            this.subPanel.Name = "subPanel";
            this.subPanel.Size = new System.Drawing.Size(156, 0);
            this.subPanel.TabIndex = 0;
            this.subPanel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
            // 
            // ParticleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "ParticleControl";
            this.Size = new System.Drawing.Size(320, 1164);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.objectPanel.ResumeLayout(false);
            this.sourcePanel.ResumeLayout(false);
            this.shapePanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.instancePanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stretchRateUpDown)).EndInit();
            this.colorPanel.ResumeLayout(false);
            this.positionPanel.ResumeLayout(false);
            this.rotationPanel.ResumeLayout(false);
            this.scalePanel.ResumeLayout(false);
            this.scalePanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Assets.Components.StackPanel mainPanel;
        private Assets.Components.StackPanel sourcePanel;
        private Assets.Components.StackPanel shapePanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label8;
        private Assets.Components.ComboBox shapeTypeComboBox;
        private System.Windows.Forms.Panel shapeControlPanel;
        private Assets.Components.StackPanel instancePanel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox localSpaceCheckBox;
        private Assets.Components.NumericUpDown stretchRateUpDown;
        private System.Windows.Forms.Label stretchRateLabel;
        private Assets.Components.ComboBox viewComboBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox prewarmCheckBox;
        private System.Windows.Forms.Label label4;
        private Assets.Components.TrackControl emissionMaxControl;
        private System.Windows.Forms.Label label3;
        private Assets.Components.TrackControl emissionRateControl;
        private Components.AnimationFloatConstantControl lifeControl;
        private Assets.Components.StackPanel colorPanel;
        private Assets.Components.StackPanel positionPanel;
        private Components.AnimationFloatControl radialControl;
        private Components.AnimationFloatControl xControl;
        private Components.AnimationFloatControl yControl;
        private Components.AnimationFloatControl zControl;
        private Components.AnimationFloatControl billboardXControl;
        private Components.AnimationFloatControl billboardYControl;
        private Components.AnimationFloatControl billboardZControl;
        private Assets.Components.StackPanel rotationPanel;
        private Components.AnimationFloatControl rotationXControl;
        private Components.AnimationFloatControl rotationYControl;
        private Components.AnimationFloatControl rotationZControl;
        private Assets.Components.StackPanel scalePanel;
        private Components.AnimationFloatControl scaleXControl;
        private Components.AnimationFloatControl scaleYControl;
        private Components.AnimationFloatControl scaleZControl;
        private System.Windows.Forms.CheckBox clearCheckBox;
        private System.Windows.Forms.CheckBox finishCheckBox;
        private System.Windows.Forms.CheckBox shapeShellCheckBox;
        private System.Windows.Forms.CheckBox scaleEachCheckBox;
        private Sources.AnimationSourcesControl sourcesControl;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Assets.Components.StackPanel subPanel;
        private Components.AnimationColorControl colorControl;
        private Assets.Components.StackPanel objectPanel;
        private Scenes.SceneObjectControl objectControl;
    }
}

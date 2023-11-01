
namespace CDK.Assets.Animations.Trail
{
    partial class TrailControl
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.mainPanel = new CDK.Assets.Components.StackPanel();
            this.sourcePanel = new CDK.Assets.Components.StackPanel();
            this.sourcesControl = new CDK.Assets.Animations.Sources.AnimationSourcesControl();
            this.instancePanel = new CDK.Assets.Components.StackPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.distanceControl = new CDK.Assets.Components.TrackControl();
            this.label4 = new System.Windows.Forms.Label();
            this.emissionCheckBox = new System.Windows.Forms.CheckBox();
            this.localSpaceCheckBox = new System.Windows.Forms.CheckBox();
            this.billboardCheckBox = new System.Windows.Forms.CheckBox();
            this.emissionPanel = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.emissionSmoothnessControl = new CDK.Assets.Components.TrackControl();
            this.emissionLifeControl = new CDK.Assets.Components.TrackControl();
            this.label3 = new System.Windows.Forms.Label();
            this.repeatScalePanel = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.repeatScaleControl = new CDK.Assets.Components.TrackControl();
            this.colorPanel = new CDK.Assets.Components.StackPanel();
            this.colorControl = new CDK.Assets.Animations.Components.AnimationColorControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.colorLoopControl = new CDK.Assets.Animations.Components.AnimationLoopControl();
            this.colorDurationControl = new CDK.Assets.Components.TrackControl();
            this.label1 = new System.Windows.Forms.Label();
            this.rotationPanel = new CDK.Assets.Components.StackPanel();
            this.rotationControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.rotationLoopControl = new CDK.Assets.Animations.Components.AnimationLoopControl();
            this.rotationDurationControl = new CDK.Assets.Components.TrackControl();
            this.label6 = new System.Windows.Forms.Label();
            this.scalePanel = new CDK.Assets.Components.StackPanel();
            this.scaleControl = new CDK.Assets.Animations.Components.AnimationFloatControl();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.scaleLoopControl = new CDK.Assets.Animations.Components.AnimationLoopControl();
            this.scaleDurationControl = new CDK.Assets.Components.TrackControl();
            this.label8 = new System.Windows.Forms.Label();
            this.subPanel = new CDK.Assets.Components.StackPanel();
            this.objectPanel = new CDK.Assets.Components.StackPanel();
            this.objectControl = new CDK.Assets.Scenes.SceneObjectControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.sourcePanel.SuspendLayout();
            this.instancePanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.emissionPanel.SuspendLayout();
            this.repeatScalePanel.SuspendLayout();
            this.colorPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.rotationPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.scalePanel.SuspendLayout();
            this.panel4.SuspendLayout();
            this.objectPanel.SuspendLayout();
            this.SuspendLayout();
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
            this.splitContainer.Size = new System.Drawing.Size(320, 813);
            this.splitContainer.SplitterDistance = 160;
            this.splitContainer.TabIndex = 1;
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.AutoSize = true;
            this.mainPanel.Controls.Add(this.objectPanel);
            this.mainPanel.Controls.Add(this.sourcePanel);
            this.mainPanel.Controls.Add(this.instancePanel);
            this.mainPanel.Controls.Add(this.colorPanel);
            this.mainPanel.Controls.Add(this.rotationPanel);
            this.mainPanel.Controls.Add(this.scalePanel);
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(320, 813);
            this.mainPanel.TabIndex = 0;
            this.mainPanel.SizeChanged += new System.EventHandler(this.Panel_SizeChanged);
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
            this.sourcesControl.Location = new System.Drawing.Point(3, 24);
            this.sourcesControl.Name = "sourcesControl";
            this.sourcesControl.Size = new System.Drawing.Size(314, 100);
            this.sourcesControl.TabIndex = 0;
            this.sourcesControl.UsingBones = true;
            // 
            // instancePanel
            // 
            this.instancePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.instancePanel.AutoSize = true;
            this.instancePanel.Collapsible = true;
            this.instancePanel.Controls.Add(this.panel3);
            this.instancePanel.Controls.Add(this.emissionPanel);
            this.instancePanel.Controls.Add(this.repeatScalePanel);
            this.instancePanel.Location = new System.Drawing.Point(0, 359);
            this.instancePanel.Margin = new System.Windows.Forms.Padding(0);
            this.instancePanel.Name = "instancePanel";
            this.instancePanel.Size = new System.Drawing.Size(320, 151);
            this.instancePanel.TabIndex = 9;
            this.instancePanel.Title = "Instance";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.distanceControl);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.emissionCheckBox);
            this.panel3.Controls.Add(this.localSpaceCheckBox);
            this.panel3.Controls.Add(this.billboardCheckBox);
            this.panel3.Location = new System.Drawing.Point(3, 24);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(314, 43);
            this.panel3.TabIndex = 0;
            // 
            // distanceControl
            // 
            this.distanceControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.distanceControl.Increment = 1F;
            this.distanceControl.Location = new System.Drawing.Point(90, 0);
            this.distanceControl.Maximum = 100F;
            this.distanceControl.Minimum = 1F;
            this.distanceControl.Name = "distanceControl";
            this.distanceControl.Size = new System.Drawing.Size(224, 21);
            this.distanceControl.TabIndex = 134;
            this.distanceControl.Value = 10F;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 12);
            this.label4.TabIndex = 133;
            this.label4.Text = "Distance";
            // 
            // emissionCheckBox
            // 
            this.emissionCheckBox.AutoSize = true;
            this.emissionCheckBox.Location = new System.Drawing.Point(180, 27);
            this.emissionCheckBox.Name = "emissionCheckBox";
            this.emissionCheckBox.Size = new System.Drawing.Size(77, 16);
            this.emissionCheckBox.TabIndex = 132;
            this.emissionCheckBox.Text = "Emission";
            this.emissionCheckBox.UseVisualStyleBackColor = true;
            // 
            // localSpaceCheckBox
            // 
            this.localSpaceCheckBox.AutoSize = true;
            this.localSpaceCheckBox.Location = new System.Drawing.Point(79, 27);
            this.localSpaceCheckBox.Name = "localSpaceCheckBox";
            this.localSpaceCheckBox.Size = new System.Drawing.Size(95, 16);
            this.localSpaceCheckBox.TabIndex = 130;
            this.localSpaceCheckBox.Text = "Local Space";
            this.localSpaceCheckBox.UseVisualStyleBackColor = true;
            // 
            // billboardCheckBox
            // 
            this.billboardCheckBox.AutoSize = true;
            this.billboardCheckBox.Location = new System.Drawing.Point(0, 27);
            this.billboardCheckBox.Name = "billboardCheckBox";
            this.billboardCheckBox.Size = new System.Drawing.Size(73, 16);
            this.billboardCheckBox.TabIndex = 129;
            this.billboardCheckBox.Text = "Billboard";
            this.billboardCheckBox.UseVisualStyleBackColor = true;
            // 
            // emissionPanel
            // 
            this.emissionPanel.Controls.Add(this.label10);
            this.emissionPanel.Controls.Add(this.emissionSmoothnessControl);
            this.emissionPanel.Controls.Add(this.emissionLifeControl);
            this.emissionPanel.Controls.Add(this.label3);
            this.emissionPanel.Location = new System.Drawing.Point(3, 73);
            this.emissionPanel.Name = "emissionPanel";
            this.emissionPanel.Size = new System.Drawing.Size(314, 48);
            this.emissionPanel.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(0, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(76, 12);
            this.label10.TabIndex = 135;
            this.label10.Text = "Smoothness";
            // 
            // emissionSmoothnessControl
            // 
            this.emissionSmoothnessControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.emissionSmoothnessControl.Increment = 1F;
            this.emissionSmoothnessControl.Location = new System.Drawing.Point(90, 27);
            this.emissionSmoothnessControl.Maximum = 10F;
            this.emissionSmoothnessControl.Minimum = 0F;
            this.emissionSmoothnessControl.Name = "emissionSmoothnessControl";
            this.emissionSmoothnessControl.Size = new System.Drawing.Size(224, 21);
            this.emissionSmoothnessControl.TabIndex = 135;
            this.emissionSmoothnessControl.Value = 5F;
            // 
            // emissionLifeControl
            // 
            this.emissionLifeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.emissionLifeControl.DecimalPlaces = 2;
            this.emissionLifeControl.Increment = 0.1F;
            this.emissionLifeControl.Location = new System.Drawing.Point(90, 0);
            this.emissionLifeControl.Maximum = 10F;
            this.emissionLifeControl.Minimum = 0F;
            this.emissionLifeControl.Name = "emissionLifeControl";
            this.emissionLifeControl.Size = new System.Drawing.Size(224, 21);
            this.emissionLifeControl.TabIndex = 111;
            this.emissionLifeControl.Value = 0F;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 12);
            this.label3.TabIndex = 112;
            this.label3.Text = "Life";
            // 
            // repeatScalePanel
            // 
            this.repeatScalePanel.Controls.Add(this.label9);
            this.repeatScalePanel.Controls.Add(this.repeatScaleControl);
            this.repeatScalePanel.Location = new System.Drawing.Point(3, 127);
            this.repeatScalePanel.Name = "repeatScalePanel";
            this.repeatScalePanel.Size = new System.Drawing.Size(314, 21);
            this.repeatScalePanel.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(0, 2);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 12);
            this.label9.TabIndex = 135;
            this.label9.Text = "Repeat Scale";
            // 
            // repeatScaleControl
            // 
            this.repeatScaleControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.repeatScaleControl.DecimalPlaces = 2;
            this.repeatScaleControl.Increment = 0.01F;
            this.repeatScaleControl.Location = new System.Drawing.Point(90, 0);
            this.repeatScaleControl.Maximum = 10F;
            this.repeatScaleControl.Minimum = 0F;
            this.repeatScaleControl.Name = "repeatScaleControl";
            this.repeatScaleControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Log;
            this.repeatScaleControl.Size = new System.Drawing.Size(224, 21);
            this.repeatScaleControl.TabIndex = 131;
            this.repeatScaleControl.Value = 1F;
            // 
            // colorPanel
            // 
            this.colorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorPanel.AutoSize = true;
            this.colorPanel.Collapsible = true;
            this.colorPanel.Controls.Add(this.colorControl);
            this.colorPanel.Controls.Add(this.panel1);
            this.colorPanel.Location = new System.Drawing.Point(0, 510);
            this.colorPanel.Margin = new System.Windows.Forms.Padding(0);
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.Size = new System.Drawing.Size(320, 101);
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
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.colorLoopControl);
            this.panel1.Controls.Add(this.colorDurationControl);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(3, 50);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(314, 48);
            this.panel1.TabIndex = 111;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Loop";
            // 
            // colorLoopControl
            // 
            this.colorLoopControl.Location = new System.Drawing.Point(90, 27);
            this.colorLoopControl.Name = "colorLoopControl";
            this.colorLoopControl.Size = new System.Drawing.Size(208, 21);
            this.colorLoopControl.TabIndex = 2;
            // 
            // colorDurationControl
            // 
            this.colorDurationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.colorDurationControl.DecimalPlaces = 2;
            this.colorDurationControl.Increment = 0.1F;
            this.colorDurationControl.Location = new System.Drawing.Point(90, 0);
            this.colorDurationControl.Maximum = 10F;
            this.colorDurationControl.Minimum = 0F;
            this.colorDurationControl.Name = "colorDurationControl";
            this.colorDurationControl.Size = new System.Drawing.Size(224, 21);
            this.colorDurationControl.TabIndex = 1;
            this.colorDurationControl.Value = 0F;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Duration";
            // 
            // rotationPanel
            // 
            this.rotationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rotationPanel.AutoSize = true;
            this.rotationPanel.Collapsible = true;
            this.rotationPanel.Controls.Add(this.rotationControl);
            this.rotationPanel.Controls.Add(this.panel2);
            this.rotationPanel.Location = new System.Drawing.Point(0, 611);
            this.rotationPanel.Margin = new System.Windows.Forms.Padding(0);
            this.rotationPanel.Name = "rotationPanel";
            this.rotationPanel.Size = new System.Drawing.Size(320, 101);
            this.rotationPanel.TabIndex = 12;
            this.rotationPanel.Title = "Rotation";
            // 
            // rotationControl
            // 
            this.rotationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rotationControl.Location = new System.Drawing.Point(3, 24);
            this.rotationControl.Name = "rotationControl";
            this.rotationControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Linear;
            this.rotationControl.Size = new System.Drawing.Size(314, 20);
            this.rotationControl.TabIndex = 109;
            this.rotationControl.Title = "Rotation";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.rotationLoopControl);
            this.panel2.Controls.Add(this.rotationDurationControl);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Location = new System.Drawing.Point(3, 50);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(314, 48);
            this.panel2.TabIndex = 112;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "Loop";
            // 
            // rotationLoopControl
            // 
            this.rotationLoopControl.Location = new System.Drawing.Point(90, 27);
            this.rotationLoopControl.Name = "rotationLoopControl";
            this.rotationLoopControl.Size = new System.Drawing.Size(208, 21);
            this.rotationLoopControl.TabIndex = 2;
            // 
            // rotationDurationControl
            // 
            this.rotationDurationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rotationDurationControl.DecimalPlaces = 2;
            this.rotationDurationControl.Increment = 0.1F;
            this.rotationDurationControl.Location = new System.Drawing.Point(90, 0);
            this.rotationDurationControl.Maximum = 10F;
            this.rotationDurationControl.Minimum = 0F;
            this.rotationDurationControl.Name = "rotationDurationControl";
            this.rotationDurationControl.Size = new System.Drawing.Size(224, 21);
            this.rotationDurationControl.TabIndex = 1;
            this.rotationDurationControl.Value = 0F;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 2);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "Duration";
            // 
            // scalePanel
            // 
            this.scalePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scalePanel.AutoSize = true;
            this.scalePanel.Collapsible = true;
            this.scalePanel.Controls.Add(this.scaleControl);
            this.scalePanel.Controls.Add(this.panel4);
            this.scalePanel.Location = new System.Drawing.Point(0, 712);
            this.scalePanel.Margin = new System.Windows.Forms.Padding(0);
            this.scalePanel.Name = "scalePanel";
            this.scalePanel.Size = new System.Drawing.Size(320, 101);
            this.scalePanel.TabIndex = 111;
            this.scalePanel.Title = "Scale";
            // 
            // scaleControl
            // 
            this.scaleControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scaleControl.Increment = 0.01F;
            this.scaleControl.Location = new System.Drawing.Point(3, 24);
            this.scaleControl.Name = "scaleControl";
            this.scaleControl.ScaleType = CDK.Assets.Components.TrackBarScaleType.Log;
            this.scaleControl.Size = new System.Drawing.Size(314, 20);
            this.scaleControl.TabIndex = 110;
            this.scaleControl.Title = "Scale";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label7);
            this.panel4.Controls.Add(this.scaleLoopControl);
            this.panel4.Controls.Add(this.scaleDurationControl);
            this.panel4.Controls.Add(this.label8);
            this.panel4.Location = new System.Drawing.Point(3, 50);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(314, 48);
            this.panel4.TabIndex = 113;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 12);
            this.label7.TabIndex = 3;
            this.label7.Text = "Loop";
            // 
            // scaleLoopControl
            // 
            this.scaleLoopControl.Location = new System.Drawing.Point(90, 27);
            this.scaleLoopControl.Name = "scaleLoopControl";
            this.scaleLoopControl.Size = new System.Drawing.Size(208, 21);
            this.scaleLoopControl.TabIndex = 2;
            // 
            // scaleDurationControl
            // 
            this.scaleDurationControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scaleDurationControl.DecimalPlaces = 2;
            this.scaleDurationControl.Increment = 0.1F;
            this.scaleDurationControl.Location = new System.Drawing.Point(90, 0);
            this.scaleDurationControl.Maximum = 10F;
            this.scaleDurationControl.Minimum = 0F;
            this.scaleDurationControl.Name = "scaleDurationControl";
            this.scaleDurationControl.Size = new System.Drawing.Size(224, 21);
            this.scaleDurationControl.TabIndex = 1;
            this.scaleDurationControl.Value = 0F;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(0, 2);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "Duration";
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
            // objectPanel
            // 
            this.objectPanel.AutoSize = true;
            this.objectPanel.Collapsible = true;
            this.objectPanel.Controls.Add(this.objectControl);
            this.objectPanel.Location = new System.Drawing.Point(0, 0);
            this.objectPanel.Margin = new System.Windows.Forms.Padding(0);
            this.objectPanel.Name = "objectPanel";
            this.objectPanel.Size = new System.Drawing.Size(320, 232);
            this.objectPanel.TabIndex = 134;
            this.objectPanel.Title = "Object";
            // 
            // objectControl
            // 
            this.objectControl.Location = new System.Drawing.Point(3, 24);
            this.objectControl.Name = "objectControl";
            this.objectControl.Size = new System.Drawing.Size(314, 205);
            this.objectControl.TabIndex = 15;
            // 
            // TrailControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "TrailControl";
            this.Size = new System.Drawing.Size(320, 813);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.sourcePanel.ResumeLayout(false);
            this.instancePanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.emissionPanel.ResumeLayout(false);
            this.emissionPanel.PerformLayout();
            this.repeatScalePanel.ResumeLayout(false);
            this.repeatScalePanel.PerformLayout();
            this.colorPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.rotationPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.scalePanel.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.objectPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Assets.Components.StackPanel mainPanel;
        private Assets.Components.StackPanel sourcePanel;
        private Assets.Components.StackPanel instancePanel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label3;
        private Assets.Components.TrackControl emissionLifeControl;
        private Assets.Components.StackPanel colorPanel;
        private Assets.Components.StackPanel rotationPanel;
        private Components.AnimationFloatControl rotationControl;
        private Sources.AnimationSourcesControl sourcesControl;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Assets.Components.StackPanel subPanel;
        private System.Windows.Forms.CheckBox localSpaceCheckBox;
        private System.Windows.Forms.CheckBox billboardCheckBox;
        private Components.AnimationColorControl colorControl;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private Components.AnimationLoopControl colorLoopControl;
        private Assets.Components.TrackControl colorDurationControl;
        private System.Windows.Forms.Label label1;
        private Assets.Components.StackPanel scalePanel;
        private Components.AnimationFloatControl scaleControl;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label5;
        private Components.AnimationLoopControl rotationLoopControl;
        private Assets.Components.TrackControl rotationDurationControl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label7;
        private Components.AnimationLoopControl scaleLoopControl;
        private Assets.Components.TrackControl scaleDurationControl;
        private System.Windows.Forms.Label label8;
        private Assets.Components.TrackControl repeatScaleControl;
        private System.Windows.Forms.CheckBox emissionCheckBox;
        private Assets.Components.TrackControl distanceControl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel emissionPanel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel repeatScalePanel;
        private System.Windows.Forms.Label label9;
        private Assets.Components.TrackControl emissionSmoothnessControl;
        private Assets.Components.StackPanel objectPanel;
        private Scenes.SceneObjectControl objectControl;
    }
}

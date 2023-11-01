﻿namespace CDK.Assets.Components
{
    partial class RectangleTrackControl
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.xControl = new CDK.Assets.Components.TrackControl();
            this.yControl = new CDK.Assets.Components.TrackControl();
            this.widthControl = new CDK.Assets.Components.TrackControl();
            this.heightControl = new CDK.Assets.Components.TrackControl();
            this.SuspendLayout();
            // 
            // xControl
            // 
            this.xControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xControl.DecimalPlaces = 0;
            this.xControl.Increment = 1F;
            this.xControl.Location = new System.Drawing.Point(0, 0);
            this.xControl.Maximum = 1000F;
            this.xControl.Minimum = -1000F;
            this.xControl.Name = "xControl";
            this.xControl.ScaleType = TrackBarScaleType.None;
            this.xControl.Size = new System.Drawing.Size(240, 21);
            this.xControl.TabIndex = 0;
            this.xControl.Value = 0F;
            this.xControl.ValueChanged += new System.EventHandler(this.Component_ValueChanged);
            // 
            // yControl
            // 
            this.yControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.yControl.DecimalPlaces = 0;
            this.yControl.Increment = 1F;
            this.yControl.Location = new System.Drawing.Point(0, 27);
            this.yControl.Maximum = 1000F;
            this.yControl.Minimum = -1000F;
            this.yControl.Name = "yControl";
            this.yControl.ScaleType = TrackBarScaleType.None;
            this.yControl.Size = new System.Drawing.Size(240, 21);
            this.yControl.TabIndex = 1;
            this.yControl.Value = 0F;
            this.yControl.ValueChanged += new System.EventHandler(this.Component_ValueChanged);
            // 
            // widthControl
            // 
            this.widthControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.widthControl.DecimalPlaces = 0;
            this.widthControl.Increment = 1F;
            this.widthControl.Location = new System.Drawing.Point(0, 54);
            this.widthControl.Maximum = 2000F;
            this.widthControl.Minimum = 0F;
            this.widthControl.Name = "widthControl";
            this.widthControl.ScaleType = TrackBarScaleType.None;
            this.widthControl.Size = new System.Drawing.Size(240, 21);
            this.widthControl.TabIndex = 2;
            this.widthControl.Value = 0F;
            this.widthControl.ValueChanged += new System.EventHandler(this.Component_ValueChanged);
            // 
            // heightControl
            // 
            this.heightControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.heightControl.DecimalPlaces = 0;
            this.heightControl.Increment = 1F;
            this.heightControl.Location = new System.Drawing.Point(0, 81);
            this.heightControl.Maximum = 2000F;
            this.heightControl.Minimum = 0F;
            this.heightControl.Name = "heightControl";
            this.heightControl.ScaleType = TrackBarScaleType.None;
            this.heightControl.Size = new System.Drawing.Size(240, 21);
            this.heightControl.TabIndex = 3;
            this.heightControl.Value = 0F;
            this.heightControl.ValueChanged += new System.EventHandler(this.Component_ValueChanged);
            // 
            // RectangleTrackControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.heightControl);
            this.Controls.Add(this.widthControl);
            this.Controls.Add(this.yControl);
            this.Controls.Add(this.xControl);
            this.Name = "RectangleTrackControl";
            this.Size = new System.Drawing.Size(240, 102);
            this.ResumeLayout(false);

        }

        #endregion

        private TrackControl xControl;
        private TrackControl yControl;
        private TrackControl widthControl;
        private TrackControl heightControl;
    }
}
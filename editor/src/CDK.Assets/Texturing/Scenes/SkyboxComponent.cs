using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Assets.Scenes;

namespace CDK.Assets.Texturing
{
    public class SkyboxComponent : SceneComponent
    {
        public SkyboxAsset Asset { private set; get; }

        public override string Name
        {
            set { }
            get => Asset.TagName;
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "TagName":
                    OnPropertyChanged("Name");
                    break;
            }
        }

        private void Asset_Refresh(object sender, EventArgs e) => OnRefresh();

        public SkyboxComponent(SkyboxAsset asset)
        {
            Asset = asset;

            Asset.AddWeakPropertyChanged(Asset_PropertyChanged);
            Asset.AddWeakRefresh(Asset_Refresh);
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            retains.Add(Asset.Key);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            if (Asset == element)
            {
                from = this;
                return true;
            }
            from = null;
            return false;
        }

        public override SceneComponentType Type => SceneComponentType.Skybox;
        public override SceneComponent Clone(bool binding) => null;
        public override SceneComponentType[] SubTypes => new SceneComponentType[0];
        public override bool AddSubEnabled(SceneComponent obj) => false;
        public override void AddSub(SceneComponentType type) { }
        protected override void SaveContent(XmlWriter writer) => throw new InvalidOperationException();
        protected override void LoadContent(XmlNode node) => throw new InvalidOperationException();
    }
}

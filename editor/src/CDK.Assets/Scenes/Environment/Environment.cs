using System;
using System.Linq;
using System.Xml;
using System.IO;

using CDK.Assets.Configs;

namespace CDK.Assets.Scenes
{
    public class Environment : SceneComponent
    {
        public override string Name
        {
            set { }
            get => "Environment";
        }

        private PreferenceConfig _PreferenceConfig;
        [Binding]
        public PreferenceConfig PreferenceConfig
        {
            set
            {
                if (value == null) throw new InvalidOperationException();

                var prev = _PreferenceConfig;
                if (SetProperty(ref _PreferenceConfig, value))
                {
                    prev.RemoveWeakRefresh(Config_Refresh);
                    _PreferenceConfig.AddWeakRefresh(Config_Refresh);
                }
            }
            get => _PreferenceConfig;
        }

        private EnvironmentConfig _EnvironmentConfig;
        [Binding]
        public EnvironmentConfig EnvironmentConfig
        {
            set
            {
                if (value == null) throw new InvalidOperationException();

                var prev = _EnvironmentConfig;
                if (SetProperty(ref _EnvironmentConfig, value))
                {
                    prev.RemoveWeakRefresh(Config_Refresh);
                    _EnvironmentConfig.AddWeakRefresh(Config_Refresh);

                    ResetPropBinding();
                }
            }
            get => _EnvironmentConfig;
        }

        public Environment(PreferenceConfig pref, EnvironmentConfig light)
        {
            _PreferenceConfig = pref;
            _PreferenceConfig.AddWeakRefresh(Config_Refresh);
            _EnvironmentConfig = light;
            _EnvironmentConfig.AddWeakRefresh(Config_Refresh);

            ResetPropBinding();
        }

        public Environment(Environment other, bool binding) : base(other, binding, false)
        {
            _PreferenceConfig = other.PreferenceConfig;
            _PreferenceConfig.AddWeakRefresh(Config_Refresh);
            _EnvironmentConfig = other.EnvironmentConfig;
            _EnvironmentConfig.AddWeakRefresh(Config_Refresh);

            ResetPropBinding();
        }

        private void Config_Refresh(object sender, EventArgs e) => OnRefresh();

        private void ResetPropBinding()
        {
            using (new AssetCommandHolder())
            {
                Children.Bind(_EnvironmentConfig.Props, true, true);
            }
        }

        public override SceneComponentType Type => SceneComponentType.Environment;
        public override SceneComponent Clone(bool binding) => new Environment(this, binding);
        public override SceneComponentType[] SubTypes
        {
            get
            {
                return new SceneComponentType[] {
                    SceneComponentType.DirectionalLight,
                    SceneComponentType.PointLight,
                    SceneComponentType.SpotLight
                };
            }
        }
        protected override void SaveContent(XmlWriter writer)
        {
            writer.WriteAttribute("preferenceConfig", _PreferenceConfig.Key);
            writer.WriteAttribute("environmentConfig", _EnvironmentConfig.Key);
        }

        protected override void LoadContent(XmlNode node)
        {
            var config = Scene.Config;

            var key = node.ReadAttributeString("preferenceConfig");
            PreferenceConfig = config.Preferences.FirstOrDefault(e => e.Key == key) ?? config.Preferences[0];

            key = node.ReadAttributeString("environmentConfig");
            EnvironmentConfig = config.Environments.FirstOrDefault(e => e.Key == key) ?? config.Environments[0];
        }
    }
}

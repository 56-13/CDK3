using System;
using System.Numerics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.IO;

using CDK.Drawing;
using CDK.Drawing.Meshing;

namespace CDK.Assets.Meshing
{
    public class MeshGeometry : AssetElement
    {
        public MeshAsset Parent { private set; get; }
        public override AssetElement GetParent() => Parent;
        public override string GetLocation() => $"{Parent.GetLocation()}.{Name}";
        public Geometry Origin { internal set; get; }
        public string Name => Origin.Name;
        public AssetElementList<MaterialConfig> MaterialConfigs { private set; get; }
        public AssetElementList<MeshCollider> Colliders { private set; get; }

        public MeshGeometry(MeshAsset parent, Geometry geometry)
        {
            Parent = parent;

            Origin = geometry;

            MaterialConfigs = new AssetElementList<MaterialConfig>(this);
            using (new AssetCommandHolder())
            {
                foreach (var materialConfig in Origin.MaterialConfigs)
                {
                    MaterialConfigs.Add(new MaterialConfig(this, materialConfig.Name));
                }
            }
            MaterialConfigs.BeforeListChanged += MaterialConfigs_BeforeListChanged;

            Colliders = new AssetElementList<MeshCollider>(this);
            Colliders.ListChanged += Colliders_ListChanged;
        }

        public MeshGeometry(MeshAsset parent, MeshGeometry other)
        {
            AssetManager.Instance.AddRedirection(other, this);

            Parent = parent;

            Origin = other.Origin;

            MaterialConfigs = new AssetElementList<MaterialConfig>(this);
            using (new AssetCommandHolder())
            {
                foreach (var materialConfig in other.MaterialConfigs)
                {
                    MaterialConfigs.Add(new MaterialConfig(this, materialConfig.Name, materialConfig));
                }
            }
            MaterialConfigs.BeforeListChanged += MaterialConfigs_BeforeListChanged;

            Colliders = new AssetElementList<MeshCollider>(this);

            using (new AssetCommandHolder())
            {
                foreach (var collider in other.Colliders)
                {
                    Colliders.Add(new MeshCollider(this, collider));
                }
            }

            Colliders.ListChanged += Colliders_ListChanged;
        }

        private void MaterialConfigs_BeforeListChanged(object sender, BeforeListChangedEventArgs<MaterialConfig> e)
        {
            e.Cancel = true;
        }

        private void Colliders_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor != null) break;
                    goto case ListChangedType.ItemAdded;
                case ListChangedType.ItemAdded:
                    if (Colliders[e.NewIndex].Parent != this) throw new InvalidOperationException();
                    break;
                case ListChangedType.Reset:
                    foreach (var collider in Colliders)
                    {
                        if (collider.Parent != this) throw new InvalidOperationException();
                    }
                    break;
            }
        }

        internal override void AddRetains(ICollection<string> retains)
        {
            foreach (var materialConfig in MaterialConfigs) materialConfig.AddRetains(retains);
        }

        internal override bool IsRetaining(AssetElement element, out AssetElement from)
        {
            foreach (var materialConfig in MaterialConfigs)
            {
                if (materialConfig.IsRetaining(element, out from)) return true;
            }
            from = null;
            return false;
        }

        public void GetTransformNames(ICollection<string> names) => Origin.GetBoneNames(names);
        public string[] GetTransformNames() => Origin.GetBoneNames();

        internal void AddCollider(Instance instance, in Matrix4x4 instanceTransform, ref Collider result)
        {
            if (Colliders.Count != 0)
            {
                if (result == null) result = new Collider(Colliders.Count);
                foreach (var collider in Colliders) collider.AddCollider(instance, instanceTransform, result);
            }
        }

        internal void Save(XmlWriter writer)
        {
            writer.WriteStartElement("geometry");
            writer.WriteAttribute("name", Origin.Name);

            writer.WriteStartElement("materials");
            foreach (var materialConfig in MaterialConfigs) materialConfig.Save(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("colliders");
            foreach (var collider in Colliders) collider.Save(writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }


        internal void Load(XmlNode node, Geometry geometry)
        {
            Origin = geometry;

            Colliders.Clear();

            foreach (XmlNode subnode in node.ChildNodes)
            {
                switch (subnode.LocalName)
                {
                    case "materials":
                        for (var i = 0; i < Origin.MaterialConfigs.Length; i++)
                        {
                            MaterialConfigs[i].Load(subnode.ChildNodes[i]);
                        }
                        break;
                    case "colliders":
                        foreach (XmlNode colliderNode in subnode.ChildNodes)
                        {
                            Colliders.Add(new MeshCollider(this, colliderNode));
                        }
                        break;
                }
            }
        }
    }
}

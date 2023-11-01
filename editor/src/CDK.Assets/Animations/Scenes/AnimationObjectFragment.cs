using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using CDK.Drawing;

using CDK.Assets.Scenes;

namespace CDK.Assets.Animations
{
    public class AnimationObjectFragment : SceneComponent
    {
        private AnimationObjectBase _Root;
        public AnimationObjectBase Root
        {
            set
            {
                if (_Root != value)
                {
                    _Root = value;
                    OnPropertyChanged("Root");

                    foreach (AnimationObjectFragment child in Children) child.Root = value;
                }
            }
            get => _Root;
        }

        public AnimationFragment Origin { private set; get; }

        public override string Name 
        { 
            set => Origin.Name = value;
            get => Origin.Name ?? "Animation";
        }

        private bool _SubstanceView;
        public bool SubstanceView
        {
            set
            {
                if (SetProperty(ref _SubstanceView, value, false))
                {
                    if (_focus) Substance?.Select(_SubstanceView);
                }
            }
            get => _SubstanceView;
        }

        public SceneObject Substance { private set; get; }
        public AnimationObjectDerivation Derivation { private set; get; }
        internal Matrix4x4 PostTransform { set; get; }
        public UpdateState State { private set; get; }
        public float Progress { private set; get; }

        private int _xRandom;
        private int _yRandom;
        private int _zRandom;
        private int _radialRandom;
        private int _tangentialRandom;
        private int _tangentialAngleRandom;
        private int _rotationRandom;
        private int _scaleRandom;
        private int _soundHandle;
        private int _soundCounter;
        private bool _visible;
        private bool _bindingChanging;
        private bool _focus;

        private const float FacingInterval = 0.016f;

        public AnimationObjectFragment(AnimationFragment origin)
        {
            Origin = origin;

            Init();
        }

        public AnimationObjectFragment(AnimationObjectFragment other)
        {
            AssetManager.Instance.InvokeRedirection(() =>
            {
                Origin = AssetManager.Instance.GetRedirection(other.Origin);
                if (Origin != other.Origin) Origin = new AnimationFragment(other.Origin);
                //복사가 Animation을 복사해서 카피하는 것인지 애니메이션과 함께 리디렉션으로 카피하는 것인지 체크

                Init();
            });
        }

        private void Init()
        {
            foreach (var child in Origin.Children) Children.Add(new AnimationObjectFragment(child));

            if (Origin.Children.Count != 0) Derivation = Origin.Derivation.CreateObject(this);
            if (Origin.Substance != null) Substance = Origin.Substance.CreateObject(this);

            PropertyChanged += AnimationObjectFragment_PropertyChanged;
            Children.ListChanged += Children_ListChanged;
            Origin.AddWeakPropertyChanged(Origin_PropertyChanged);
            Origin.Children.AddWeakListChanged(Origin_Children_ListChanged);

            PostTransform = Matrix4x4.Identity;
            State = UpdateState.None;
            Progress = -Origin.Latency;
            _visible = !Origin.Closing && Origin.Latency == 0 && Origin.GetLocaleVisible();     //부모가 없어서 keyVisible 은 아직 알 수 없다
            ResetRandoms();
        }

        private void AnimationObjectFragment_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Parent") Root = GetAncestor<AnimationObjectBase>(false);
        }

        private void Children_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (_bindingChanging) return;

            _bindingChanging = true;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    Origin.Children.Insert(e.NewIndex, ((AnimationObjectFragment)Children[e.NewIndex]).Origin);

                    if (Derivation == null) Derivation = Origin.Derivation.CreateObject(this);
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        Origin.Children[e.NewIndex] = ((AnimationObjectFragment)Children[e.NewIndex]).Origin;
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    Origin.Children.RemoveAt(e.NewIndex);

                    if (Origin.Children.Count == 0) Derivation = null;
                    break;
                case ListChangedType.ItemMoved:
                    Origin.Children.Move(e.OldIndex, e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    Origin.Children.Clear();
                    foreach (AnimationObjectFragment obj in Children)
                    {
                        Origin.Children.Add(obj.Origin);
                    }
                    if (Derivation == null)
                    {
                        if (Origin.Children.Count != 0) Derivation = Origin.Derivation.CreateObject(this);
                    }
                    else
                    {
                        if (Origin.Children.Count == 0) Derivation = null;
                    }
                    break;
            }

            _bindingChanging = false;
        }

        private void Origin_Children_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (_bindingChanging) return;

            _bindingChanging = true;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    Children.Insert(e.NewIndex, new AnimationObjectFragment(Origin.Children[e.NewIndex]));

                    if (Derivation == null) Derivation = Origin.Derivation.CreateObject(this);
                    break;
                case ListChangedType.ItemChanged:
                    if (e.PropertyDescriptor == null)
                    {
                        Children[e.NewIndex] = new AnimationObjectFragment(Origin.Children[e.NewIndex]);
                    }
                    break;
                case ListChangedType.ItemMoved:
                    Children.Move(e.OldIndex, e.NewIndex);
                    break;
                case ListChangedType.ItemDeleted:
                    Children.RemoveAt(e.NewIndex);

                    if (Origin.Children.Count == 0) Derivation = null;
                    break;
                case ListChangedType.Reset:
                    Children.Clear();
                    foreach (var child in Origin.Children)
                    {
                        Children.Add(new AnimationObjectFragment(child));
                    }

                    if (Derivation == null)
                    {
                        if (Origin.Children.Count != 0) Derivation = Origin.Derivation.CreateObject(this);
                    }
                    else
                    {
                        if (Origin.Children.Count == 0) Derivation = null;
                    }
                    break;
            }

            _bindingChanging = false;
        }

        private void Origin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Name":
                    OnPropertyChanged("Name");
                    break;
                case "Derivation":
                    if (Origin.Children.Count != 0) Derivation = Origin.Derivation.CreateObject(this);
                    break;
                case "Substance":
                    Substance = Origin.Substance?.CreateObject(this);
                    if (_SubstanceView && _focus) Substance?.Select(true);
                    break;
                case "Parent":
                    if (_soundHandle != 0)
                    {
                        AudioPlayer.Instance.Stop(_soundHandle);
                        _soundHandle = 0;
                    }
                    break;
            }
        }

        public override SceneComponentType Type => SceneComponentType.AnimationFragment;
        public override SceneComponent Clone(bool binding) => new AnimationObjectFragment(this);
        public override SceneComponentType[] SubTypes => new SceneComponentType[] { SceneComponentType.AnimationFragment };
        public override bool AddSubEnabled(SceneComponent obj) => obj.Type == SceneComponentType.AnimationFragment;
        public override void AddSub(SceneComponentType type)
        {
            if (type == SceneComponentType.AnimationFragment) Origin.Children.Add(new AnimationFragment());
        }

        internal bool AddAABB(ref ABoundingBox aabb)
        {
            bool flag = false;
            if (_visible)
            {
                if (Substance != null) flag |= Substance.AddAABB(ref aabb);
                if (Derivation != null) flag |= Derivation.AddAABB(ref aabb);
            }
            return flag;
        }


        internal void AddCollider(ref Collider result) 
        {
            if (_visible) {
                Substance?.AddCollider(ref result);
                Derivation?.AddCollider(ref result);
            }
        }

        internal bool CameraCapture(ref Camera camera)
        {
            if (Substance != null && Substance.CameraCapture(ref camera)) return true;

            foreach (AnimationObjectFragment child in Children)
            {
                if (child.CameraCapture(ref camera)) return true;
            }
            return false;
        }

        internal bool CameraFilter(Graphics graphics)
        {
            if (Substance != null && Substance.CameraFilter(graphics)) return true;

            foreach (AnimationObjectFragment child in Children)
            {
                if (child.CameraFilter(graphics)) return true;
            }
            return false;
        }

        internal bool AfterCameraUpdate()
        {
            if (Origin.Billboard) return true;

            if (Substance != null && Substance.AfterCameraUpdate()) return true;

            foreach (AnimationObjectFragment child in Children)
            {
                if (child.AfterCameraUpdate()) return true;
            }
            return false;
        }

        private static Vector3 GetTangentialDirection(in Vector3 dir, float angle)
        {
            var rtn = new Vector3(dir.Y, -dir.X, 0);
            rtn = Vector3.Normalize(Vector3.Cross(rtn, dir));
            Quaternion rotation = Quaternion.CreateFromAxisAngle(dir, angle);
            rtn = Vector3.Transform(rtn, rotation);
            return rtn;
        }

        internal unsafe bool GetTransform(float progress, string name, out Matrix4x4 result)
        {
            if (name != null)
            {
                if (Origin.Pivot)
                {
                    if (Substance != null && Substance.GetTransform(progress + Substance.Progress - Progress, name, out result)) return true;
                    if (Origin.Name == name) goto local;
                }
                if (Derivation != null) return Derivation.GetTransform(progress, name, out result);
                result = Matrix4x4.Identity;
                return false;
            }

        local:

            if (Parent == _Root)
            {
                if (!_Root.GetTransform(progress, null, out result)) return false;
            }
            else if (Parent is AnimationObjectFragment parent)
            {
                var parentProgress = progress + parent.Progress - Progress;
                if (Origin.Binding != null && parent.Substance != null)
                {
                    if (!parent.Substance.GetTransform(parentProgress, Origin.Binding, out result)) return false;
                }
                else if (!parent.GetTransform(parentProgress, null, out result)) return false;
            }
            else
            {
                result = Matrix4x4.Identity;
                return false;
            }

            if (progress < 0) progress = 0;

            result = PostTransform * result;

            Quaternion rotation;
            Vector3 scale;

            if (Origin.RotationDuration != 0)
            {
                var rotationProgress = Origin.RotationLoop.GetProgress(progress / Origin.RotationDuration, out var rotation0RandomSeq, out var rotation1RandomSeq);

                var r = RandomUtil.ToFloatSequenced(_rotationRandom, rotation0RandomSeq, rotation1RandomSeq, rotationProgress);

                rotation = Quaternion.CreateFromYawPitchRoll(
                    Origin.RotationY.GetValue(rotationProgress, r) * MathUtil.ToRadians,
                    Origin.RotationX.GetValue(rotationProgress, r) * MathUtil.ToRadians,
                    Origin.RotationZ.GetValue(rotationProgress, r) * MathUtil.ToRadians);
            }
            else
            {
                var r = RandomUtil.ToFloat(_rotationRandom);

                rotation = Quaternion.CreateFromYawPitchRoll(
                    Origin.RotationY.GetValue(1, r) * MathUtil.ToRadians,
                    Origin.RotationX.GetValue(1, r) * MathUtil.ToRadians,
                    Origin.RotationZ.GetValue(1, r) * MathUtil.ToRadians);
            }

            if (Origin.ScaleDuration != 0)
            {
                var scaleProgress = Origin.ScaleLoop.GetProgress(progress / Origin.ScaleDuration, out var scale0RandomSeq, out var scale1RandomSeq);

                var r = RandomUtil.ToFloatSequenced(_scaleRandom, scale0RandomSeq, scale1RandomSeq, scaleProgress);

                var x = Origin.ScaleX.GetValue(scaleProgress, r);
                var y = Origin.ScaleEach ? Origin.ScaleY.GetValue(scaleProgress, r) : x;
                var z = Origin.ScaleEach ? Origin.ScaleZ.GetValue(scaleProgress, r) : x;
                scale = new Vector3(x, y, z);
            }
            else
            {
                var r = RandomUtil.ToFloat(_scaleRandom);

                var x = Origin.ScaleX.GetValue(1, r);
                var y = Origin.ScaleEach ? Origin.ScaleY.GetValue(1, r) : x;
                var z = Origin.ScaleEach ? Origin.ScaleZ.GetValue(1, r) : x;
                scale = new Vector3(x, y, z);
            }

            if (scale.X <= 0 || scale.Y <= 0 || scale.Z <= 0) return false;

            var transforms = stackalloc Matrix4x4[2];
            transforms[0] = result;
            if (Origin.Target == AnimationTarget.Origin) transforms[1] = result;
            else if (!_Root.GetTargetTransform(Origin.Target, progress + _Root.Progress - Progress, out transforms[1])) return false;

            var points = stackalloc Vector3[2];
            
            for (var i = 0; i < 2; i++)
            {
                points[i] = new Vector3(Origin.X.GetValue(i), Origin.Y.GetValue(i), Origin.Z.GetValue(i));

                if (points[i] == Vector3.Zero) points[i] = transforms[i].Translation;
                else if (Origin.Billboard)
                {
                    points[i] = Scene.Camera.Right * points[i].X - Scene.Camera.Up * points[i].Y + Scene.Camera.Forward * points[i].Z;
                    points[i] += transforms[i].Translation;
                }
                else points[i] = Vector3.Transform(points[i], transforms[i]);
            }

            var normalFlag = !VectorUtil.NearEqual(points[0], points[1]);
            var normal = normalFlag ? Vector3.Normalize(points[1] - points[0]) : Vector3.Zero;

            if (normalFlag)
            {
                for (var i = 0; i < 2; i++)
                {
                    var radial = Origin.Radial.GetValue(i);

                    if (radial != 0) points[i] += normal * radial;
                }
            }
            var pathDuration = Origin.UsingPathSpeed ? (Origin.PathSpeed != 0 ? Vector3.Distance(points[0], points[1]) / Origin.PathSpeed : 0) : Origin.PathDuration;

            var pos0RandomSeq = 0;
            var pos1RandomSeq = 0;
            var pathProgress = pathDuration != 0 ? Origin.PathLoop.GetProgress(progress / pathDuration, out pos0RandomSeq, out pos1RandomSeq) : 1;

            if (Origin.Reverse) pathProgress = 1 - pathProgress;

            var offset = new Vector3(
                Origin.X.GetValue(pathProgress, RandomUtil.ToFloatSequenced(_xRandom, pos0RandomSeq, pos1RandomSeq, pathProgress)),
                Origin.Y.GetValue(pathProgress, RandomUtil.ToFloatSequenced(_yRandom, pos0RandomSeq, pos1RandomSeq, pathProgress)),
                Origin.Z.GetValue(pathProgress, RandomUtil.ToFloatSequenced(_zRandom, pos0RandomSeq, pos1RandomSeq, pathProgress)));

            if (Origin.Billboard)
            {
                points[0] = transforms[0].Translation;
                points[1] = transforms[1].Translation;

                offset = Scene.Camera.Right * offset.X - Scene.Camera.Up * offset.Y + Scene.Camera.Forward * offset.Z;
                points[0] += offset;
                points[1] += offset;
            }
            else
            {
                points[0] = Vector3.Transform(offset, transforms[0]);
                points[1] = Vector3.Transform(offset, transforms[1]);
            }

            var translation = Vector3.Lerp(points[0], points[1], pathProgress);

            if (Origin.Facing)
            {
                if (progress > FacingInterval && GetTransform(progress - FacingInterval, null, out var prevTransform))
                {
                    var start = prevTransform.Translation;
                    var end = result.Translation;

                    if (!VectorUtil.NearEqual(start, end))
                    {
                        var forward = result.Forward();         //TODO:체크필요
                        var dir = end - start;
                        var w = Vector3.Cross(forward, dir);
                        var facingRotation = new Quaternion(w.X, w.Y, w.Z, Vector3.Dot(forward, dir));
                        facingRotation.W += facingRotation.Length();
                        facingRotation = Quaternion.Normalize(facingRotation);

                        rotation = facingRotation * rotation;
                    }
                    else return false;
                }
                else return false;
            }

            if (normalFlag)
            {
                var radial = Origin.Radial.GetValue(pathProgress, RandomUtil.ToFloatSequenced(_radialRandom, pos0RandomSeq, pos1RandomSeq, pathProgress));

                if (radial != 0) translation += normal * radial;

                var tangential = Origin.Tangential.GetValue(pathProgress, RandomUtil.ToFloatSequenced(_tangentialRandom, pos0RandomSeq, pos1RandomSeq, pathProgress));

                if (tangential != 0)
                {
                    var tangentialAngle = Origin.TangentialAngle.GetValue(pathProgress, RandomUtil.ToFloatSequenced(_tangentialAngleRandom, pos0RandomSeq)) * MathUtil.ToRadians;
                    translation += GetTangentialDirection(normal, tangentialAngle) * tangential;
                }
            }

            result = Matrix4x4.Lerp(transforms[0], transforms[1], pathProgress);
            result.M41 = translation.X;
            result.M42 = translation.Y;
            result.M43 = translation.Z;

            if (!rotation.IsIdentity)
            {
                result = Matrix4x4.CreateFromQuaternion(rotation) * result;
            }
            if (scale.X != 1)
            {
                result.M11 *= scale.X;
                result.M12 *= scale.X;
                result.M13 *= scale.X;
            }
            if (scale.Y != 1)
            {
                result.M21 *= scale.Y;
                result.M22 *= scale.Y;
                result.M23 *= scale.Y;
            }
            if (scale.Z != 1)
            {
                result.M31 *= scale.Z;
                result.M32 *= scale.Z;
                result.M33 *= scale.Z;
            }

            return true;
        }

        internal unsafe float GetPathDuration()
        {
            if (!Origin.UsingPathSpeed) return Origin.PathDuration;

            if (Origin.PathSpeed == 0) return 0;

            Matrix4x4 transform;
            if (Parent == _Root)
            {
                if (!_Root.GetTransform(out transform)) return 0;
            }
            else if (Parent is AnimationObjectFragment parent)
            {
                if (Origin.Binding != null && parent.Substance != null)
                {
                    if (!parent.Substance.GetTransform(Origin.Binding, out transform)) return 0;
                }
                else if (!parent.GetTransform(parent.Progress, null, out transform)) return 0;
            }
            else return 0;

            var transforms = stackalloc Matrix4x4[2];
            transforms[0] = transform;
            if (Origin.Target == AnimationTarget.Origin) transforms[1] = transform;
            else if (!_Root.GetTargetTransform(Origin.Target, out transforms[1])) return 0;

            var points = stackalloc Vector3[2];

            for (var i = 0; i < 2; i++)
            {
                points[i] = new Vector3(Origin.X.GetValue(i), Origin.Y.GetValue(i), Origin.Z.GetValue(i));
                if (points[i] != Vector3.Zero) points[i] = transforms[i].Translation;
                else if (Origin.Billboard)
                {
                    points[i] = Scene.Camera.Right * points[i].X - Scene.Camera.Up * points[i].Y + Scene.Camera.Forward * points[i].Z;
                    points[i] += transforms[i].Translation;
                }
                else points[i] = Vector3.Transform(points[i], transforms[i]);
            }

            if (!VectorUtil.NearEqual(points[0], points[1]))
            {
                var normal = Vector3.Normalize(points[1] - points[0]);

                for (var i = 0; i < 2; i++)
                {
                    var radial = Origin.Radial.GetValue(i);

                    if (radial != 0) points[i] += normal * radial;
                }
            }
            return Vector3.Distance(points[0], points[1]) / Origin.PathSpeed;
        }

        public float GetDuration(DurationParam param)
        {
            var duration = Origin.Duration;

            var pathDuration = GetPathDuration();

            if (pathDuration != 0)
            {
                pathDuration *= Origin.PathLoop.Count;

                if (duration < pathDuration) duration = pathDuration;
            }

            if (Origin.RotationDuration != 0)
            {
                var rotationDuration = Origin.RotationDuration * Origin.RotationLoop.Count;

                if (duration < rotationDuration) duration = rotationDuration;
            }

            if (Origin.ScaleDuration != 0)
            {
                var scaleDuration = Origin.ScaleDuration * Origin.ScaleLoop.Count;

                if (duration < scaleDuration) duration = scaleDuration;
            }

            if (Substance != null)
            {
                var substanceDuration = Substance.GetDuration(param, duration);

                if (duration < substanceDuration) duration = substanceDuration;
            }

            if (Derivation != null)
            {
                var derivationDuration = Derivation.GetDuration(param, duration);

                if (duration < derivationDuration) duration = derivationDuration;
            }

            if (duration != 0) duration += Origin.Latency;

            return duration;
        }

        private void ResetRandoms()
        {
            _xRandom = RandomUtil.Next();
            _yRandom = RandomUtil.Next();
            _zRandom = RandomUtil.Next();
            _radialRandom = RandomUtil.Next();
            _tangentialRandom = RandomUtil.Next();
            _tangentialAngleRandom = RandomUtil.Next();
            _rotationRandom = RandomUtil.Next();
            _scaleRandom = RandomUtil.Next();
        }

        private void StopSound()
        {
            if (_soundHandle != 0 && (Origin.SoundSource == null || Origin.SoundLoop == 0 || Origin.SoundStop))
            {
                AudioPlayer.Instance.Stop(_soundHandle);
            }
            _soundCounter = 0;
        }

        internal void Rewind()
        {
            ResetRandoms();

            StopSound();

            State = UpdateState.None;
            
            Progress = -Origin.Latency;

            _visible = !Origin.Closing && Origin.Latency == 0 && Origin.GetLocaleVisible() && (Origin.Keys == null || _Root.Keys == null || Origin.Keys.All(k => Array.IndexOf(_Root.Keys, k) >= 0));

            Derivation?.Rewind();
            Substance?.Rewind();
        }

        private void UpdateVisible(bool visible, ref UpdateFlags outtrans)
        {
            if (_visible != visible)
            {
                _visible = visible;
                if ((outtrans & UpdateFlags.Transform) == 0 && Origin.HasPivot) outtrans |= UpdateFlags.Transform;
                if ((outtrans & UpdateFlags.AABB) == 0 && Origin.HasSubstance) outtrans |= UpdateFlags.AABB;
            }
        }

        internal UpdateState Update(LightSpace lightSpace, float delta, bool alive, UpdateFlags inflags, ref UpdateFlags outflags)
        {
            if (Origin.Closing)
            {
                if (alive)
                {
                    UpdateVisible(false, ref outflags);
                    State = UpdateState.None;
                    return UpdateState.Alive;
                }
                else alive = true;
            }

            Progress += delta;

            if (Progress < 0)
            {
                UpdateVisible(false, ref outflags);
                State = UpdateState.None;
                return UpdateState.Alive;
            }

            if (Origin.Duration != 0 && Progress >= Origin.Duration) alive = false;

            var remaining = false;

            if (Origin.Billboard && (inflags & UpdateFlags.View) != 0) inflags |= UpdateFlags.Transform;

            if ((Origin.UsingPathSpeed ? Origin.PathSpeed : Origin.PathDuration) != 0)
            {
                inflags |= UpdateFlags.Transform;

                if (Origin.PathLoop.Count != 0)
                {
                    var pathDuration = GetPathDuration();

                    if (pathDuration != 0)
                    {
                        if (Progress < pathDuration * Origin.PathLoop.Count) remaining = true;
                        else if (Origin.PathLoop.Finish) alive = false;
                    }
                }
            }

            if (Origin.RotationDuration != 0)
            {
                inflags |= UpdateFlags.Transform;

                if (Origin.RotationLoop.Count != 0)
                {
                    if (Progress < Origin.RotationDuration * Origin.RotationLoop.Count) remaining = true;
                    else if (Origin.RotationLoop.Finish) alive = false;
                }
            }

            if (Origin.ScaleDuration != 0)
            {
                inflags |= UpdateFlags.Transform;

                if (Origin.ScaleLoop.Count != 0)
                {
                    if (Progress < Origin.ScaleDuration * Origin.ScaleLoop.Count) remaining = true;
                    else if (Origin.ScaleLoop.Finish) alive = false;
                }
            }

            if (Progress < delta) delta = Progress;

            if (Origin.Pivot && (inflags & UpdateFlags.Transform) != 0) outflags |= UpdateFlags.Transform;

            if (Substance != null)
            {
                var outflags_ = inflags;
                switch (Substance.Update(lightSpace, delta, alive || remaining, ref outflags_))
                {
                    case UpdateState.Stopped:
                        alive = false;
                        break;
                    case UpdateState.Alive:
                        remaining = true;
                        break;
                    case UpdateState.Finishing:
                        alive = false;
                        remaining = true;
                        break;
                }
                if (!Origin.Pivot) outflags_ &= UpdateFlags.Transform;
                outflags |= outflags_;
            }

            if (Derivation != null)
            {
                switch (Derivation.Update(lightSpace, delta, alive || remaining, inflags, ref outflags))
                {
                    case UpdateState.Stopped:
                        if (Origin.DerivationFinish) alive = false;
                        break;
                    case UpdateState.Alive:
                        remaining = true;
                        break;
                    case UpdateState.Finishing:
                        if (Origin.DerivationFinish) alive = false;
                        remaining = true;
                        break;
                }
            }

            UpdateVisible((alive || remaining) && Origin.GetLocaleVisible() && (Origin.Keys == null || _Root.Keys == null || Origin.Keys.All(k => Array.IndexOf(_Root.Keys, k) >= 0)), ref outflags);       //TODO

            if (_visible && Origin.SoundSource != null && Progress >= Origin.SoundLatency)
            {
                var path = Origin.SoundSource.DataPath;

                if (path != null)
                {
                    var counter = Origin.SoundDuration != 0 ? (int)((Progress - Origin.SoundLatency) / Origin.SoundDuration) + 1 : 1;

                    if (_soundCounter != counter)
                    {
                        _soundCounter = counter;

                        var loop = Origin.SoundDuration != 0 ? 1 : Origin.SoundLoop;

                        if (Origin.SoundPerspective)
                        {
                            if (GetTransform(Progress, null, out var transform))
                            {
                                var pos = transform.Translation;
                                _soundHandle = Scene.PlaySound(path, Origin.SoundVolume, Origin.SoundControl, loop, Origin.SoundPriority, Origin.SoundDuplication, pos);
                            }
                        }
                        else _soundHandle = Scene.PlaySound(path, Origin.SoundVolume, Origin.SoundControl, loop, Origin.SoundPriority, Origin.SoundDuplication, null);
                    }
                }
            }

            if (alive) State = UpdateState.Alive;
            else if (remaining) State = UpdateState.Finishing;
            else
            {
                StopSound();
                State = UpdateState.Stopped;
            }
            return State;
        }

        internal void LogState(StringBuilder strbuf)
        {
            var p = Parent;
            while (p is AnimationObjectFragment parent)
            {
                strbuf.Append(' ');
                p = parent.Parent;
            }
            strbuf.Append(Origin.Name);
            strbuf.Append(':');
            strbuf.Append(State);
            if (Substance != null)
            {
                strbuf.Append(':');
                strbuf.Append(Substance.Type);

                var status = Substance.GetStatus();
                if (status != null)
                {
                    strbuf.Append(':');
                    strbuf.Append(status);
                }
            }
            strbuf.Append('\n');

            Derivation?.LogState(strbuf);
        }

        internal ShowFlags Show(bool all)
        {
            var showFlags = ShowFlags.None;
            if (_visible)
            {
                if (Substance != null) showFlags |= Substance.Show();
                if (all && Derivation != null) showFlags |= Derivation.Show();
            }
            return showFlags;
        }

        internal override ShowFlags Show() => Show(false);

        internal void Draw(Graphics graphics, InstanceLayer layer, bool all)
        {
            if (_visible)
            {
                graphics.Push();
                graphics.StencilMode = Origin.Stencil && layer != InstanceLayer.Cursor ? StencilMode.Inclusive : StencilMode.None;

                Substance?.Draw(graphics, layer);
                if (all) Derivation?.Draw(graphics, layer);

                if (layer == InstanceLayer.Cursor && GetTransform(Progress, null, out var transform))
                {
                    graphics.DepthMode = DepthMode.ReadWrite;
                    graphics.Material = Material.Default;
                    graphics.Material.Metallic = 0;
                    graphics.Material.Roughness = 1;
                    graphics.DrawSphere(transform.Translation, 10);
                }
                
                graphics.Pop();
            }
        }

        internal override void Draw(Graphics graphics, InstanceLayer layer) => Draw(graphics, layer, false);

        internal override void Select(bool focus)
        {
            _focus = focus;
            
            if (_SubstanceView) Substance?.Select(focus);
        }

        internal override bool KeyDown(KeyEventArgs e) => _SubstanceView && Substance != null && Substance.KeyDown(e);
        internal override bool KeyUp(KeyEventArgs e) => _SubstanceView && Substance != null && Substance.KeyUp(e);
        internal override bool MouseDown(MouseEventArgs e, bool controlKey, bool shiftKey) => _SubstanceView && Substance != null && Substance.MouseDown(e, controlKey, shiftKey);
        internal override bool MouseUp(MouseEventArgs e, bool controlKey, bool shiftKey) => _SubstanceView && Substance != null && Substance.MouseUp(e, controlKey, shiftKey);
        internal override bool MouseMove(MouseEventArgs e, int prevX, int prevY, bool controlKey, bool shiftKey) => _SubstanceView && Substance != null && Substance.MouseMove(e, prevX, prevY, controlKey, shiftKey);
        internal override bool MouseWheel(MouseEventArgs e, bool controlKey, bool shiftKey) => _SubstanceView && Substance != null && Substance.MouseWheel(e, controlKey, shiftKey);
        protected override void SaveContent(XmlWriter writer) => throw new InvalidOperationException();
        protected override void LoadContent(XmlNode node) => throw new InvalidOperationException();
    }
}

using System;

namespace CDK.Assets.Support
{
    public struct ScreenRatioItem : IEquatable<ScreenRatioItem>
    {
        public float Value { private set; get; }

        public ScreenRatioItem(float value)
        {
            Value = value;
        }

        public override string ToString() => $"{(int)Math.Round(Value * 100)}%";
        public bool Equals(ScreenRatioItem other) => Value == other.Value;
        public override bool Equals(object obj) => obj is ScreenRatioItem other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
    }

    public class ScreenRatio : NotifyPropertyChanged
    {
        private int _SelectedIndex;
        public int SelectedIndex
        {
            set
            {
                if (value < 0) value = DefaultIndex;

                if (SetProperty(ref _SelectedIndex, value))
                {
                    OnPropertyChanged("SelectedItem");
                    OnPropertyChanged("Value");
                }
            }
            get => _SelectedIndex;
        }

        public ScreenRatioItem SelectedItem
        {
            set => SelectedIndex = Array.IndexOf(Items, value);
            get => Items[_SelectedIndex];
        }

        public float Value => SelectedItem.Value;

        public static implicit operator float(ScreenRatio ratio) => ratio.SelectedItem.Value;

        public ScreenRatio()
        {
            _SelectedIndex = DefaultIndex;
        }

        public void Up()
        {
            if (_SelectedIndex < Items.Length - 1) SelectedIndex++;
        }

        public void Down()
        {
            if (_SelectedIndex > 0) SelectedIndex--;
        }

        private const int DefaultIndex = 9;

        public static readonly ScreenRatioItem[] Items ={
            new ScreenRatioItem(0.1f),
            new ScreenRatioItem(0.2f),
            new ScreenRatioItem(0.3f),
            new ScreenRatioItem(0.4f),
            new ScreenRatioItem(0.5f),
            new ScreenRatioItem(0.6f),
            new ScreenRatioItem(0.7f),
            new ScreenRatioItem(0.8f),
            new ScreenRatioItem(0.9f),
            new ScreenRatioItem(1.0f),
            new ScreenRatioItem(2.0f),
            new ScreenRatioItem(3.0f),
            new ScreenRatioItem(4.0f),
            new ScreenRatioItem(5.0f),
            new ScreenRatioItem(6.0f),
            new ScreenRatioItem(7.0f),
            new ScreenRatioItem(8.0f),
            new ScreenRatioItem(9.0f),
            new ScreenRatioItem(10.0f)
        };
    }
}

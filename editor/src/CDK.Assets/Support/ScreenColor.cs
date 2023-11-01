using System;
using System.Drawing;

namespace CDK.Assets
{
    public struct ScreenColorItem : IEquatable<ScreenColorItem>
    {
        public Color Foreground { private set; get; }
        public Color Background { private set; get; }

        public ScreenColorItem(in Color foreground, in Color background)
        {
            Foreground = foreground;
            Background = background;
        }

        public ScreenColorItem(Color background)
            : this(Color.FromArgb(255 - background.R, 255 - background.G, 255 - background.B), background)
        {

        }

        public override string ToString() => Background.Name;

        public bool Equals(ScreenColorItem other) => Foreground == other.Foreground && Background == other.Background;
        public override bool Equals(object obj) => obj is ScreenColorItem other && Equals(other);
        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(Foreground.GetHashCode());
            hash.Combine(Background.GetHashCode());
            return hash;
        }
    }

    public class ScreenColor : NotifyPropertyChanged
    {
        private int _SelectedIndex;
        public int SelectedIndex
        {
            set
            {
                if (value < 0) value = 0;

                if (SetProperty(ref _SelectedIndex, value))
                {
                    OnPropertyChanged("SelectedItem");
                    OnPropertyChanged("Foreground");
                    OnPropertyChanged("Background");
                }
            }
            get => _SelectedIndex;
        }

        public ScreenColorItem SelectedItem
        {
            set => SelectedIndex = Array.IndexOf(Items, value);
            get => Items[_SelectedIndex];
        }

        public Color Foreground => SelectedItem.Foreground;
        public Color Background => SelectedItem.Background;

        public void Up()
        {
            if (_SelectedIndex < Items.Length - 1) SelectedIndex++;
        }

        public void Down()
        {
            if (_SelectedIndex > 0) SelectedIndex--;
        }

        public static readonly ScreenColorItem[] Items = {
            new ScreenColorItem(Color.Black),
            new ScreenColorItem(Color.White, Color.Gray),
            new ScreenColorItem(Color.White),
            new ScreenColorItem(Color.Magenta),
            new ScreenColorItem(Color.LightBlue),
            new ScreenColorItem(Color.Blue),
            new ScreenColorItem(Color.DarkBlue),
            new ScreenColorItem(Color.Orange),
            new ScreenColorItem(Color.Red),
            new ScreenColorItem(Color.Brown),
            new ScreenColorItem(Color.LightGreen),
            new ScreenColorItem(Color.Green),
            new ScreenColorItem(Color.DarkGreen)
        };
    }
}

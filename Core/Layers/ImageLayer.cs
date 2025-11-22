namespace ImageEditor.Core.Layers;

public class ImageLayer : Layer
{
    public Bitmap Image { get; set; } = new(800, 600);

    public override Bitmap Render() => Image;

    public override Layer Clone() => new ImageLayer
    {
        Name = Name + " (копія)",
        Image = (Bitmap)Image.Clone(),
        Visible = Visible,
        Opacity = Opacity
    };

    public override void ApplyEffect(Effects.Effect? effect) => effect?.Apply(Image);
}
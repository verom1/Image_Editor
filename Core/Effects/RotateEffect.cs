namespace ImageEditor.Core.Effects;

public class RotateEffect : Effect
{
    public float Angle { get; set; } = 90;

    public override void Apply(Bitmap bmp)
    {
        var type = Angle switch
        {
            90 => RotateFlipType.Rotate90FlipNone,
            180 => RotateFlipType.Rotate180FlipNone,
            270 => RotateFlipType.Rotate270FlipNone,
            _ => RotateFlipType.RotateNoneFlipNone
        };
        bmp.RotateFlip(type);
    }

    public override object Clone() => new RotateEffect { Angle = Angle };
}
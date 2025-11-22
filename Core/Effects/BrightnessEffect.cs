// BrightnessEffect.cs
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageEditor.Core.Effects;

public class BrightnessEffect : Effect
{
    /// <summary>
    /// Дельта яскравості: -255 (чорний) ... +255 (білий)
    /// </summary>
    public int Delta { get; set; } = 50;

    public override void Apply(Bitmap bmp)
    {
        if (Delta == 0) return;

        float brightness = Delta / 255f;
        brightness = Math.Clamp(brightness, -1f, 1f);

        var matrix = new ColorMatrix(new float[][]
        {
            new float[] { 1, 0, 0, 0, 0 },
            new float[] { 0, 1, 0, 0, 0 },
            new float[] { 0, 0, 1, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 },
            new float[] { brightness, brightness, brightness, 0, 1 }
        });

        var attributes = new ImageAttributes();
        attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        using var g = Graphics.FromImage(bmp);
        g.DrawImage(
            bmp,
            new Rectangle(0, 0, bmp.Width, bmp.Height),
            0, 0, bmp.Width, bmp.Height,
            GraphicsUnit.Pixel,
            attributes);
    }

    public override object Clone() => new BrightnessEffect { Delta = this.Delta };
}
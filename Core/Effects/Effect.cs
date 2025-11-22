// Effect.cs
namespace ImageEditor.Core.Effects;

public abstract class Effect : ICloneable
{
    public abstract void Apply(System.Drawing.Bitmap bmp);
    public abstract object Clone();
}
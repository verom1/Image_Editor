namespace ImageEditor.Core.Layers;

public abstract class Layer
{
    public string Name { get; set; } = "Шар";
    public bool Visible { get; set; } = true;
    public float Opacity { get; set; } = 1f;

    public abstract Bitmap Render();
    public abstract Layer Clone();
    public virtual void ApplyEffect(Effects.Effect? effect) { }
}
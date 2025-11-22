namespace ImageEditor.Core.Layers;

public class GroupLayer : Layer
{
    public List<Layer> Children { get; } = new();

    public override Bitmap Render()
    {
        var bmp = new Bitmap(1920, 1080);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.Transparent);
        foreach (var child in Children.Where(c => c.Visible))
            g.DrawImage(child.Render(), 0, 0);
        return bmp;
    }

    public override Layer Clone()
    {
        var g = new GroupLayer { Name = Name + " (копія)" };
        g.Children.AddRange(Children.Select(c => c.Clone()));
        return g;
    }
}
using ImageEditor.Core.Layers;
using ImageEditor.Core.Effects;

namespace ImageEditor.Core.Tools;

public abstract class Tool
{
    public virtual void MouseDown(Canvas canvas, Point p) { }
    public virtual void MouseDrag(Canvas canvas, Point p) { }
    public virtual void MouseUp(Canvas canvas, Point p) { }
}
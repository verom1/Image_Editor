// CropTool.cs
using System.Drawing;
using ImageEditor.Core.Layers;   // ← ЦЕ ВАЖЛИВО!

namespace ImageEditor.Core.Tools;

public class CropTool : Tool
{
    private Point? _start;

    public override void MouseDown(Canvas canvas, Point p) => _start = p;

    public override void MouseDrag(Canvas canvas, Point p)
    {
        if (_start.HasValue)
        {
            int x = Math.Min(_start.Value.X, p.X);
            int y = Math.Min(_start.Value.Y, p.Y);
            int w = Math.Abs(p.X - _start.Value.X);
            int h = Math.Abs(p.Y - _start.Value.Y);
            canvas.Selection = new Rectangle(x, y, w, h);
        }
    }

    public override void MouseUp(Canvas canvas, Point p)
    {
        if (canvas.Selection.HasValue && 
            canvas.ActiveLayer is ImageLayer imgLayer && 
            canvas.Selection.Value.Width > 0 && 
            canvas.Selection.Value.Height > 0)
        {
            var rect = canvas.Selection.Value;
            var cropped = new Bitmap(rect.Width, rect.Height);
            using (var g = Graphics.FromImage(cropped))
                g.DrawImage(imgLayer.Image, 0, 0, rect, GraphicsUnit.Pixel);

            imgLayer.Image = cropped;
        }

        canvas.Selection = null;
        _start = null;
    }
}
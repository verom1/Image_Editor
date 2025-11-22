using ImageEditor.Core.Effects;
using ImageEditor.Core.Layers;
using ImageEditor.Core.Tools;

namespace ImageEditor.Core.Facade;

public class ImageEditorFacade
{
    private readonly Canvas _canvas = new();
    private Tool _currentTool = new SelectTool();
    private readonly Caretaker _history = new();

    public ImageEditorFacade() => SaveState();

    public void SetTool(Tool tool) => _currentTool = tool;
    public void MouseDown(Point p) => _currentTool.MouseDown(_canvas, p);
    public void MouseDrag(Point p) => _currentTool.MouseDrag(_canvas, p);
    public void MouseUp(Point p) { _currentTool.MouseUp(_canvas, p); SaveState(); }

    public void AddLayer(Layer layer)
    {
        _canvas.Layers.Add(layer);
        _canvas.ActiveLayer = layer;
        SaveState();
    }

    public void ApplyEffect(Effect effect)
    {
        _canvas.ActiveLayer?.ApplyEffect(effect);
        SaveState();
    }

    public void Undo() { if (_history.CanUndo) _canvas.Restore(_history.Undo()); }
    public void Redo() { if (_history.CanRedo) _canvas.Restore(_history.Redo()); }

    public void Render(Graphics g)
    {
        g.Clear(Color.White);
        g.DrawImage(_canvas.Render(), 0, 0);
    }

    private void SaveState() => _history.Save(_canvas.CreateMemento());
}
// Canvas.cs — містить Canvas + CanvasMemento + Caretaker
using System.Drawing;
using ImageEditor.Core.Layers;

namespace ImageEditor.Core;

/// <summary>
/// Memento — зберігає стан Canvas (усі шари + активний шар)
/// </summary>
public sealed class CanvasMemento
{
    public List<Layer> Layers { get; }
    public Layer? ActiveLayer { get; }

    // Конструктор робить глибоку копію через Prototype
    public CanvasMemento(List<Layer> layers, Layer? activeLayer)
    {
        Layers = layers.Select(l => l.Clone()).ToList();
        ActiveLayer = activeLayer?.Clone();
    }
}

/// <summary>
/// Caretaker — відповідає за історію Undo/Redo
/// </summary>
public sealed class Caretaker
{
    private readonly Stack<CanvasMemento> _undoStack = new();
    private readonly Stack<CanvasMemento> _redoStack = new();

    public void Save(CanvasMemento memento)
    {
        _undoStack.Push(memento);
        _redoStack.Clear();
    }

    public CanvasMemento Undo()
    {
        if (_undoStack.Count <= 1) return _undoStack.Peek(); // не даємо знищити початковий стан

        var previous = _undoStack.Pop();
        _redoStack.Push(previous);
        return _undoStack.Peek();
    }

    public CanvasMemento Redo()
    {
        if (_redoStack.Count == 0) return _undoStack.Peek();

        var next = _redoStack.Pop();
        _undoStack.Push(next);
        return next;
    }

    public bool CanUndo => _undoStack.Count > 1;
    public bool CanRedo => _redoStack.Count > 0;
}

/// <summary>
/// Основний клас Canvas — містить шари, виділення, рендерить результат
/// </summary>
public sealed class Canvas
{
    public List<Layer> Layers { get; } = new();
    public Layer? ActiveLayer { get; set; }
    public Rectangle? Selection { get; set; }   // для CropTool

    // Memento: створення знімка стану
    public CanvasMemento CreateMemento() => new(Layers, ActiveLayer);

    // Memento: відновлення стану
    public void Restore(CanvasMemento memento)
    {
        Layers.Clear();
        Layers.AddRange(memento.Layers);
        ActiveLayer = memento.ActiveLayer;
        Selection = null;
    }

    /// <summary>
    /// Рендерить усі видимі шари у фінальне зображення
    /// </summary>
    public Bitmap Render()
    {
        // Розмір можна зробити динамічним — поки фіксований для простоти
        var result = new Bitmap(1920, 1080);
        using var g = Graphics.FromImage(result);
        g.Clear(Color.White);

        foreach (var layer in Layers.Where(l => l.Visible))
        {
            var layerBmp = layer.Render();
            g.DrawImage(layerBmp, Point.Empty);
        }

        // Малюємо виділення (для інструменту Crop)
        if (Selection.HasValue)
        {
            using var pen = new Pen(Color.FromArgb(180, 255, 0, 0), 3)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
            };
            g.DrawRectangle(pen, Selection.Value);
        }

        return result;
    }
}
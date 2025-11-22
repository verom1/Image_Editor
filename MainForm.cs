// MainForm.cs — остаточна версія, працює на .NET 10.0.100
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ImageEditor.Core;
using ImageEditor.Core.Facade;
using ImageEditor.Core.Effects;
using ImageEditor.Core.Layers;
using ImageEditor.Core.Tools;

namespace ImageEditor;

public partial class MainForm : Form
{
    private readonly ImageEditorFacade _editor;
    private readonly ConfigurationManager _config = ConfigurationManager.Instance;

    public MainForm(ImageEditorFacade editor)
    {
        _editor = editor;
        InitializeComponent();

        RestoreLastTool();
        FormClosing += (s, e) => _config.Save();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        _editor.MouseDown(e.Location);
        Invalidate();
        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _editor.MouseDrag(e.Location);
            Invalidate();
        }
        base.OnMouseMove(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        _editor.MouseUp(e.Location);
        Invalidate();
        base.OnMouseUp(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        _editor.Render(e.Graphics);

        if (_config.GridVisible)
        {
            using var pen = new Pen(Color.FromArgb(40, 100, 100, 100), 1)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dot
            };
            for (int x = 0; x < ClientSize.Width; x += 50)
                e.Graphics.DrawLine(pen, x, 0, x, ClientSize.Height);
            for (int y = 0; y < ClientSize.Height; y += 50)
                e.Graphics.DrawLine(pen, 0, y, ClientSize.Width, y);
        }

        base.OnPaint(e);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.Z)) { _editor.Undo(); Invalidate(); return true; }
        if (keyData == (Keys.Control | Keys.Y)) { _editor.Redo(); Invalidate(); return true; }
        if (keyData == (Keys.Control | Keys.G))
        {
            _config.GridVisible = !_config.GridVisible;
            Invalidate();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void InitializeComponent()
    {
        Text = "Редактор зображень — 6 патернів GoF + Singleton";
        Size = new Size(1250, 800);
        MinimumSize = new Size(800, 600);
        StartPosition = FormStartPosition.CenterScreen;
        DoubleBuffered = true;
        BackColor = Color.FromArgb(45, 45, 48);

        var menu = new MenuStrip { BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.WhiteSmoke };

        // ─── Файл ───
        var fileMenu = new ToolStripMenuItem("Файл");
        var openItem = new ToolStripMenuItem("Відкрити...");
        openItem.Click += (_, __) => OpenImage();
        openItem.ShortcutKeys = Keys.Control | Keys.O;           // ← правильне місце в .NET 10
        var exitItem = new ToolStripMenuItem("Вихід");
        exitItem.Click += (_, __) => Close();
        fileMenu.DropDownItems.AddRange(new ToolStripItem[] { openItem, new ToolStripSeparator(), exitItem });

        // ─── Інструменти ───
        var toolsMenu = new ToolStripMenuItem("Інструменти");
        toolsMenu.DropDownItems.Add("Вибір",    null, (_, __) => { _editor.SetTool(new SelectTool()); _config.LastTool = "Select"; });
        toolsMenu.DropDownItems.Add("Рука",     null, (_, __) => { _editor.SetTool(new HandTool());  _config.LastTool = "Hand"; });
        toolsMenu.DropDownItems.Add("Обрізка",  null, (_, __) => { _editor.SetTool(new CropTool());  _config.LastTool = "Crop"; });

        // ─── Ефекти ───
        var effectsMenu = new ToolStripMenuItem("Ефекти");
        effectsMenu.DropDownItems.Add("Повернути 90°", null, (_, __) =>
        {
            _editor.ApplyEffect(new RotateEffect { Angle = 90 });
            Invalidate();
        });
        effectsMenu.DropDownItems.Add("Яскравість +50", null, (_, __) =>
        {
            _editor.ApplyEffect(new BrightnessEffect { Delta = 50 });
            Invalidate();
        });
        effectsMenu.DropDownItems.Add("Яскравість -50", null, (_, __) =>
        {
            _editor.ApplyEffect(new BrightnessEffect { Delta = -50 });
            Invalidate();
        });

        // ─── Правка ───
        var editMenu = new ToolStripMenuItem("Правка");

        var undoItem = new ToolStripMenuItem("Відмінити");
        undoItem.Click += (_, __) => { _editor.Undo(); Invalidate(); };
        undoItem.ShortcutKeys = Keys.Control | Keys.Z;           // ← правильне місце

        var redoItem = new ToolStripMenuItem("Повторити");
        redoItem.Click += (_, __) => { _editor.Redo(); Invalidate(); };
        redoItem.ShortcutKeys = Keys.Control | Keys.Y;           // ← правильне місце

        var gridItem = new ToolStripMenuItem("Сітка (Ctrl+G)");
        gridItem.Click += (_, __) => { _config.GridVisible = !_config.GridVisible; Invalidate(); };

        editMenu.DropDownItems.AddRange(new ToolStripItem[] { undoItem, redoItem, new ToolStripSeparator(), gridItem });

        menu.Items.AddRange(new ToolStripItem[] { fileMenu, toolsMenu, effectsMenu, editMenu });
        MainMenuStrip = menu;
        Controls.Add(menu);
    }

    private void OpenImage()
    {
        using var dlg = new OpenFileDialog
        {
            Filter = "Зображення|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.webp;*.tiff",
            InitialDirectory = _config.LastOpenPath
        };

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            _config.LastOpenPath = Path.GetDirectoryName(dlg.FileName)!;

            try
            {
                var bmp = new Bitmap(dlg.FileName);
                var layer = new ImageLayer { Name = Path.GetFileName(dlg.FileName), Image = bmp };
                _editor.AddLayer(layer);
                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося відкрити файл:\n{ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void RestoreLastTool()
    {
        Tool tool = _config.LastTool switch
        {
            "Crop" => new CropTool(),
            "Hand" => new HandTool(),
            _ => new SelectTool()
        };
        _editor.SetTool(tool);
    }
}
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundForgeController;

public partial class Form1 : Form
{
    private Label statusLabel = null!;
    private Label activeLabel = null!;
    private RoundedButton? activeButton;
    private bool isFullscreen = true;

    private readonly (string Text, string Shortcut, string Name, Color ButtonColor)[] commands =
    {
        ("▶ PLAY",    "RESTART_PLAY", "PLAY",  Color.FromArgb(30, 170, 85)),
        ("⏸ PAUSE",  "{ENTER}",      "PAUSE", Color.FromArgb(230, 185, 35)),
        ("■ STOP",   "{ESC}",        "STOP",  Color.FromArgb(210, 45, 45)),
        ("⌂ HOME",   "^{HOME}",      "HOME",  Color.FromArgb(120, 80, 210))
    };

    public Form1()
    {
        InitializeComponent();
        Controls.Clear();

        Text = "Topesa Sound Forge Controller";
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        StartPosition = FormStartPosition.CenterScreen;
        TopMost = true;
        KeyPreview = true;
        BackColor = Color.FromArgb(12, 14, 20);

        KeyDown += Form1_KeyDown;

        BuildUi();
    }

    private void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F11)
        {
            ToggleFullscreen();
        }

        if (e.KeyCode == Keys.Escape)
        {
            Close();
        }
    }

    private void BuildUi()
    {
        var main = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            BackColor = Color.FromArgb(12, 14, 20),
            Padding = new Padding(28)
        };

        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 115));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));

        main.Controls.Add(BuildHeader(), 0, 0);
        main.Controls.Add(BuildButtonGrid(), 0, 1);
        main.Controls.Add(BuildFooter(), 0, 2);

        Controls.Add(main);
    }

    private Control BuildHeader()
    {
        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(12, 14, 20)
        };

        var title = new Label
        {
            Text = "Topesa Sound Forge Controller",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 34, FontStyle.Bold)
        };

        var hint = new Label
        {
            Text = "F11 — Fullscreen / Window",
            Dock = DockStyle.Bottom,
            Height = 26,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(135, 145, 160),
            Font = new Font("Segoe UI", 10, FontStyle.Regular)
        };

        var exitButton = new Button
        {
            Text = "EXIT",
            Dock = DockStyle.Right,
            Width = 130,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(95, 25, 30),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        exitButton.FlatAppearance.BorderSize = 0;
        exitButton.Click += (_, _) => Close();

        var minimizeButton = new Button
        {
            Text = "MIN",
            Dock = DockStyle.Right,
            Width = 130,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(35, 45, 70),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        minimizeButton.FlatAppearance.BorderSize = 0;
        minimizeButton.Click += (_, _) => WindowState = FormWindowState.Minimized;

        header.Controls.Add(title);
        header.Controls.Add(exitButton);
        header.Controls.Add(minimizeButton);
        header.Controls.Add(hint);

        return header;
    }

    private Control BuildButtonGrid()
    {
        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(28),
            BackColor = Color.FromArgb(12, 14, 20)
        };

        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        foreach (var cmd in commands)
        {
            var button = MakeButton(cmd.Text, cmd.Shortcut, cmd.Name, cmd.ButtonColor);
            grid.Controls.Add(button);
        }

        return grid;
    }

    private Control BuildFooter()
    {
        var footer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.FromArgb(12, 14, 20),
            Padding = new Padding(12, 8, 12, 8)
        };

        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));

        activeLabel = new Label
        {
            Text = "ACTIVE: NONE",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(28, 31, 42),
            Font = new Font("Segoe UI", 20, FontStyle.Bold)
        };

        statusLabel = new Label
        {
            Text = "Ready — გახსენი Sound Forge და დააჭირე ღილაკს",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.Gainsboro,
            BackColor = Color.FromArgb(20, 22, 30),
            Font = new Font("Segoe UI", 14, FontStyle.Regular)
        };

        footer.Controls.Add(activeLabel, 0, 0);
        footer.Controls.Add(statusLabel, 1, 0);

        return footer;
    }

    private RoundedButton MakeButton(string text, string shortcut, string name, Color color)
    {
        var btn = new RoundedButton(color)
        {
            Text = text,
            Dock = DockStyle.Fill,
            Margin = new Padding(24),
            Font = new Font("Segoe UI", 42, FontStyle.Bold),
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };

        btn.Click += async (_, _) =>
        {
            bool sent = await SendShortcutToSoundForge(name, shortcut);

            if (sent)
            {
                SetActiveButton(btn, name);
            }
        };

        return btn;
    }

    private void SetActiveButton(RoundedButton btn, string name)
    {
        if (activeButton != null)
        {
            activeButton.SetActive(false);
        }

        btn.SetActive(true);
        activeButton = btn;

        activeLabel.Text = $"ACTIVE: {name}";
    }

    private async Task<bool> SendShortcutToSoundForge(string commandName, string shortcut)
    {
        IntPtr hwnd = FindSoundForgeWindow();

        if (hwnd == IntPtr.Zero)
        {
            statusLabel.Text = "Sound Forge ვერ ვიპოვე — ჯერ გაუშვი პროგრამა";
            return false;
        }

        if (IsIconic(hwnd))
        {
            ShowWindow(hwnd, SW_RESTORE);
        }

        SetForegroundWindow(hwnd);

        await Task.Delay(150);

        try
        {
            if (shortcut == "RESTART_PLAY")
            {
                SendKeys.SendWait("{ESC}");
                await Task.Delay(80);

                SendKeys.SendWait("^{HOME}");
                await Task.Delay(80);

                SendKeys.SendWait(" ");
            }
            else
            {
                SendKeys.SendWait(shortcut);
            }

            statusLabel.Text = $"{commandName} გაგზავნილია Sound Forge-ში";
            return true;
        }
        catch (Exception ex)
        {
            statusLabel.Text = "შეცდომა: " + ex.Message;
            return false;
        }
    }

    private IntPtr FindSoundForgeWindow()
    {
        int currentProcessId = Process.GetCurrentProcess().Id;

        foreach (var p in Process.GetProcesses())
        {
            try
            {
                if (p.Id == currentProcessId)
                {
                    continue;
                }

                if (p.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }

                string title = p.MainWindowTitle ?? "";
                string process = p.ProcessName ?? "";

                if (title.Contains("Topesa Sound Forge Controller", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                bool found =
                    title.Contains("Sound Forge", StringComparison.OrdinalIgnoreCase) ||
                    title.Contains("SOUND FORGE", StringComparison.OrdinalIgnoreCase) ||
                    process.Contains("forge", StringComparison.OrdinalIgnoreCase);

                if (found)
                {
                    return p.MainWindowHandle;
                }
            }
            catch
            {
            }
        }

        return IntPtr.Zero;
    }

    private void ToggleFullscreen()
    {
        if (isFullscreen)
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = FormWindowState.Normal;
            Width = 1100;
            Height = 720;
            TopMost = false;
            isFullscreen = false;
        }
        else
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            TopMost = true;
            isFullscreen = true;
        }
    }

    private const int SW_RESTORE = 9;

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}

public class RoundedButton : Button
{
    private readonly Color buttonColor;
    private bool isHovering;
    private bool isPressed;
    private bool isActive;

    public RoundedButton(Color color)
    {
        buttonColor = color;

        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        BackColor = Color.Transparent;

        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true
        );
    }

    public void SetActive(bool value)
    {
        isActive = value;
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        isHovering = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        isHovering = false;
        isPressed = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        isPressed = true;
        Invalidate();
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        isPressed = false;
        Invalidate();
        base.OnMouseUp(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Parent?.BackColor ?? Color.FromArgb(12, 14, 20));

        var rect = new Rectangle(8, 8, Width - 16, Height - 16);
        int radius = 38;

        Color drawColor = buttonColor;

        if (isPressed)
        {
            drawColor = ControlPaint.Dark(buttonColor, 0.30f);
        }
        else if (isHovering)
        {
            drawColor = ControlPaint.Light(buttonColor, 0.16f);
        }

        using var shadowPath = RoundedRect(
            new Rectangle(rect.X + 6, rect.Y + 8, rect.Width, rect.Height),
            radius
        );

        using var shadowBrush = new SolidBrush(Color.FromArgb(90, 0, 0, 0));
        e.Graphics.FillPath(shadowBrush, shadowPath);

        using var buttonPath = RoundedRect(rect, radius);

        using var brush = new LinearGradientBrush(
            rect,
            ControlPaint.Light(drawColor, 0.18f),
            ControlPaint.Dark(drawColor, 0.16f),
            LinearGradientMode.Vertical
        );

        e.Graphics.FillPath(brush, buttonPath);

        if (isActive)
        {
            using var activePen = new Pen(Color.White, 7);
            e.Graphics.DrawPath(activePen, buttonPath);

            using var dotBrush = new SolidBrush(Color.White);
            e.Graphics.FillEllipse(dotBrush, rect.Right - 50, rect.Top + 24, 24, 24);
        }
        else
        {
            using var borderPen = new Pen(Color.FromArgb(120, 255, 255, 255), 2);
            e.Graphics.DrawPath(borderPen, buttonPath);
        }

        TextRenderer.DrawText(
            e.Graphics,
            Text,
            Font,
            rect,
            ForeColor,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.EndEllipsis
        );
    }

    private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        int diameter = radius * 2;
        var path = new GraphicsPath();

        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}
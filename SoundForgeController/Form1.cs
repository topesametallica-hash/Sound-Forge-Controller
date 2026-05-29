using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundForgeController;

public partial class Form1 : Form
{
    private Label statusLabel = null!;

    private readonly (string Text, string Keys, string Name)[] commands =
    {
        ("▶ PLAY",   " ",       "Play"),
        ("⏸ PAUSE", "{ENTER}", "Pause"),
        ("🔁 LOOP",  "q",       "Loop"),
        ("■ STOP",  "{ESC}",   "Stop")
    };

    public Form1()
    {
        InitializeComponent();

        Controls.Clear();

        Text = "Sound Forge Controller";
        Width = 900;
        Height = 560;
        MinimumSize = new Size(700, 430);
        StartPosition = FormStartPosition.CenterScreen;
        TopMost = true;
        BackColor = Color.FromArgb(25, 25, 25);

        var title = new Label
        {
            Text = "SOUND FORGE CONTROLLER",
            Dock = DockStyle.Top,
            Height = 70,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 24, FontStyle.Bold)
        };

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(18),
            BackColor = Color.FromArgb(25, 25, 25)
        };

        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        foreach (var cmd in commands)
        {
            grid.Controls.Add(MakeButton(cmd.Text, cmd.Keys, cmd.Name));
        }

        statusLabel = new Label
        {
            Text = "Ready — გახსენი Sound Forge და დააჭირე ღილაკს",
            Dock = DockStyle.Bottom,
            Height = 44,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.Gainsboro,
            Font = new Font("Segoe UI", 11, FontStyle.Regular)
        };

        Controls.Add(grid);
        Controls.Add(title);
        Controls.Add(statusLabel);
    }

    private Button MakeButton(string text, string keys, string name)
    {
        var btn = new Button
        {
            Text = text,
            Dock = DockStyle.Fill,
            Margin = new Padding(14),
            Font = new Font("Segoe UI", 34, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(50, 50, 50),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        btn.FlatAppearance.BorderSize = 2;
        btn.FlatAppearance.BorderColor = Color.FromArgb(120, 120, 120);

        btn.Click += async (_, _) => await SendShortcutToSoundForge(name, keys);

        return btn;
    }

    private async Task SendShortcutToSoundForge(string commandName, string keys)
    {
        IntPtr hwnd = FindSoundForgeWindow();

        if (hwnd == IntPtr.Zero)
        {
            statusLabel.Text = "Sound Forge ვერ ვიპოვე — ჯერ გაუშვი პროგრამა";
            return;
        }

        if (IsIconic(hwnd))
        {
            ShowWindow(hwnd, SW_RESTORE);
        }

        SetForegroundWindow(hwnd);

        await Task.Delay(150);

        try
        {
            SendKeys.SendWait(keys);
            statusLabel.Text = $"{commandName} გაგზავნილია";
        }
        catch (Exception ex)
        {
            statusLabel.Text = "შეცდომა: " + ex.Message;
        }
    }

    private IntPtr FindSoundForgeWindow()
    {
        foreach (var p in Process.GetProcesses())
        {
            try
            {
                if (p.MainWindowHandle == IntPtr.Zero)
                    continue;

                string title = p.MainWindowTitle ?? "";
                string process = p.ProcessName ?? "";

                bool found =
                    title.Contains("Sound Forge", StringComparison.OrdinalIgnoreCase) ||
                    title.Contains("SOUND FORGE", StringComparison.OrdinalIgnoreCase) ||
                    process.Contains("forge", StringComparison.OrdinalIgnoreCase);

                if (found)
                    return p.MainWindowHandle;
            }
            catch
            {
                // ზოგ პროცესზე Windows წვდომას არ მოგვცემს, ვატარებთ
            }
        }

        return IntPtr.Zero;
    }

    private const int SW_RESTORE = 9;

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
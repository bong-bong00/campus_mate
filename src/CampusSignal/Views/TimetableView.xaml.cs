using System.Windows;
using System.Windows.Controls;
using CampusSignal.Infrastructure;
using CampusSignal.ViewModels;

namespace CampusSignal.Views;

public partial class TimetableView : UserControl
{
    public TimetableView() => InitializeComponent();

    private TimetableViewModel? Vm => DataContext as TimetableViewModel;

    private static bool HasFiles(DragEventArgs e) => e.Data.GetDataPresent(DataFormats.FileDrop);

    private void OnDragOver(object sender, DragEventArgs e)
    {
        e.Effects = HasFiles(e) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
        if (!HasFiles(e)) return;

        // 드래그 오버 피드백 — 테두리 진초록 + 배경 연녹
        DropOutline.Stroke = Palette.Get("EligibleFg");
        DropOutline.Fill = Palette.Get("AccentBgSoft");
    }

    private void OnDragLeave(object sender, DragEventArgs e) => ResetDropVisual();

    private void OnDrop(object sender, DragEventArgs e)
    {
        ResetDropVisual();
        if (!HasFiles(e)) return;

        if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
            Vm?.ImportFiles(paths);

        e.Handled = true;
    }

    private void ResetDropVisual()
    {
        DropOutline.Stroke = Palette.Get("BorderDashed");
        DropOutline.Fill = System.Windows.Media.Brushes.Transparent;
    }
}

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FiaMedKnuffGrupp4
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly Grid grid = new Grid();
        private int numberOfColumnsInGrid = 15;
        private bool drawGrid = true; //For debugging purposes
        private float cellSize;
        CanvasDrawingSession drawingSession;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            drawingSession = args.DrawingSession;
            float fontSize = 12;

            // Create a CanvasTextFormat with the desired font size
            var textFormat = new CanvasTextFormat
            {
                FontSize = fontSize
            };

            // Loop through all cells and draw a black outline with coordinates inside each cell.
            if (drawGrid)
            {
                for (int y = 0; y < 15; y++)
                {
                    for (int x = 0; x < 15; x++)
                    {
                        float cellX = x * cellSize;
                        float cellY = y * cellSize;

                        drawingSession.DrawRectangle(cellX, cellY, cellSize, cellSize, Colors.Black, 1);

                        string coordinates = $"{x}, {y}";
                        drawingSession.DrawText(coordinates, cellX + cellSize / 4, cellY + cellSize / 4, Colors.Black, textFormat);
                    }
                }
            }

        }

        private void canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            setCellSize();
            setCanvasMargin();
        }

        private void canvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {

        }

        private async void setCellSize()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                cellSize = (float)backgroundImage.ActualHeight / numberOfColumnsInGrid;
            });
        }

        private async void setCanvasMargin()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                canvas.Margin = new Thickness((boardGrid.ActualWidth - backgroundImage.ActualWidth) / 2, (boardGrid.ActualHeight - backgroundImage.ActualHeight) / 2,
                                                                (boardGrid.ActualWidth - backgroundImage.ActualWidth) / 2, (boardGrid.ActualHeight - backgroundImage.ActualHeight) / 2);
            });
        }
    }
}
namespace Projekt.Data
{
    using global::Projekt.Models;
    using QuestPDF.Drawing;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;
    public class GenerateFileLook : IDocument
    {
        public Order Model { get; }
        
        public GenerateFileLook(Order model)
        {
            Model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item()
                        .Text($"NR ZAMÓWIENIA #{Model.OrderId}")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    column.Item().Text(text =>
                    {
                        text.Span("DATA ZAMÓWIENIA: ").SemiBold();
                        text.Span($"{Model.OrderDate:d}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("KLIENT: ").SemiBold();
                        text.Span($"{Model.User?.FullName ?? "Brak danych":d}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("STATUS: ").SemiBold();
                        text.Span($"{Model.Status:d}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("ŁĄCZNA KWOTA: ").SemiBold();
                        text.Span($"{Model.Total:f}");
                    });
                });

                row.ConstantItem(100).Height(50).Placeholder();
            });
        }

        void ComposeContent(IContainer container)
        {
            container
                .PaddingVertical(40)
                .Height(250)
                .Background(Colors.Grey.Lighten3)
                .AlignCenter()
                .AlignMiddle()
                .Text("Content").FontSize(16);
        }
    }
}

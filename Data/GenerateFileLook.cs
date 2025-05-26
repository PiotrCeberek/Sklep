namespace Projekt.Data
{
    using global::Projekt.Models;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using QuestPDF.Drawing;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
    using static System.Net.Mime.MediaTypeNames;

    public class GenerateFileLook : IDocument
    {
        public Order Model { get; }
        public string Waluta { get; }
        public decimal Mnoznik { get; }

        public GenerateFileLook(Order model, string waluta, decimal mnoznik)
        {
            Model = model;
            Waluta = waluta;
            Mnoznik = mnoznik;
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
            container
                .Row(row =>
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
                        text.Span("E-MAIL: ").SemiBold();
                        text.Span($"{Model.User?.Email ?? "Brak danych":d}");
                    });


                    column.Item().Text(text =>
                    {
                        text.Span("STATUS: ").SemiBold();
                        text.Span($"{Model.Status:d}");
                    });
                });
            });
        } 

        void ComposeContent(IContainer container)
        {
            container
                .PaddingVertical(20)
                .Background(Colors.Grey.Lighten3)
                .Column(column =>
                {

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();   
                            columns.ConstantColumn(120);    
                            columns.ConstantColumn(120);
                            columns.ConstantColumn(120);
                        });

                        static IContainer HeaderCellStyle(IContainer container) => container
                            .Padding(15)
                            .Background(Colors.Grey.Lighten2)
                            .Border(1)
                            .BorderColor(Colors.Grey.Darken2)
                            .AlignCenter()                         
                            .AlignMiddle();

                        static IContainer DataCellStyle(IContainer container) => container
                            .Padding(5)                             
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten1)
                            .AlignCenter()                         
                            .AlignMiddle();              

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("Produkt").SemiBold();
                            header.Cell().Element(HeaderCellStyle).Text("Cena").SemiBold();
                            header.Cell().Element(HeaderCellStyle).Text("Ilość").SemiBold();
                            header.Cell().Element(HeaderCellStyle).Text("Zdjęcie").SemiBold();
                        });

                        foreach (var item in Model.ItemOrders)
                        {
                            table.Cell().Element(DataCellStyle).Text(item.Product.Name);
                            table.Cell().Element(DataCellStyle).Text($"{Math.Round((item.Product.Price / Mnoznik),2)} {Waluta}");
                            table.Cell().Element(DataCellStyle).Text(item.Quantity);

                            string nazwaPliku = Path.GetFileName(item.Product.ImagePath);
                            string nowaSciezka = @$"C:\Users\MSI\source\repos\PiotrCeberek\Sklep\wwwroot\images\products\{nazwaPliku}";


                            table.Cell().Element(DataCellStyle).Image(nowaSciezka);
                        }

                        column.Item()
                            .AlignRight()
                            .PaddingTop(20)
                            .PaddingRight(20)
                            .Text(text =>
                            {
                                text.Span("ŁĄCZNA KWOTA: ").SemiBold().FontSize(14);
                                text.Span($"{Math.Round((Model.Total / Mnoznik), 2):0.00} {Waluta}").FontSize(20);
                            });
                    });
                });


        }
    }
}

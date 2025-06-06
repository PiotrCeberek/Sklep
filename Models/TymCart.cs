namespace Projekt.Models
{
    public class TymCart
    {
        public int TymCartId { get; set; }

        public int OrderId { get; set; }  // Id zamówienia (powiązanie z Orders)
        public string Name { get; set; }   // Nazwa produktu
        public int Quantity { get; set; }  // Ilość w zamówieniu (z ItemOrder)
        public decimal Price { get; set; } // Cena jednostkowa
        public bool Available { get; set; } // Czy dostępny
    }
}

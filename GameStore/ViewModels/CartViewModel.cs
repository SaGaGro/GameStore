using System.Collections.Generic;
using GameStore.Models.DB;

namespace GameStore.ViewModels
{
    public class CartItem
    {
        public Game Game { get; set; }
        public decimal EffectivePrice { get; set; }   // ราคาหลัง discount ต่อเกม
        public decimal DiscountAmount { get; set; }    // ส่วนลดต่อเกม
        public string? DiscountNote { get; set; }      // เหตุผล เช่น "FestivalSale -20%"
    }

    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Subtotal { get; set; }           // ราคาเต็มรวม
        public decimal DiscountFromPromotion { get; set; } // ส่วนลดจากโปรโมชั่นรวม
        public int PointsAvailable { get; set; }        // แต้มที่มี
        public int PointsRedeemable { get; set; }       // แต้มที่ใช้ได้สูงสุด (ไม่เกิน total)
        public int PointsUsed { get; set; }             // แต้มที่จะใช้ (รับจาก form)
        public decimal PointsDiscount { get; set; }     // ส่วนลดจากแต้ม
        public decimal Total { get; set; }              // ยอดรวมสุดท้าย
        public int PointsEarned { get; set; }           // แต้มที่จะได้รับ
    }
}
